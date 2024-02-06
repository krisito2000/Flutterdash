using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("------- Images -------")]
    private SpriteRenderer theSR;
    public Sprite defaultImg;
    public Sprite pressedImg;

    [Header("------- Keys -------")]
    public KeyToPress keyToPress;

    // Enum to define the keys
    public enum KeyToPress { Up, Left, Down, Right }

    // Getting the SpriteRenderer of the component
    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();

        // Set default key values if the username is empty or null
        if (string.IsNullOrEmpty(DatabaseManager.instance.username))
        {
            SetDefaultKeyValues();
        }
        else
        {
            StartCoroutine(InitializeFirebaseAndGetData());
        }
    }

    IEnumerator InitializeFirebaseAndGetData()
    {
        // Wait for Firebase to finish checking dependencies
        var checkDependenciesTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => checkDependenciesTask.IsCompleted);

        // Check if Firebase initialization was successful
        if (checkDependenciesTask.Exception != null)
        {
            Debug.LogError("Failed to initialize Firebase: " + checkDependenciesTask.Exception);
            yield break; // Exit the coroutine if initialization failed
        }

        // Get key code from the database based on the username settings
        GetKeyCodeFromDatabase(keyToPress.ToString(), (keyCode) =>
        {
            // Assign the retrieved key code to the class variable
            keyToPress = keyCode;
        });
    }

    public void GetKeyCodeFromDatabase(string keyName, System.Action<KeyToPress> callback)
    {
        // Get data from the database
        DatabaseManager.instance.databaseReference
            .Child("Users")
            .Child(DatabaseManager.instance.username)
            .Child("Settings")
            .Child("Input")
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve key codes: " + task.Exception);
                    return;
                }
                else if (task.IsCompleted)
                {
                    // Process retrieved data
                    DataSnapshot snapshot = task.Result;
                    string keyCode = snapshot.Child(keyName).Value.ToString();

                    // Assign the key code
                    KeyToPress keyToPress = (KeyToPress)System.Enum.Parse(typeof(KeyToPress), keyCode);
                    callback(keyToPress);
                }
            });
    }

    void SetDefaultKeyValues()
    {
        // Set default key values if the username is empty or null
        switch (keyToPress)
        {
            case KeyToPress.Up:
                keyToPress = KeyToPress.Up;
                break;
            case KeyToPress.Left:
                keyToPress = KeyToPress.Left;
                break;
            case KeyToPress.Down:
                keyToPress = KeyToPress.Down;
                break;
            case KeyToPress.Right:
                keyToPress = KeyToPress.Right;
                break;
            default:
                Debug.LogError("Invalid key code");
                break;
        }
    }

    // Checking if the button on the keyboard is pressed and changing the image
    void Update()
    {
        if (!PauseMenu.instance.gameIsPaused)
        {
            if (Input.GetKeyDown(GetKeyCode()))
            {
                theSR.sprite = pressedImg;
            }

            if (Input.GetKeyUp(GetKeyCode()))
            {
                theSR.sprite = defaultImg;
            }
        }
    }

    KeyCode GetKeyCode()
    {
        switch (keyToPress)
        {
            case KeyToPress.Up:
                return GetKeyCodeFromDatabase("W");
            case KeyToPress.Left:
                return GetKeyCodeFromDatabase("A");
            case KeyToPress.Down:
                return GetKeyCodeFromDatabase("S");
            case KeyToPress.Right:
                return GetKeyCodeFromDatabase("D");
            default:
                Debug.LogError("Invalid key code");
                return KeyCode.None;
        }
    }

    KeyCode GetKeyCodeFromDatabase(string keyName)
    {
        // Create a variable to store the retrieved key code
        KeyCode retrievedKeyCode = KeyCode.None;

        // Get data from the database
        DatabaseManager.instance.databaseReference
            .Child("Users")
            .Child(DatabaseManager.instance.username)
            .Child("Settings")
            .Child("Input")
            .Child(keyName)
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve key code: " + task.Exception);
                    return;
                }
                else if (task.IsCompleted)
                {
                    // Process retrieved data
                    DataSnapshot snapshot = task.Result;
                    string keyCode = snapshot.Value.ToString();

                    // Convert the string representation of the key code to the KeyCode enum value
                    retrievedKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCode);
                }
            });

        // Return the retrieved key code
        return retrievedKeyCode;
    }

}
