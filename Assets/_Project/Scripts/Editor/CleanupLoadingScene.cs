using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CleanupLoadingScene
{
    [InitializeOnLoadMethod]
    private static void Cleanup()
    {
        if (SessionState.GetBool("CleanupLoadingSceneDone", false)) return;
        SessionState.SetBool("CleanupLoadingSceneDone", true);
        
        EditorApplication.delayCall += () =>
        {
            var activeScene = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/Loading.unity");
            
            bool modified = false;
            var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem != null)
            {
                Object.DestroyImmediate(eventSystem.gameObject);
                modified = true;
            }
            
            if (modified)
            {
                EditorSceneManager.SaveScene(scene);
                Debug.Log("Deleted old EventSystem from Loading scene.");
            }
            
            if (activeScene.path != "Assets/_Project/Scenes/Loading.unity")
            {
                EditorSceneManager.OpenScene(activeScene.path);
            }
        };
    }
}
