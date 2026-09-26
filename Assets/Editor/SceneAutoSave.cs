using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SceneAutoSave
{
    private const double IntervalSeconds = 120;
    private static double nextSave;

    static SceneAutoSave()
    {
        nextSave = EditorApplication.timeSinceStartup + IntervalSeconds;
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (EditorApplication.timeSinceStartup < nextSave)
            return;

        nextSave = EditorApplication.timeSinceStartup + IntervalSeconds;

        if (EditorApplication.isPlayingOrWillChangePlaymode ||
            EditorApplication.isCompiling ||
            EditorApplication.isUpdating)
            return;

        Scene scene = SceneManager.GetActiveScene();

        if (!scene.isDirty || string.IsNullOrEmpty(scene.path))
            return;

        EditorSceneManager.SaveScene(scene);
        Debug.Log($"Auto-saved scene: {scene.name}");
    }
}