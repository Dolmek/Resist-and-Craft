using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildingMeshGenerator : EditorWindow
{
    // The JSON file that contains the data for the meshes.
    public string jsonFilePath;

    [MenuItem("Window/Building Mesh Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BuildingMeshGenerator));
    }

    void OnGUI()
    {
        GUILayout.Label("Create Meshes From JSON");
        GUILayout.BeginHorizontal();
        GUILayout.Label("JSON File:");
        jsonFilePath = GUILayout.TextField(jsonFilePath);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Create"))
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);

                // Parse the JSON data into a custom data structure.
                BuildingData buildingData = JsonUtility.FromJson<BuildingData>(jsonData);

                // Process the building data and create meshes.
                CreateMeshesFromBuildingData(buildingData);
            }
            else
            {
                Debug.LogError("JSON file does not exist!");
            }
        }
    }

    private void CreateMeshesFromBuildingData(BuildingData buildingData)
    {
        foreach (BuildingGeometry geometry in buildingData.features)
        {
            if (geometry.type == "Polygon" || geometry.type == "MultiPolygon")
            {
                // Create a new GameObject for the mesh.
                GameObject gameObject = new GameObject(geometry.id);

                // Create a new MeshFilter for the GameObject.
                MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

                // Create a new MeshRenderer for the GameObject.
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

                // Set the materials for the MeshRenderer.
                meshRenderer.sharedMaterials = GetMaterialsForGeometry(geometry);

                // Create a new Mesh from the coordinates.
                Mesh mesh = CreateMeshFromCoordinates(geometry.coordinates);

                // Set the Mesh for the MeshFilter.
                meshFilter.sharedMesh = mesh;
            }
        }
    }

    private Material[] GetMaterialsForGeometry(BuildingGeometry geometry)
    {
        // Determine the materials based on the geometry type, if needed.
        // You can customize this method to assign different materials for different geometries.

        // Example: Assign a red material for polygons and a green material for multipolygons.
        if (geometry.type == "Polygon")
        {
            return new Material[] { new Material(Shader.Find("Diffuse")) { color = Color.red } };
        }
        else if (geometry.type == "MultiPolygon")
        {
            return new Material[] { new Material(Shader.Find("Diffuse")) { color = Color.green } };
        }

        // If no specific materials are assigned, return an empty array.
        return new Material[0];
    }

    private Mesh CreateMeshFromCoordinates(Vector2[][] coordinates)
    {
        Mesh mesh = new Mesh();

        // Create a list to hold all the vertices.
        List<Vector3> vertices = new List<Vector3>();

        // Create a list to hold all the triangles.
        List<int> triangles = new List<int>();

        // Loop through the coordinate arrays and add vertices and triangles.
        foreach (Vector2[] coordinateArray in coordinates)
        {
            int startIndex = vertices.Count;

            // Add vertices for each coordinate in the array.
            foreach (Vector2 coordinate in coordinateArray)
            {
                vertices.Add(new Vector3(coordinate.x, 0f, coordinate.y));
            }

            // Create triangles based on the vertices.
            for (int i = 1; i < coordinateArray.Length - 1; i++)
            {
                triangles.Add(startIndex);
                triangles.Add(startIndex + i);
                triangles.Add(startIndex + i + 1);
            }
        }

        // Assign vertices and triangles to the mesh.
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        // Recalculate normals and bounds.
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}

[System.Serializable]
public class BuildingData
{
    public BuildingFeature[] features;
}

[System.Serializable]
public class BuildingFeature
{
    public string type;
    public string id;
    public Vector2[][] coordinates;
    // Add additional properties if needed.
}

[System.Serializable]
public class BuildingGeometry
{
    public string type;
    public string id;
    public float width;
    public Vector2[][] coordinates;
    // Add additional properties if needed.
}
