using System;
using System.Runtime.Serialization.Json;
using Unity.Mathematics;
using UnityEngine;

namespace BulletFury.Data
{
    public enum BulletPlane {XY, XZ}
    
    /// <summary>
    /// Container for bullet settings
    /// </summary>
    [CreateAssetMenu(menuName = "BulletFury/Bullet Settings")]
    public class BulletSettings : ScriptableObject
    {
        // an event to fire when the bullet died
        public event Action<BulletContainer, bool> BulletDied;

        #region Fields
        // the mesh to use for the bullet
        [SerializeField] private Mesh mesh = null;
        // public accessor for the mesh - doing it this way means we can never set the mesh in code
        public Mesh Mesh => mesh;
        
        // the material to use for the bullet
        [SerializeField] private Material material = null;
        // public accessor for the material - doing it this way means we can never set the material in code
        public Material Material => material;

        [SerializeField] private BulletPlane plane = BulletPlane.XY;
        public BulletPlane Plane => plane;
        
        // wait until the bullet manager says go - used for bullet tracing
        [SerializeField]
        private bool waitToStart = false;
        
        // the amount, in seconds, of the bullet's animation that should play before the bullet stops
        // using this means the bullet will spawn with an animation, rather than just not doing anything til the manager says go 
        [SerializeField] private float timeToPlayWhileWaiting;
        
        // the amount of time, in seconds, that the bullet should stay alive
        [SerializeField] private float lifetime = 1;
        // how quickly the bullet should move, in unity units per second
        [SerializeField] private float speed = 10;
        [SerializeField] private bool inheritSpeedFromTransform = false;

        // how big, in unity units, the bullet should be
        // this affects scale - so a value of 1 correlates to a scale of (1,1,1)
        [SerializeField] private float size = 1;
        
        // the amount the bullet should rotate, in degrees per second
        [SerializeField] private float angularVelocity = 0;
        
        // the start colour of the bullet
        [SerializeField, ColorUsage(true, true)] private Color startColor = Color.white;
        
        // colour the bullet over time (multiplied with the start colour)
        [SerializeField] private bool useColorOverTime = false;
        [SerializeField, GradientUsage(true)] private Gradient colorOverTime = null;

        // rotate the bullet over time
        [SerializeField] private bool useRotationOverTime = false;
        [SerializeField] private AnimationCurve rotationOverTime = null;

        // scale the bullet over time
        [SerializeField] private bool useSizeOverTime = false;
        [SerializeField] private AnimationCurve sizeOverTime = null;
        
        // change velocity over time
        [SerializeField] private bool useVelocityOverTime = false;
        // local: use the bullet's forward/up/right axes rather than world axes
        // world: use the world forward/up/right axes rather than the object's local axes
        [SerializeField] private ForceSpace velocitySpace = ForceSpace.Local;
        // scale with speed or use the absolute value of the animation curve
        [SerializeField] private bool scaleWithSpeed = false;
        [SerializeField] private AnimationCurve velocityOverTimeX = AnimationCurve.Linear(0,0,1, 0);
        [SerializeField] private AnimationCurve velocityOverTimeY = AnimationCurve.Linear(0,0,1, 0);
        [SerializeField] private AnimationCurve velocityOverTimeZ = AnimationCurve.Linear(0,0,1, 0);

        public bool TrackObject
        {
            get => trackObject;
            set => trackObject = value;
        }

        [SerializeField] private bool trackObject;
        [SerializeField] private string trackedObjectTag;
        [SerializeField] private float turnSpeed;

        // add force over time. N.B. this is ADDITIVE, so it'll get fast quickly
        [SerializeField] private bool useForceOverTime = false;
        // local: use the bullet's forward/up/right axes rather than world axes
        // world: use the world forward/up/right axes rather than the object's local axes
        [SerializeField] private ForceSpace forceSpace = ForceSpace.Local;
        [SerializeField] private AnimationCurve forceOverTimeX = AnimationCurve.Linear(0,0,1, 0);
        [SerializeField] private AnimationCurve forceOverTimeY = AnimationCurve.Linear(0,0,1, 0);
        [SerializeField] private AnimationCurve forceOverTimeZ = AnimationCurve.Linear(0,0,1, 0);
#if UNITY_EDITOR
        [SerializeField] private bool isExpanded;
#endif
        #endregion

        private Transform _trackedObject = null;
        
