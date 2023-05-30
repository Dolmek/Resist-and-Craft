using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFOV : MonoBehaviour
{
    public Camera mainCamera;
    private List<Renderer> visibleRenderers = new List<Renderer>();
    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Find all the visible renderers
        FindVisibleRenderers();

        // Save the player's position
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player has moved, update the list of visible renderers
        if (lastPosition != transform.position)
        {
            FindVisibleRenderers();
            lastPosition = transform.position;
        }

        // Hide all the renderers that are not visible
        foreach (var renderer in visibleRenderers)
        {
            renderer.enabled = true;
        }

        // Clear the list of visible renderers
        visibleRenderers.Clear();
    }

    private void FindVisibleRenderers()
    {
        // Get the camera's view frustum
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        // Find all the visible renderers
        foreach (var renderer in FindObjectsOfType<Renderer>())
        {
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds))
            {
                visibleRenderers.Add(renderer);
            }
            else
            {
                renderer.enabled = false;
            }
        }
    }
}
