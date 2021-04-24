using BulletFury.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BulletFury
{
    /// <summary>
    /// A C# job that checks for collisions between bullet colliders and bullets
    /// </summary>
    [BurstCompile]
    public struct BulletDistanceJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BulletContainer> In;
        [WriteOnly] public NativeArray<BulletContainer> Out;
        [ReadOnly] public float Distance;
        [ReadOnly] public float3 Position;
        
        public void Execute(int index)
        {
            var container = In[index];
            if (container.Dead == 1)
            {
                Out[index] = container;
                return;
            }

            var vec = Position - container.Position;
            var dist = (vec.x * vec.x) + (vec.y * vec.y) + (vec.z * vec.z);

            container.Collided = dist <= (Distance * Distance) + (container.CurrentSize * container.CurrentSize) ? (byte) 1 : (byte) 0;
            Out[index] = container;
        }
    }

    [BurstCompile]
    public struct BulletAABBJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BulletContainer> In;
        [WriteOnly] public NativeArray<BulletContainer> Out;
        [ReadOnly] public float3 BoxMin;
        [ReadOnly] public float3 BoxMax;

        public void Execute(int index)
        {
            var container = In[index];
            if (container.Dead == 1)
            {
                Out[index] = container;
                return;
            }

            var x = math.max(BoxMin.x, math.min(container.Position.x, BoxMax.x));
            var y = math.max(BoxMin.y, math.min(container.Position.y, BoxMax.y));
            var z = math.max(BoxMin.z, math.min(container.Position.z, BoxMax.z));

            var sqrDist = (x - container.Position.x) * (x - container.Position.x) +
                          (y - container.Position.y) * (y - container.Position.y) +
                          (z - container.Position.z) * (z - container.Position.z);
            
            container.Collided = sqrDist <= (container.CurrentSize * container.CurrentSize) ? (byte) 1 : (byte) 0;
            Out[index] = container;
        }
    }
}