using UnityEngine;

public class BuildingMeshGenerator : MonoBehaviour
{
    public TextAsset jsonFile;

    private void Start()
    {
        GenerateMeshFromJSON();
    }

    private void GenerateMeshFromJSON()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not assigned!");
            return;
        }

        // Parse the JSON data
        MapData mapData = JsonUtility.FromJson<MapData>(jsonFile.text);

        // Loop through the buildings in the JSON data
        foreach (Geometry building in mapData.buildings)
        {
            if (building.type == "MultiPolygon")
            {
                // Create a new GameObject for each building
                GameObject buildingGO = new GameObject("Building");
                buildingGO.transform.SetParent(transform);

                // Loop through the coordinates of the building
                foreach (Coordinate coordinate in building.coordinates)
                {
                    // Create a mesh for each set of coordinates
                    Mesh mesh = CreateMeshFromCoordinates(coordinate);

                    // Create a new GameObject for the mesh
                    GameObject meshGO = new GameObject("Mesh");
                    meshGO.transform.SetParent(buildingGO.transform);

                    // Add a MeshFilter and MeshRenderer to the GameObject
                    MeshFilter meshFilter = meshGO.AddComponent<MeshFilter>();
                    MeshRenderer meshRenderer = meshGO.AddComponent<MeshRenderer>();

                    // Assign the mesh to the MeshFilter
                    meshFilter.sharedMesh = mesh;
                }
            }
        }
    }

    private Mesh CreateMeshFromCoordinates(Coordinate coordinate)
    {
        Mesh mesh = new Mesh();

        // Create vertices from the coordinate's points
        Vector3[] vertices = new Vector3[coordinate.points.Length];
        for (int i = 0; i < coordinate.points.Length; i++)
        {
            vertices[i] = new Vector3(coordinate.points[i].x, 0f, coordinate.points[i].y);
        }

        // Create triangles based on the vertices
        int[] triangles = new int[vertices.Length - 2];
        for (int i = 0; i < vertices.Length - 2; i++)
        {
            triangles[i] = 0;
            triangles[i + 1] = i + 1;
            triangles[i + 2] = i + 2;
        }

        // Assign the vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}

[System.Serializable]
public class MapData
{
    public MapValues values;
    public Geometry earth;
    public Geometry[] roads;
    public Geometry[] walls;
    public Geometry[] rivers;
    public Geometry[] planks;
    public Geometry[] buildings;
    public Geometry[] prisms;
    public Geometry[] squares;
    public Geometry[] greens;
    public Geometry[] fields;
    public Coordinate[] trees;
    public Geometry[] districts;
}

[System.Serializable]
public class MapValues
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
