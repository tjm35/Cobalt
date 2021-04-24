using System.Collections;
using System.Collections.Generic;
using BulletFury.Data;
using BulletFury.Rendering;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace BulletFury
{
    public class BulletManager : MonoBehaviour
    {
        #region Serialized Fields
        // <----- Serialized Fields ----->
        // the maximum amount of bullets this bullet manager can show
        [SerializeField, Range(0, 1023)] private int maxBullets = 1023;

        // render priority. Higher number = drawn on top.
        [SerializeField] private int drawPriority = 0;
        
        // the settings for the bullet's behaviour over time
        [SerializeField] private BulletSettings bulletSettings = null;
        public BulletPlane Plane => bulletSettings.Plane;

        // the settings for spawning bullets
        [SerializeField] private SpawnSettings spawnSettings = null;

        [SerializeField] private int currentActiveBullets;
        [SerializeField] private int maxActiveBullets;

        public bool TrackObject
        {
            get => bulletSettings.TrackObject;
            set => bulletSettings.TrackObject = value;
        }

        // Unity Event that fires when a bullet reaches end-of-life, can be set in the inspector like a button 
        // ReSharper disable once InconsistentNaming
        [SerializeField] private BulletDiedEvent OnBulletDied;
        
        // Unity Event that fires when a bullet is spawned, can be set in the inspector like a button 
        // ReSharper disable once InconsistentNaming
        [SerializeField] private BulletSpawnedEvent OnBulletSpawned;
        #endregion

        #region Private Fields
        private BulletContainer[] _bullets;
        private bool _hasBullets;
        private BulletContainer _currentBullet;
        private Matrix4x4[] _matrices;
        private MaterialPropertyBlock _materialPropertyBlock;
        private Vector4[] _colors;
        private BulletJob _bulletJob;
        private JobHandle _handle;
        private static readonly int Color = Shader.PropertyToID("_Color");
        private float _currentTime = 0;
        private Vector3 _previousPos;
        #endregion


        /// <summary>
        /// Unity function, happens when the object is first loaded.
        /// Initialise the data.
        /// </summary>
        private void Awake()
        {
            _bullets = new BulletContainer[maxBullets];
            _matrices = new Matrix4x4[maxBullets];
            _colors = new Vector4[maxBullets];
            _materialPropertyBlock = new MaterialPropertyBlock();
            _previousPos = transform.position;
        }

        /// <summary>
        /// Unity function, happens when the object is enabled.
        /// Render the bullets.
        /// </summary>
        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += Render;
            bulletSettings.BulletDied += BulletDied;
        }

        /// <summary>
        /// Unity funciton, happens when the object is disabled.
        /// Stop rendering bullets.
        /// </summary>
        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= Render;
            bulletSettings.BulletDied -= BulletDied;
        }

        /// <summary>
        /// Fire a unity event when the bullet dies.
        /// E.g. used for spawning an explosion when the bullet has hit something. 
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="endOfLife"></param>
        private void BulletDied(BulletContainer bullet, bool endOfLife)
        {
            OnBulletDied?.Invoke(bullet, endOfLife);
        }
        
        /// <summary>
        /// Render the bullets. Called from OnEnable and OnDisable.
        /// Note: this function is called once for every camera.
        /// </summary>
        /// <param name="context">The current scriptable render context</param>
        /// <param name="cam">The camera being used to render</param>
        private void Render(ScriptableRenderContext context, Camera cam)
        {
            // create a new buffer - this will contain the render data
            var buffer = new CommandBuffer();
            #if UNITY_EDITOR
            buffer.name = "BulletFury";
            #endif
            
            // create a new material property block - this contains the different colours for every instance
            _materialPropertyBlock = new MaterialPropertyBlock();
            
            _hasBullets = false;
            currentActiveBullets = 0;
            // loop through and render the bullets
            for (int i = _bullets.Length - 1; i >= 0; i--)
            {
                _currentBullet = _bullets[i];
                // if the bullet is alive
                if (_currentBullet.Dead == 0)
                {
                    ++currentActiveBullets;
                    // we've got at least one active bullet, so we should render something
                    _hasBullets = true;

                    // set the colour for the bullet
                    _colors[i] = _currentBullet.Color;
                    
                    // if the "w" part of the rotation is 0, the Quaternion is invalid. Set it to the a rotation of 0,0,0
                    if (_currentBullet.Rotation.w == 0)
                        _currentBullet.Rotation = Quaternion.identity;

                    if (bulletSettings.Plane == BulletPlane.XZ)
                        _currentBullet.Rotation *= Quaternion.AngleAxis(90, Vector3.right);
                    
                    // set the matrix for the current bullet - translation, rotation, scale, in that order.
                    _matrices[i] = Matrix4x4.TRS(_currentBullet.Position,
                        _currentBullet.Rotation,
                        Vector3.one * _currentBullet.CurrentSize);
                }
                else // if the bullet is not alive, position the mesh at 0,0,0, with no rotation, and a scale of 0,0,0, so it isn't displayed.
                    _matrices[i] = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero);
            }

            // if we don't have any bullets, don't bother rendering anything
            if (!_hasBullets)
                return;

            // set the colours of the material property block
            _materialPropertyBlock.SetVectorArray(Color, _colors);
            
            // draw all the meshes
            // n.b. this is why we can only have 1023 bullets per spawner
            buffer.DrawMeshInstanced(bulletSettings.Mesh, 0, bulletSettings.Material, 0, _matrices, _bullets.Length, _materialPropertyBlock);
            
            // can't have two objects with the same priority, so keep increasing it til we find one that fits
            var priority = drawPriority;
            if (BulletFuryRenderPass.Buffers == null)
                BulletFuryRenderPass.Buffers = new SortedList<int, CommandBuffer>();
            while (BulletFuryRenderPass.Buffers.ContainsKey(priority))
                ++priority;
            
            // add the command buffer to the render pass
            BulletFuryRenderPass.Buffers.Add(priority, buffer);
            maxActiveBullets = Mathf.Max(maxActiveBullets, currentActiveBullets);
        }

        /// <summary>
        /// Unity function, called every frame
        /// Update the values of the bullets that can't be done in a Job, and run the Job
        /// </summary>
        private void Update()
        {
            var deltaTime = Time.deltaTime;
            // update the bullets according to the settings
            for (int i = 0; i < _bullets.Length; ++i)
                bulletSettings.SetValues(ref _bullets[i], deltaTime, transform.position, _previousPos);

            // create a new job
            _bulletJob = new BulletJob
            {
                DeltaTime = deltaTime,
                In = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                Out = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob)
            };

            // start the job
            _handle = _bulletJob.Schedule(_bullets.Length, 256);
            
            // make sure the job is finished this frame
            _handle.Complete();
            // grab the results
            _bulletJob.Out.CopyTo(_bullets);
            // dispose the native arrays 
            _bulletJob.In.Dispose();
            _bulletJob.Out.Dispose();
            
            // increment the current timer
            _currentTime += deltaTime;
            _previousPos = transform.position;
        }

        public void Spawn(Vector3 position, Vector3 forward)
        {
            // don't spawn a bullet if we haven't reached the correct fire rate
            if (_currentTime < spawnSettings.FireRate)
                return;
            // reset the current time
            _currentTime = 0;
            
            // start the spawning - it's a coroutine so we can do burst shots over time 
            StartCoroutine(SpawnIE(position, forward));
        }
        
        private IEnumerator SpawnIE(Vector3 position, Vector3 forward)
        {
            // keep a list of positions and rotations, so we can update the bullets all at once
            var positions = new List<Vector3>();
            var rotations = new List<Quaternion>();

            for (int burstNum = 0; burstNum < spawnSettings.BurstCount; ++burstNum)
            {
                // make sure the positions and rotations are clear before doing anything
                positions.Clear();
                rotations.Clear();
                
                // spawn the bullets
                spawnSettings.Spawn((point, dir) =>
                {
                    // for every point that the spawner gets
                    
                    // set up the rotation 
                    if (Plane == BulletPlane.XY)
                    {
                        rotations.Add(Quaternion.LookRotation(Vector3.forward, dir) *
                                      Quaternion.LookRotation(Vector3.forward, forward));
                        
                        positions.Add(position +Quaternion.LookRotation(Vector3.forward, forward) * point);
                    }
                    else
                    {
                        var rotation = dir == Vector2.zero ? Quaternion.identity : Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y));

                        // rotate it by the direction the object is facing
                        var y =rotation.eulerAngles.y;
                        rotation.SetLookRotation(Quaternion.Euler(0, y, 0) * forward);
                        // add it to the list
                        rotations.Add(rotation);
                        // grab the position, rotated by the direction the object is facing
                        var pos = position + (Quaternion.LookRotation(forward) * new Vector3(point.x, 0, point.y));
                        // at the position to the list
                        positions.Add(pos);
                    }
                });
                
                // for every bullet we found
                for (int i = 0; i < positions.Count; i++)
                {
                    // create a new container that isn't dead, at the position and rotation we found with the spawner
                    var newContainer = new BulletContainer
                    {
                        Dead = 0,
                        Position = positions[i],
                        Rotation = rotations[i]
                    };
                    
                    // initialise the bullet
                    bulletSettings.Init(ref newContainer);

                    // find a bullet that isn't alive and replace it with this one
                    for (int j = 0; j < _bullets.Length; ++j)
                    {
                        if (_bullets[j].Dead == 0) continue;
                        _bullets[j] = newContainer;
                        break;
                    }
                    OnBulletSpawned?.Invoke(newContainer);
                }
                
                // wait a little bit before doing the next burst
                yield return new WaitForSeconds(spawnSettings.BurstDelay);
            }
        }
        
        /// <summary>
        /// Activate any waiting bullets.
        /// Use this when you want to do bullet tracing.
        /// </summary>
        public void ActivateWaitingBullets()
        {
            for (int i = 0; i < _bullets.Length; i++)
                _bullets[i].Waiting = 0;
        }
        
        /// <summary>
        /// Get all the bullets in this bullet manager
        /// Used for checking collisions
        /// </summary>
        /// <returns>All the bullets in this bullet manager</returns>
        public BulletContainer[] GetBullets()
        {
            return _bullets;
        }

        /// <summary>
        /// Fire the "Bullet Hit" event and mark the bullet as dead 
        /// </summary>
        public void HitBullet(int idx)
        {
            bulletSettings.Die(ref _bullets[idx], false);
        }

        public void WaitBullet(int idx)
        {
            var b = _bullets[idx];
            b.Waiting = 1;
            _bullets[idx] = b;
        }
    }
}
