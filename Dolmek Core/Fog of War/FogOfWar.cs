using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{

    public Transform player;
    public LayerMask fogLayer;
    public float radius = 5f;
    public float speed = 5f;

    private float playerYOffset;
    private Texture2D fogTexture;
    private Color[] fogColorArray;

    void Start()
    {
        playerYOffset = player.position.y;
        fogTexture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        fogColorArray = new Color[512 * 512];
        GetComponent<Renderer>().material.mainTexture = fogTexture;
    }

    void Update()
    {
        float fogX = Mathf.RoundToInt((player.position.x - transform.position.x) / speed) + (fogTexture.width / 2);
        float fogY = Mathf.RoundToInt((player.position.z - transform.position.z) / speed) + (fogTexture.height / 2);

        for (int y = 0; y < fogTexture.height; y++)
        {
            for (int x = 0; x < fogTexture.width; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(fogX, fogY));
                if (dist < radius)
                {
                    if (Physics.Linecast(new Vector3(player.position.x, playerYOffset, player.position.z), new Vector3(x * speed + transform.position.x, playerYOffset, y * speed + transform.position.z), fogLayer))
                    {
                        fogColorArray[y * 512 + x] = new Color(0, 0, 0, 1);
                    }
                    else
                    {
                        fogColorArray[y * 512 + x] = new Color(1, 1, 1, 1);
                    }
                }
                else
                {
                    fogColorArray[y * 512 + x] = new Color(0, 0, 0, 1);
                }
            }
        }

        fogTexture.SetPixels(fogColorArray);
        fogTexture.Apply();
    }
}

/*
 For the _MainTex, you can use any texture that you want to apply the fog effect to. This can be a diffuse texture, a normal map, or any other texture that you want to be affected by the fog.

For the _DepthTex, you will need to render a depth texture for the scene. To do this, you can add a new camera to your scene and set its "Depth Texture Mode" to "Depth". This will cause the camera to render a depth texture that encodes the distance from the camera to each pixel in the scene.

Once you have the depth texture, you can assign it to the _DepthTex property in your fog of war shader. To do this, you can create a new Render Texture asset in Unity and set its "Render Texture Format" to "Depth". Then, you can assign this render texture to the "Target Texture" property of your depth camera, and use it as the _DepthTex property in your fog of war shader.
*/
