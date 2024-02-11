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
    public KeyCode keyToPressKeyCode;

    public KeyToPress keyToPress;

    private bool isInitializingFirebase = false;

    // Enum to define the keys
    public enum KeyToPress { W, A, S, D }

    // Getting the SpriteRenderer of the component
    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();

        if (!isInitializingFirebase && !PauseMenu.instance.gameIsPaused)
        {
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
            keyToPressKeyCode = keyCode;
        });
    }

    public void GetKeyCodeFromDatabase(string keyName, System.Action<KeyCode> callback)
    {
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

                    // Convert the string representation of the key code to the KeyCode value
                    KeyCode retrievedKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCode);

                    // Invoke the callback with the retrieved key code
                    callback(retrievedKeyCode);
                }
            });
    }

    void SetDefaultKeyValues()
    {
        // Set default key values if the username is empty or null
        switch (keyToPress)
        {
            case KeyToPress.W:
                keyToPressKeyCode = KeyCode.W;
                break;
            case KeyToPress.A:
                keyToPressKeyCode = KeyCode.A;
                break;
            case KeyToPress.S:
                keyToPressKeyCode = KeyCode.S;
                break;
            case KeyToPress.D:
                keyToPressKeyCode = KeyCode.D;
                break;
            default:
                Debug.LogError("Invalid key code");
                break;
        }
    }

    void Update()
    {
        if (!PauseMenu.instance.gameIsPaused)
        {
            if (Input.GetKeyDown(keyToPressKeyCode))
            {
                theSR.sprite = pressedImg;
            }

            if (Input.GetKeyUp(keyToPressKeyCode))
            {
                theSR.sprite = defaultImg;
            }
        }
    }
}
