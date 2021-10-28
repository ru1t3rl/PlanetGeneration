// https://www.alanzucconi.com/2015/07/08/screen-shaders-and-postprocessing-effects-in-unity3d/
using UnityEngine;
using Ru1t3rl.Planets.Atmos;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostProcessingEffect : MonoBehaviour
{
    public System.Action<PostProcessingEffect> OnRender;
    public Atmosphere atmos;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (atmos == null)
            return;

        OnRender?.Invoke(this);

        if (Camera.main != null)
            Camera.main.depthTextureMode = DepthTextureMode.DepthNormals | DepthTextureMode.Depth;

        Graphics.Blit(source, destination);
        //apply our material to the ouput
        Graphics.Blit(source, destination, atmos.material);
    }
}
