using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildingMeshGenerator : UnityEditor.EditorWindow
{
    [UnityEditor.MenuItem("Window/Building Mesh Generator")]
    public static void ShowWindow()
    {
        UnityEditor.EditorWindow.GetWindow(typeof(BuildingMeshGenerator));
    }

    private string jsonFilePath = "Assets/Dolmek Core/JSON/cauldronhope.json";

    private void OnGUI()
    {
        GUILayout.Label("Building Mesh Generator", UnityEditor.EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Building Mesh"))
        {
            GenerateBuildingMesh();
        }
    }

    private void GenerateBuildingMesh()
    {
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);
        RootObject rootObject = JsonUtility.FromJson<RootObject>(jsonText);

        List<Building> buildings = rootObject.buildings;

        foreach (Building building in buildings)
        {
            string type = building.type;
            List<List<List<float>>> coordinates = building.coordinates;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            foreach (List<List<float>> polygon in coordinates)
            {
                foreach (List<float> coordinate in polygon)
                {
                    float x = coordinate[0];
                    float z = coordinate[1];

                    vertices.Add(new Vector3(x, 0f, z));
                }
            }

            int[] buildingTriangles = Triangulator.Triangulate(vertices);
            triangles.AddRange(buildingTriangles);

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            GameObject buildingObject = new GameObject("Building");
            MeshFilter meshFilter = buildingObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = buildingObject.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }

        Debug.Log("Building mesh generation completed.");
    }

    [System.Serializable]
    public class Building
    {
        public string type;
        public List<List<List<float>>> coordinates;
    }

    [System.Serializable]
    public class RootObject
    {
        public List<Building> buildings;
    }
}
