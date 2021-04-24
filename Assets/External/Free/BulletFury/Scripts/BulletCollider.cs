using System.Collections.Generic;
using BulletFury.Data;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace BulletFury
{
    public enum ColliderShape {Sphere, AABB}
    /// <summary>
    /// Tell an object to collide with bullets
    /// </summary>
    public class BulletCollider : MonoBehaviour
    {
        // the set of bullets this collider should collide with
        [SerializeField] private List<BulletManager> hitByBullets = new List<BulletManager>();
        public List<BulletManager> HitByBullets => hitByBullets;


        [SerializeField] private ColliderShape shape;
        
        // the radius of the sphere that describes this collider
        [SerializeField] private float radius = .5f;

        // the bounding box that describes this collider
        [SerializeField] private Vector3 size;
        
        // the offset of the collider
        [SerializeField] private Vector3 offset;

        // Unity Event that fires when a bullet collides with this collider, can be set in the inspector like a button 
        // ReSharper disable once InconsistentNaming
        [SerializeField] private BulletCollisionEvent OnCollide;

        // cached job and job handle
        private BulletDistanceJob _bulletJobCircle;
        private BulletAABBJob _bulletJobAABB;
        private JobHandle _handle;

        // an array of bullets
        private BulletContainer[] _bullets;
        
        /// <summary>
        /// Unity function, called every frame
        /// Run the job, tell the bullet manager that the bullet has been collided with
        /// </summary>
        void Update()
        {
            foreach (var manager in hitByBullets)
            {
                if (manager == null || !manager.enabled || !manager.gameObject.activeSelf || manager.GetBullets() == null)
                    continue;
                // grab the bullets in the bullet manager
                _bullets = manager.GetBullets();

                if (shape == ColliderShape.Sphere)
                {
                    // create the job
                    _bulletJobCircle = new BulletDistanceJob
                    {
                        In = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        Out = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        Distance = radius,
                        Position = transform.position + offset
                    };

                    // run the job
                    _handle = _bulletJobCircle.Schedule(_bullets.Length, 256);

                    // make sure the job finished this frame
                    _handle.Complete();
                    // grab the results
                    _bulletJobCircle.Out.CopyTo(_bullets);
                    // dispose the native arrays
                    _bulletJobCircle.In.Dispose();
                    _bulletJobCircle.Out.Dispose();
                }
                else
                {
                    var bounds = new Bounds(transform.position + offset, size);
                    // create the job
                    _bulletJobAABB = new BulletAABBJob
                    {
                        In = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        Out = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        BoxMin = bounds.min,
                        BoxMax = bounds.max
                    };

                    // run the job
                    _handle = _bulletJobAABB.Schedule(_bullets.Length, 256);

                    // make sure the job finished this frame
                    _handle.Complete();
                    // grab the results
                    _bulletJobAABB.Out.CopyTo(_bullets);
                    // dispose the native arrays
                    _bulletJobAABB.In.Dispose();
                    _bulletJobAABB.Out.Dispose();

                }
                
                // loop through the bullets, if there was a collision this frame - tell the bullet manager and anything else that needs to know
                for (int i = 0; i < _bullets.Length; i++)
                {
                    if (_bullets[i].Dead == 0 && _bullets[i].Collided == 1)
                    {
                        manager.HitBullet(i);
                        OnCollide?.Invoke(_bullets[i], this);
                    }
                }
            }
        }

        public void AddManagerToBullets(BulletManager manager)
        {
            hitByBullets.Add(manager);
        }

        public void RemoveManagerFromBullets(BulletManager manager)
        {
            hitByBullets.Remove(manager);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            if (shape == ColliderShape.Sphere)
                Gizmos.DrawWireSphere(transform.position + offset, radius);
            else
                Gizmos.DrawWireCube(transform.position + offset, size);
        }
    }
}
