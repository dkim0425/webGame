using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class CreateEmptyTile
{
    [MenuItem("Assets/Create/2D/Tiles/Empty Tile")]
    public static void CreateTile()
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();

        string path = "Assets/NewEmptyTile.asset";
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(tile, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tile;
    }
}
