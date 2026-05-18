using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class RemoveInGameUITool
{
    [MenuItem("Pirate Tiles/Remove InGame HUD (No Logic change)")]
    public static void RemoveUI()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        // 1. Remove Views (HUD)
        var heartsView = Object.FindAnyObjectByType<HeartsView>(FindObjectsInactive.Include);
        if (heartsView != null) Object.DestroyImmediate(heartsView.gameObject);

        var coinsView = Object.FindAnyObjectByType<CoinsView>(FindObjectsInactive.Include);
        if (coinsView != null) Object.DestroyImmediate(coinsView.gameObject);


        // 3. Keep HeartsController and CoinsController, just clear their view references
        var heartsCtrl = Object.FindAnyObjectByType<HeartsController>(FindObjectsInactive.Include);
        if (heartsCtrl != null)
        {
            var so = new SerializedObject(heartsCtrl);
            so.FindProperty("_heartsView").objectReferenceValue = null;
            so.ApplyModifiedProperties();
        }

        var coinsCtrl = Object.FindAnyObjectByType<CoinsController>(FindObjectsInactive.Include);
        if (coinsCtrl != null)
        {
            var so = new SerializedObject(coinsCtrl);
            so.FindProperty("_coinsView").objectReferenceValue = null;
            so.ApplyModifiedProperties();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Successfully removed HUD (Hearts, Coins) and completely removed Timer from the scene!");
    }
}
