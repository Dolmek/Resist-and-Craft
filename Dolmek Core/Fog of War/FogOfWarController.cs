using UnityEngine;

public class FogOfWarController : MonoBehaviour
{
    public Transform characterTransform; // assign this to the transform of your character
    public Camera depthCamera;
    private Renderer objectRenderer;
    private Material fogOfWarMaterial;

    private void Start()
    {
        // Get the renderer component of the object
        objectRenderer = GetComponent<Renderer>();

        // Get the material with the FogOfWar_Circle shader
        fogOfWarMaterial = objectRenderer.material;

        // Create a temporary render texture for the depth camera to render to
        var depthTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
        depthTexture.Create();

        // Set the depth camera to render to the temporary depth texture
        depthCamera.targetTexture = depthTexture;

        // Enable depth texture mode for the depth camera
        depthCamera.depthTextureMode = DepthTextureMode.Depth;

        // Set the _MainTex and _DepthTex properties of the fog of war material
        fogOfWarMaterial.SetTexture("_MainTex", depthTexture);
        fogOfWarMaterial.SetTexture("_DepthTex", depthTexture);

        // Set the _CharacterPosition parameter of the fog of war material
        fogOfWarMaterial.SetVector("_CharacterPosition", characterTransform.position);
    }

    private void Update()
    {
        // check if the object has a renderer component and a material with the fog of war shader
        if (objectRenderer != null && objectRenderer.material.shader.name == "Custom/FogOfWar_Circle")
        {
            // set the _CharacterPosition parameter of the material to the character's position
            objectRenderer.material.SetVector("_CharacterPosition", characterTransform.position);
        }

        // Update the _CharacterPosition parameter of the fog of war material every frame
        fogOfWarMaterial.SetVector("_CharacterPosition", characterTransform.position);

    }
}
