using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BulletFury.Data
{
    /// <summary>
    /// Container for the bullet spawning settings
    /// </summary>
    [CreateAssetMenu(menuName = "BulletFury/Spawn Settings")]
    public class SpawnSettings : ScriptableObject
    {
        // how often the bullet should fire, in seconds
        [SerializeField] private float fireRate = 0.1f;
        // public accessor for the fire rate, so we can't change it in code
        public float FireRate => fireRate;

        // the number of bursts each shot should fire
        [SerializeField] private int burstCount = 1;
        // public accessor
        public int BurstCount => burstCount;
        
        // the delay between burst shots
        [SerializeField] private float burstDelay = 0.1f;
        // public accessor
        public float BurstDelay => burstDelay;
        
        // the method by which we decide what direction to spawn the bullets in
        [SerializeField] private SpawnDir spawnDir;
        
        // the amount of sides the spawn shape should have 
        [SerializeField] private int numSides;
        // the number of bullets per side the shape should have
        [SerializeField] private int numPerSide;
        // the radius of the shape
        [SerializeField] private float radius;
        
        // the arc of the shape
        [SerializeField, Range(0, 360)] private float arc;
        #if UNITY_EDITOR
        [SerializeField] private bool isExpanded;
        #endif
        /// <summary>
        /// Get a point based on the spawning settings
        /// </summary>
        /// <param name="onGetPoint"> a function to run for every point that has been found </param>
        public void Spawn(Action<Vector2, Vector2> onGetPoint)
        {
            // initialise the array
            var points = new Vector2[numSides];
            
            // take a first pass and add some points to every side
            for (int i = 0; i < numSides; i++)
            {
                var angle = Mathf.Deg2Rad * (2 * arc * i - arc * numSides + arc + 180 * numSides) / (2 * numSides);

                points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                if (numPerSide == 1)
                    onGetPoint?.Invoke(points[i] * radius, points[i].normalized);
            }

            if (numPerSide == 1)
                return;

            // for every side
            for (int i = 0; i < numSides; ++i)
            {
                // get the next position
                var next = i + 1;
                if (next == numSides)
                    next = 0;

                // the normal of the current side
                var direction = Vector2.Lerp(points[i], points[next], 0.5f).normalized;

                // for every bullet we should spawn on this side of the shape
                for (int j = 0; j < numPerSide; ++j)
                {
                    // position the current point a percentage of the way between each end of the side
                    var t = j / (float) numPerSide;
                    t += (1f / numPerSide) / 2f;
                    var point = Vector2.Lerp(points[i], points[next], t);
                    point *= radius;

                    // set the direction based on the spawnDir enum
                    var dir = spawnDir switch
                    {
                        // use the overall direction for the side
                        SpawnDir.Directional => direction,
                        // point it away from the centre
                        SpawnDir.Spherised => point.normalized,
                        // point it in a random direction
                        SpawnDir.Randomised => Random.insideUnitCircle.normalized,
                        // if there's somehow another value, point it straight up
                        _ => Vector2.up
                    };

                    // tell function what the point and direction is
                    onGetPoint?.Invoke(point, dir);
                }
            }
        }
    }
}