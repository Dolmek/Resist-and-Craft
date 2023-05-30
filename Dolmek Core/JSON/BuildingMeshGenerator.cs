using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using static Pinwheel.Vista.Graph.Serializer;

public class BuildingMeshGenerator : EditorWindow
{
    private string jsonFilePath = "Assets/Dolmek Core/JSON/cauldronhope.json";
    private List<List<Vector2>> buildingCoordinates = new List<List<Vector2>>();

    [MenuItem("Window/Building Mesh Generator")]
    public static void ShowWindow()
    {
        GetWindow<BuildingMeshGenerator>("Building Mesh Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Building Mesh Generator", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Building Mesh"))
        {
            GenerateBuildingMesh();
        }
    }

    private void GenerateBuildingMesh()
    {
        string jsonContent = File.ReadAllText(jsonFilePath);
        JSONObject jsonObject = new JSONObject(jsonContent);

        // Find the "buildings" object in the JSON
        JSONObject buildingsObject = jsonObject.GetField("buildings");

        // Iterate over the buildings
        foreach (JSONObject building in buildingsObject.list)
        {
            string type = building.GetField("type").str;

            // Check if the building has a "MultiPolygon" type
            if (type == "MultiPolygon")
            {
                // Get the coordinates defining the building's outline
                JSONObject coordinates = building.GetField("coordinates");

                // Iterate over the polygons
                foreach (JSONObject polygon in coordinates.list)
                {
                    List<Vector2> buildingContour = new List<Vector2>();

                    // Iterate over the vertices of the polygon
                    foreach (JSONObject vertex in polygon.list)
                    {
                        float x = vertex.GetField("x").n;
                        float z = vertex.GetField("z").n;
                        Vector2 vertexPosition = new Vector2(x, z);
                        buildingContour.Add(vertexPosition);
                    }

                    // Add the building's contour to the list of coordinates
                    buildingCoordinates.Add(buildingContour);
                }
            }
        }

        // Generate the building meshes
        foreach (List<Vector2> contour in buildingCoordinates)
        {
            GenerateMeshFromContour(contour);
        }

        // Clear the list of building coordinates
        buildingCoordinates.Clear();

        Debug.Log("Building meshes generated successfully!");
    }

    private void GenerateMeshFromContour(List<Vector2> contour)
    {
        GameObject buildingObject = new GameObject("Building");
        MeshFilter meshFilter = buildingObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = buildingObject.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();

        // Convert the contour vertices to 3D by setting y to 0
        Vector3[] vertices = new Vector3[contour.Count];
        for (int i = 0; i < contour.Count; i++)
        {
            vertices[i] = new Vector3(contour[i].x, 0f, contour[i].y);
        }

        // Generate the triangles for the mesh
        int[] triangles = Triangulator.Triangulate(contour);

        // Assign the vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Calculate the mesh normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Assign the mesh to the mesh filter
        meshFilter.sharedMesh = mesh;
    }
}
