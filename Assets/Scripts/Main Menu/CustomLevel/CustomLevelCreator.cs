using UnityEngine;
using System.IO;
using UnityEditor;

public class CustomLevelCreator : MonoBehaviour
{
    // Public property to hold the scene to be copied
    public Object clearSceneAsset;

    // Method to copy, save, and open the custom level scene
    public void CopySaveAndOpenCustomLevel(string musicFileName)
    {
        // Ensure that the ClearClene scene is set in the editor
        if (clearSceneAsset == null)
        {
            Debug.LogError("ClearClene scene is not set. Please assign a scene in the inspector.");
            return;
        }

        // Get the path of the ClearClene scene
        string clearScenePath = AssetDatabase.GetAssetPath(clearSceneAsset);

        // Define the folder path for custom levels
        string levelsFolderPath = Path.Combine(Application.dataPath, "Levels");

        // Check if the "Levels" folder exists, if not, create it
        if (!Directory.Exists(levelsFolderPath))
        {
            Directory.CreateDirectory(levelsFolderPath);
        }

        // Define the file path for the custom level (.unity)
        string customLevelFilePath = Path.Combine(levelsFolderPath, musicFileName + ".unity");

        // Copy the clearScenePath to the custom level file path
        File.Copy(clearScenePath, customLevelFilePath);

        // Refresh the asset database to ensure the new scene is recognized by Unity
        AssetDatabase.Refresh();
    }
}
