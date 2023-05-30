using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class FeatureCollection
{
    public Feature[] features;
}

[System.Serializable]
public class Feature
{
    public string type;
    public string id;
    public BuildingData buildings;
    // ... altre proprietà
}

[System.Serializable]
public class BuildingData
{
    public Coordinate[][] coordinates;
}

[System.Serializable]
public class Coordinate
{
    public float[] position;
}

public class BuildingMeshGenerator : EditorWindow
{
    private string jsonFilePath = "path/to/your/json/file.json";
    private BuildingData buildingData;

    [MenuItem("Window/Building Mesh Generator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BuildingMeshGenerator));
    }

    private void OnGUI()
    {
        GUILayout.Label("Building Mesh Generator", EditorStyles.boldLabel);
        jsonFilePath = EditorGUILayout.TextField("JSON File Path:", jsonFilePath);

        if (GUILayout.Button("Generate Building Mesh"))
        {
            GenerateMeshFromJSON();
        }
    }

    private void GenerateMeshFromJSON()
    {
        if (string.IsNullOrEmpty(jsonFilePath))
        {
            Debug.LogError("JSON file path is empty!");
            return;
        }

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("JSON file does not exist!");
            return;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        FeatureCollection featureCollection = JsonUtility.FromJson<FeatureCollection>(jsonContent);

        if (featureCollection != null && featureCollection.features != null && featureCollection.features.Length > 0)
        {
            foreach (Feature feature in featureCollection.features)
            {
                if (feature.buildings != null)
                {
                    buildingData = feature.buildings;

                    // Aggiungi questa verifica per assicurarti che buildingData non sia null
                    if (buildingData != null)
                    {
                        CreateBuildingMesh();
                    }
                }
            }
        }
    }

    private void CreateBuildingMesh()
    {
        // Recupera i dati dei vertici dalla buildingData
        Coordinate[][] buildingCoordinates = buildingData.coordinates;

        // Crea i vertici della mesh
        Vector3[] vertices = new Vector3[buildingCoordinates.Length * buildingCoordinates[0].Length];
        int vertexIndex = 0;
        foreach (Coordinate[] coordinates in buildingCoordinates)
        {
            foreach (Coordinate coordinate in coordinates)
            {
                float x = coordinate.position[0];
                float y = coordinate.position[1];
                float z = coordinate.position[2];
                Vector3 vertex = new Vector3(x, y, z);
                vertices[vertexIndex] = vertex;
                vertexIndex++;
            }
        }

        // Crea le triangolazioni della mesh
        int[] triangles = new int[(buildingCoordinates.Length - 1) * (buildingCoordinates[0].Length - 1) * 6];
        int triangleIndex = 0;
        for (int i = 0; i < buildingCoordinates.Length - 1; i++)
        {
            for (int j = 0; j < buildingCoordinates[0].Length - 1; j++)
            {
                int topLeft = (i * buildingCoordinates[0].Length) + j;
                int topRight = topLeft + 1;
                int bottomLeft = ((i + 1) * buildingCoordinates[0].Length) + j;
                int bottomRight = bottomLeft + 1;

                triangles[triangleIndex] = topLeft;
                triangles[triangleIndex + 1] = bottomLeft;
                triangles[triangleIndex + 2] = topRight;
                triangles[triangleIndex + 3] = topRight;
                triangles[triangleIndex + 4] = bottomLeft;
                triangles[triangleIndex + 5] = bottomRight;

                triangleIndex += 6;
            }
        }

        // Crea la mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Assegna la mesh all'oggetto GameObject corrente o creane uno nuovo
        GameObject buildingObject = Selection.activeGameObject;
        if (buildingObject == null)
        {
            buildingObject = new GameObject("Building");
            buildingObject.AddComponent<MeshFilter>();
            buildingObject.AddComponent<MeshRenderer>();
        }

        buildingObject.GetComponent<MeshFilter>().sharedMesh = mesh;

        Debug.Log("Building mesh generated!");
    }
}
