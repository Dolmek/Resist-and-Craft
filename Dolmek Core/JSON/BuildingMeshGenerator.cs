using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingMeshGenerator : EditorWindow
{
    private const string jsonFilePath = "Assets/Dolmek Core/JSON/cauldronhope.json";

    [MenuItem("Window/Building Mesh Generator")]
    static void Init()
    {
        BuildingMeshGenerator window = (BuildingMeshGenerator)EditorWindow.GetWindow(typeof(BuildingMeshGenerator));
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate Building Meshes"))
        {
            GenerateBuildingMeshes();
        }
    }

    private void GenerateBuildingMeshes()
    {
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);
        MapData mapData = UnityEngine.JsonUtility.FromJson<MapData>(jsonText);

        if (mapData.buildings != null)
        {
            Debug.Log("Number of buildings in JSON: " + mapData.buildings.Count);

            foreach (BuildingData buildingData in mapData.buildings)
            {
                string type = buildingData.type;
                List<Vector2> coordinates = GetCoordinates(buildingData.geometry.coordinates);

                int[] triangles = Triangulator.Triangulate(coordinates);

                Debug.Log("Building type: " + type);
                Debug.Log("Number of coordinates: " + coordinates.Count);
                Debug.Log("Number of triangles: " + triangles.Length / 3);

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
                meshFilter.mesh = mesh;

                // Posiziona il nuovo GameObject nella scena
                buildingObject.transform.position = new Vector3(0, 0, 0); // Modifica le coordinate XYZ in base alle tue esigenze

                // Puoi anche impostare altre proprietà del GameObject come la scala e l'orientamento

                // Assegna il materiale al MeshRenderer
                meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

                // Aggiungi ulteriori istruzioni per configurare le proprietà dei GameObject e delle mesh, se necessario


            }
        }
        else
        {
            Debug.Log("No buildings found in JSON.");
        }

        Debug.Log("Building meshes generated.");
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
