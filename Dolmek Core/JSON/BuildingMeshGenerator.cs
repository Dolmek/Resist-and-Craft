using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
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
        foreach (BuildingFeature feature in buildingData.features)
        {
            if (feature.geometry.type == "Polygon" || feature.geometry.type == "MultiPolygon")
            {
                // Create a new GameObject for the mesh.
                GameObject gameObject = new GameObject(feature.geometry.id);

                // Create a new MeshFilter for the GameObject.
                MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

                // Create a new MeshRenderer for the GameObject.
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

                // Set the materials for the MeshRenderer.
                meshRenderer.sharedMaterials = GetMaterialsForGeometry(feature.geometry);

                // Create a new Mesh from the coordinates.
                Mesh mesh = CreateMeshFromCoordinates(feature.geometry.coordinates);

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

    private Mesh CreateMeshFromCoordinates(List<List<List<Vector2>>> coordinates)
    {
        Mesh mesh = new Mesh();

        // Create a list to hold all the vertices.
        List<Vector3> vertices = new List<Vector3>();

        // Create a list to hold all the triangles.
        List<int> triangles = new List<int>();

        // Loop through the coordinate arrays and add vertices and triangles.
        foreach (List<List<Vector2>> coordinateArray in coordinates)
        {
            int startIndex = vertices.Count;

            // Add vertices for each coordinate in the array.
            foreach (List<Vector2> coordinateList in coordinateArray)
            {
                foreach (Vector2 coordinate in coordinateList)
                {
                    vertices.Add(new Vector3(coordinate.x, 0f, coordinate.y));
                }
            }

            // Create triangles using the vertices.
            for (int i = 0; i < coordinateArray.Count - 1; i++)
            {
                for (int j = 0; j < coordinateArray[i].Count - 1; j++)
                {
                    int index1 = startIndex + i * coordinateArray[i].Count + j;
                    int index2 = index1 + coordinateArray[i].Count;
                    int index3 = index2 + 1;
                    int index4 = index1 + 1;

                    triangles.Add(index1);
                    triangles.Add(index2);
                    triangles.Add(index3);

                    triangles.Add(index1);
                    triangles.Add(index3);
                    triangles.Add(index4);
                }
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
    public BuildingGeometry geometry;
    // Add additional properties if needed.
}

[System.Serializable]
public class BuildingGeometry
{
    public string type;
    public float width;
    public List<List<List<Vector2>>> coordinates;
    // Add additional properties if needed.
}
