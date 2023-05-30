using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenFieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float viewRadius;
    [SerializeField][Range(0, 360)] private float viewAngle;
    [SerializeField] private MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    // Start is called before the first frame update
    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    // Update is called once per frame
    void Update()
    {
        DrawFieldOfView();
    }

    private void DrawFieldOfView()
    {
        int rayCount = Mathf.RoundToInt(viewAngle / 90.0f) * 50 + 50;
        float angle = transform.eulerAngles.y - viewAngle / 2;
        float angleIncrease = viewAngle / rayCount;

        Vector3 origin = transform.position;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;
        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit hit;

            float currentAngle = angle + i * angleIncrease;
            Vector3 direction = new Vector3(Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngle * Mathf.Deg2Rad));
            if (Physics.Raycast(origin, direction, out hit, viewRadius, obstacleLayer))
            {
                vertex = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                vertex = transform.InverseTransformPoint(origin + direction * viewRadius);
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.uv = uv;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
}
