using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Newtonsoft.Json;

public class BuildingMeshGenerator : MonoBehaviour
{
    public TextAsset jsonFile;

    private void OnValidate()
    {
        if (jsonFile != null)
        {
            GenerateBuildingMeshes();
        }
    }

    [ContextMenu("Generate Building Meshes")]
    private void GenerateBuildingMeshes()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON file is not assigned.");
            return;
        }

        // Parse JSON data
        MapData mapData = JsonConvert.DeserializeObject<MapData>(jsonFile.text);
        if (mapData.buildings != null)
        {
            // Create building meshes
            foreach (Geometry building in mapData.buildings)
            {
                GameObject buildingObject = new GameObject();
                buildingObject.transform.SetParent(transform);
                buildingObject.name = building.type;

                MeshFilter meshFilter = buildingObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = buildingObject.AddComponent<MeshRenderer>();

                // Create mesh data from building coordinates
                Mesh mesh = new Mesh();
                Vector3[] vertices = new Vector3[building.coordinates.Length];

                for (int i = 0; i < building.coordinates.Length; i++)
                {
                    vertices[i] = new Vector3(building.coordinates[i].x, 0f, building.coordinates[i].y);
                }

                int[] triangles = new int[(building.coordinates.Length - 2) * 3];
                int triangleIndex = 0;

                for (int i = 1; i < building.coordinates.Length - 1; i++)
                {
                    triangles[triangleIndex] = 0;
                    triangles[triangleIndex + 1] = i;
                    triangles[triangleIndex + 2] = i + 1;

                    triangleIndex += 3;
                }

                // Assign mesh data to mesh
                mesh.vertices = vertices;
                mesh.triangles = triangles;

                meshFilter.mesh = mesh;
                meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
            }
        }
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/Generate Building Meshes")]
    private static void CreateBuildingMeshes()
    {
        if (Selection.activeGameObject != null)
        {
            Selection.activeGameObject.AddComponent<BuildingMeshGenerator>();
        }
    }
#endif
}

[System.Serializable]
public class MapData
{
    public Values values;
    public Geometry earth;
    public Geometry[] roads;
    public Geometry[] walls;
    public Geometry[] rivers;
    public Geometry[] planks;
    public Geometry[] buildings;
    //public Geometry[] prisms;
    //public Geometry[] squares;
    //public Geometry[] greens;
    public Geometry[] fields;
    //public Coordinate[] trees;
    //public Geometry[] districts;
}

[System.Serializable]
public class Values
{
    public float roadWidth;
    public float towerRadius;
    public float wallThickness;
    public string generator;
    public string version;
    public float riverWidth;
}

[System.Serializable]
public class Geometry
{
    public string type;
    public float width;
    public Coordinate[] coordinates;
}

[System.Serializable]
public class Coordinate
{
    public float x;
    public float y;
}
