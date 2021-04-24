using UnityEngine.Rendering.Universal;

namespace BulletFury.Rendering
{
    public class BulletFuryRenderFeature : ScriptableRendererFeature
    {
        public override void Create()
        {
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(new BulletFuryRenderPass());
        }
    }
}