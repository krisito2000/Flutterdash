#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

public class CustomLevelCreator
{
    // Method to create a copy of the specified scene and save it in the CustomLevels folder
    public void CopyAndSaveCustomLevel()
    {
        string sceneToCopy = "ClearLevel"; // Replace with the scene name you want to copy
        string copiedSceneName = "Copied_" + sceneToCopy; // Unique name for the copied scene

#if UNITY_EDITOR
        // Check if the Levels folder exists, and if not, create it
        string levelsFolderPath = "Assets/Scenes/Levels";
        if (!AssetDatabase.IsValidFolder(levelsFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
            AssetDatabase.CreateFolder("Assets/Scenes", "Levels");
        }

        // Check if the CustomLevels folder exists under Levels, and if not, create it
        string customLevelsFolderPath = levelsFolderPath + "/CustomLevels";
        if (!AssetDatabase.IsValidFolder(customLevelsFolderPath))
        {
            AssetDatabase.CreateFolder(levelsFolderPath, "CustomLevels");
        }

        // Copy the scene
        SceneAsset originalSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(levelsFolderPath + "/" + sceneToCopy + ".unity");
        if (originalSceneAsset != null)
        {
            // Duplicate the scene by moving it to the CustomLevels folder
            string copiedScenePath = customLevelsFolderPath + "/" + copiedSceneName + ".unity";
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(originalSceneAsset), copiedScenePath);
            AssetDatabase.Refresh(); // Refresh the AssetDatabase to detect changes
        }
        if (SceneWasCopied(copiedSceneName))
        {
            LoadCopiedScene(copiedSceneName);
        }
#endif
    }
    private bool SceneWasCopied(string sceneName)
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Levels/CustomLevels/" + sceneName + ".unity") != null;
#else
        return false;
#endif
    }
    // Load the scene with the given name
    private void LoadCopiedScene(string sceneName)
    {
        SceneManager.LoadScene("Scenes/Levels/CustomLevels/" + sceneName);
    }
}
