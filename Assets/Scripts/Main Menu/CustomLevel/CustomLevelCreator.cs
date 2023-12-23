using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomLevelCreator : MonoBehaviour
{
    public Text statusText1;
    public Text statusText2;

    // Method to create a copy of the specified scene and save it in the CustomLevels folder
    public void CopyAndSaveCustomLevel()
    {
        string sceneToCopy = "ClearLevel"; // Replace with the scene name you want to copy
        string copiedSceneName = "Copied_" + sceneToCopy; // Unique name for the copied scene

        string customLevelsFolderPath = Application.persistentDataPath + "/CustomLevels"; // Use persistentDataPath to save in the game's data folder

        if (!Directory.Exists(customLevelsFolderPath))
        {
            Directory.CreateDirectory(customLevelsFolderPath);
        }

        string originalScenePath = Application.dataPath + "/Scenes/Levels/" + sceneToCopy + ".unity";
        string copiedScenePath = customLevelsFolderPath + "/" + copiedSceneName + ".unity";

        if (File.Exists(originalScenePath))
        {
            File.Copy(originalScenePath, copiedScenePath, true);
        }

        if (SceneWasCopied(copiedSceneName))
        {
            LoadCopiedScene(copiedSceneName);
        }
    }

    private bool SceneWasCopied(string sceneName)
    {
        string copiedScenePath = Application.persistentDataPath + "/CustomLevels/" + sceneName + ".unity";
        return File.Exists(copiedScenePath);
    }

    // Load the scene with the given name
    private void LoadCopiedScene(string sceneName)
    {
        SceneManager.LoadScene("Scenes/Levels/CustomLevels/" + sceneName);
    }
}