        /// <summary>
        /// Initialise the bullet
        /// </summary>
        /// <param name="position">the starting value, as a percentage of the number of bullets spawned this cycle</param>
        /// <param name="bullet">a reference to the current bullet</param>
        public void Init(ref BulletContainer bullet)
        {
            bullet.Lifetime = lifetime;
            bullet.AngularVelocity = angularVelocity;
            bullet.Speed = speed;
            bullet.CurrentSpeed = speed;
            bullet.StartSize = size;
            bullet.CurrentSize = bullet.StartSize;
            bullet.StartColor = startColor;
            bullet.Color = bullet.StartColor;
            bullet.RotationChangedThisFrame = 0;
            bullet.Waiting = waitToStart ? (byte) 1 : (byte) 0;
            bullet.TimeToWait = timeToPlayWhileWaiting;
            bullet.Forward = bullet.Rotation * Vector3.forward;
            bullet.Right = bullet.Rotation * Vector3.right;
            bullet.Up = bullet.Rotation * Vector3.up;
            if (trackObject)
                _trackedObject = GameObject.FindWithTag(trackedObjectTag).transform;
        }

        
        /// <summary>
        /// Set the values of the bullet
        /// </summary>
        /// <param name="bullet">the current bullet</param>
        /// <param name="deltaTime">cached Time.deltaTime</param>
        public void SetValues(ref BulletContainer bullet, float deltaTime, Vector3 position, Vector3 previousPosition)
        {
            // if the bullet is dead or waiting, don't do anything
            if (bullet.Dead == 1 || bullet.Waiting == 1 && bullet.CurrentLifeSeconds > bullet.TimeToWait)
                return;


            // change the colour over time
            if (useColorOverTime)
                bullet.Color = bullet.StartColor * colorOverTime.Evaluate(bullet.CurrentLifePercent);

            // if we've got some extra angular velocity, rotate the bullet over time and mark the rotation as changed
            if (useRotationOverTime)
            {
                // if we've got some angular velocity, rotate the bullet over time and mark the rotation as changed
                if (Mathf.Abs(bullet.AngularVelocity) > 0)
                {
                    
                    if (plane == BulletPlane.XY)
                        bullet.Rotation *= Quaternion.Euler(0, 0, bullet.AngularVelocity * deltaTime);
                    else
                        bullet.Rotation *= Quaternion.Euler(0, bullet.AngularVelocity * deltaTime, 0);
                }
                    
                if (plane == BulletPlane.XY)
                    bullet.Rotation *= Quaternion.Euler(0, 0, rotationOverTime.Evaluate(bullet.CurrentLifePercent));
                else
                    bullet.Rotation *= Quaternion.Euler(0, rotationOverTime.Evaluate(bullet.CurrentLifePercent), 0);
                
                bullet.RotationChangedThisFrame = 1;
            }

            // if the bullet's rotation has changed this frame, calculate the forward/right/up axes for the bullet
            if (bullet.RotationChangedThisFrame == 1)
            {
                bullet.Forward = bullet.Rotation * Vector3.forward;
                bullet.Right = bullet.Rotation * Vector3.right;
                bullet.Up = bullet.Rotation * Vector3.up;
            }

            if (trackObject && _trackedObject != null)
            {
                var target = _trackedObject.position - (Vector3)bullet.Position;
                if (plane == BulletPlane.XY)
                    bullet.Up = Vector3.RotateTowards(bullet.Up, target, turnSpeed * deltaTime, 0.0f);
                else 
                    bullet.Forward = Vector3.RotateTowards(bullet.Forward, target, turnSpeed * deltaTime, 0.0f);

                bullet.Rotation = Quaternion.LookRotation(bullet.Forward, bullet.Up);
            }

            // change the size over time
            if (useSizeOverTime)
                bullet.CurrentSize = bullet.StartSize * sizeOverTime.Evaluate(bullet.CurrentLifePercent);
 
            // move the bullet forwards
            bullet.Velocity = bullet.Rotation * (Plane == BulletPlane.XY ? Vector3.up : Vector3.forward);
            
            if (inheritSpeedFromTransform)
                bullet.CurrentSpeed = bullet.Speed + Vector3.Project(previousPosition - position, bullet.Forward).magnitude / Time.deltaTime;

            // change the velocity over time
            if (useVelocityOverTime)
            {
                // if the force space is local, use the bullet's local axes
                if (velocitySpace == ForceSpace.Local)
                {
                    bullet.Velocity = bullet.Right * velocityOverTimeX.Evaluate(bullet.CurrentLifePercent) +
                                      bullet.Up * velocityOverTimeY.Evaluate(bullet.CurrentLifePercent) +
                                      bullet.Forward * velocityOverTimeZ.Evaluate(bullet.CurrentLifePercent);
                }
                else // if the force space is world, use the world axes
                {
                    bullet.Velocity = new float3(
                        velocityOverTimeX.Evaluate(bullet.CurrentLifePercent),
                        velocityOverTimeY.Evaluate(bullet.CurrentLifePercent),
                        velocityOverTimeZ.Evaluate(bullet.CurrentLifePercent)
                    );
                }

                // if we're scaling with the speed value, do that
                if (scaleWithSpeed)
                    bullet.Velocity *= bullet.CurrentSpeed;
            }
            else // if we're not using velocity over time, we definitely want to scale with speed;
                bullet.Velocity *= bullet.CurrentSpeed;

            // add force over time
            if (useForceOverTime)
            {
                // if the force space is local, use the bullet's local axes 
                if (forceSpace == ForceSpace.Local)
                {
                    bullet.Force += bullet.Right * forceOverTimeX.Evaluate(bullet.CurrentLifePercent) +
                                   bullet.Up * forceOverTimeY.Evaluate(bullet.CurrentLifePercent) +
                                   bullet.Forward * forceOverTimeZ.Evaluate(bullet.CurrentLifePercent);
                }
                else // if the force space is world, use the world axes
                {
                    bullet.Force += new float3(
                        forceOverTimeX.Evaluate(bullet.CurrentLifePercent),
                        forceOverTimeY.Evaluate(bullet.CurrentLifePercent),
                        forceOverTimeZ.Evaluate(bullet.CurrentLifePercent)
                    );
                }
            }
            
            bullet.RotationChangedThisFrame = 0;
        }

        /// <summary>
        /// Kill the bullet, invoke the event
        /// </summary>
        /// <param name="bullet"> a reference to the current bullet </param>
        /// <param name="endOfLife"> whether or not the bullet has reached the end of its lifetime, or it has been destroyed early </param>
        public void Die(ref BulletContainer bullet, bool endOfLife)
        {
            bullet.Dead = 1;
            BulletDied?.Invoke(bullet, endOfLife);
        }
    }
}