using UnityEditor;
using UnityEditor.SceneManagement;

namespace Utility.Editor
{
    public static class SceneSelector
    {
        [MenuItem("File/Scene/Loading")]
        public static void OpenLoadingScene()
        {
            var scenePath = EditorBuildSettings.scenes[0].path;
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        [MenuItem("File/Scene/Game")]
        public static void OpenGameScene()
        {
            var scenePath = EditorBuildSettings.scenes[1].path;
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
    }
}