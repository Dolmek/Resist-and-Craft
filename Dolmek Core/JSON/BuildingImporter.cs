using UnityEditor;
using UnityEngine;

public class BuildingImporter : Editor
{
    [MenuItem("Window/Import Building Meshes")]
    static void ImportMeshes()
    {
        string[] meshPaths = AssetDatabase.FindAssets("t:Mesh", new[] { "Assets/Dolmek Core/Resources/Meshes" });

        foreach (string meshPath in meshPaths)
        {
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(AssetDatabase.GUIDToAssetPath(meshPath));
            GameObject buildingObject = new GameObject(mesh.name);
            MeshFilter meshFilter = buildingObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = buildingObject.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = mesh;
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

            // Posiziona il nuovo GameObject nella scena
            buildingObject.transform.position = new Vector3(0, 0, 0); // Modifica le coordinate XYZ in base alle tue esigenze

            // Puoi anche impostare altre proprietà del GameObject come la scala e l'orientamento

            Debug.Log("Mesh imported: " + mesh.name);
        }

        Debug.Log("Building meshes imported to scene.");
    }
}
