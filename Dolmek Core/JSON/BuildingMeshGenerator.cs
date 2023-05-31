using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingMeshGenerator : EditorWindow
{
    public TextAsset jsonFile; // Aggiunto campo per il file JSON
    //private const string jsonFilePath = "Assets/Dolmek Core/JSON/cauldronhope.json";
    

    [MenuItem("Window/Building Mesh Generator")]
    static void Init()
    {
        BuildingMeshGenerator window = (BuildingMeshGenerator)EditorWindow.GetWindow(typeof(BuildingMeshGenerator));
        window.Show();
    }

    private void OnGUI()
    {
        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);

        if (GUILayout.Button("Generate Building Meshes"))
        {
            GenerateBuildingMeshes();
        }
    }

    private void GenerateBuildingMeshes()
    {
        if (jsonFile == null)
        {
            Debug.LogError("No JSON file selected.");
            return;
        }

        string jsonText = jsonFile.text;
        Debug.Log(jsonText); // Visualizza il contenuto del file JSON nella console

        MapData mapData = UnityEngine.JsonUtility.FromJson<MapData>(jsonText);

        if (mapData.buildings != null)
        {
            foreach (BuildingData buildingData in mapData.buildings)
            {
                string type = buildingData.type;
                List<Vector2> coordinates = GetCoordinates(buildingData.geometry.coordinates);

                int[] triangles = Triangulator.Triangulate(coordinates);

                Vector3[] vertices = new Vector3[coordinates.Count];
                for (int i = 0; i < coordinates.Count; i++)
                {
                    vertices[i] = new Vector3(coordinates[i].x, 0, coordinates[i].y);
                }

                // Creazione del nuovo GameObject e allegare i componenti MeshFilter e MeshRenderer
                GameObject buildingObject = new GameObject(type);
                MeshFilter meshFilter = buildingObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = buildingObject.AddComponent<MeshRenderer>();

                // Create a new mesh and assign vertices and triangles
                Mesh mesh = new Mesh();
                mesh.vertices = vertices;
                mesh.triangles = triangles;

                // Calculate normals and bounds
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                // Assign the mesh to the mesh filter
                meshFilter.sharedMesh = mesh;

                // Posiziona il nuovo GameObject nella scena
                buildingObject.transform.position = new Vector3(0, 0, 0); // Modifica le coordinate XYZ in base alle tue esigenze

                // Puoi anche impostare altre proprietà del GameObject come la scala e l'orientamento

                // Assegna il materiale al MeshRenderer
                meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

                // Aggiungi ulteriori istruzioni per configurare le proprietà dei GameObject e delle mesh, se necessario
            }

            Debug.Log("Building meshes generated.");
        }
        else
        {
            Debug.Log("No buildings found in JSON.");
        }
    }

    private List<Vector2> GetCoordinates(List<List<float>> coordinatesList)
    {
        List<Vector2> coordinates = new List<Vector2>();

        foreach (List<float> coordinate in coordinatesList)
        {
            float x = coordinate[0];
            float z = coordinate[1];
            coordinates.Add(new Vector2(x, z));
        }

        return coordinates;
    }
}

[System.Serializable]
public class MapData
{
    public List<BuildingData> buildings;
}

[System.Serializable]
public class BuildingData
{
    public string type;
    public GeometryData geometry;
}

[System.Serializable]
public class GeometryData
{
    public List<List<float>> coordinates;
}
