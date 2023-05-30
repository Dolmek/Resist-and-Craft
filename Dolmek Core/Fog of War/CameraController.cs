using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public RenderTexture renderTexture;

    private Camera camera;
    private Material material;

    private void Start()
    {
        camera = GetComponent<Camera>();
        material = new Material(Shader.Find("Hidden/DepthTexture"));
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Render a depth map of the scene
        RenderTexture depthTexture = RenderTexture.GetTemporary(source.width, source.height, 24, RenderTextureFormat.Depth);
        camera.targetTexture = depthTexture;
        camera.Render();

        // Set up the material for rendering the line of sight view
        material.SetTexture("_DepthTexture", depthTexture);
        material.SetVector("_PlayerPosition", player.transform.position);

        // Render the line of sight view onto the render texture
        Graphics.Blit(renderTexture, material);
        camera.targetTexture = null;

        // Copy the result to the destination texture
        Graphics.Blit(renderTexture, destination);
    }
}
