using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FOVController))]
public class FOVRenderer : MonoBehaviour
{
    private Vector2[] uvs;
    private List<RaycastHit> hits = new List<RaycastHit>();
    private FOVController fov;
    private Mesh viewMesh;
    private MeshFilter viewMeshFilter;
    private Dictionary<int, Vector3> viewPoints;
    private Vector3[] vertices;
    private int[] triangles;
    public Material cutoutMaterial;
    public float maskCutawayDistance = .1f;
    public float meshResolution = 1;
    public int edgeResolveIterations = 4;
    public float edgeDstThreshold = 0.5f;
    private int trianglesIndex = 1;


    private Vector2[] uvDirections = new Vector2[] {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1)
    };

    private void Start()
    {
        fov = GetComponent<FOVController>();
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        viewPoints = new Dictionary<int, Vector3>();
        vertices = new Vector3[Mathf.RoundToInt(fov.viewAngle * meshResolution) + 1];
        triangles = new int[(vertices.Length - 1) * 3];
        for (int i = 0; i < vertices.Length; i++)
        {
            float angle = fov.transform.eulerAngles.y - fov.viewAngle / 2 + fov.viewAngle * i / (vertices.Length - 1);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            viewPoints.Add(i, dir);
            RaycastHit hit;
            if (Physics.Raycast(fov.transform.position, dir, out hit, fov.viewRadius))
            {
                hits.Add(hit);
                vertices[i] = hit.point;
            }
            else
            {
                vertices[i] = fov.transform.position + dir * fov.viewRadius;
            }
            if (i > 0)
            {
                triangles[trianglesIndex + 0] = 0;
                triangles[trianglesIndex + 1] = trianglesIndex - 1;
                triangles[trianglesIndex + 2] = trianglesIndex;
                trianglesIndex += 3;
            }
        }
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
        CalculateUV();
        UpdateMeshRenderer();
    }

    private void CalculateUV()
    {
        int length = vertices.Length;
        uvs = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            float uvRatio = (float)i / (length - 1);
            Vector2 uv = new Vector2(uvRatio, 0);
            float distance = Vector3.Distance(fov.transform.position, vertices[i]);
            for (int j = 0; j < uvDirections.Length; j++)
            {
                RaycastHit hit;
                Vector3 dir = (fov.transform.TransformDirection(uvDirections[j]) + Vector3.forward) / 2f;
                if (Physics.Raycast(fov.transform.position, dir, out hit, fov.viewRadius))
                {
                    if (hit.distance <= distance)
                    {
                        uv = new Vector2(uvRatio, Vector3.Distance(fov.transform.position, hit.point) / maskCutawayDistance);
                        break;
                    }
                }
            }
            uvs[i] = uv;
        }
        viewMesh.uv = uvs;
    }

    private void UpdateMeshRenderer()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = cutoutMaterial;
            meshRenderer.material.SetFloat("_Threshold", 0.5f);
            meshRenderer.material.SetFloat("_Radius", fov.viewRadius);
        }
    }
}