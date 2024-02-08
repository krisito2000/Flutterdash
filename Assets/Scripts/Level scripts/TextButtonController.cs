using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextButtonController : MonoBehaviour
{
    public static TextButtonController instance;

    public Text CircleText;

    public string keyToPressKeyCode;
    public KeyToPress keyToPress;

    public enum KeyToPress { W, A, S, D }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (string.IsNullOrEmpty(keyToPressKeyCode))
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
    public IEnumerator InitializeFirebaseAndGetData()
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

        // Get key code from the database based on the keyToPress setting
        GetKeyCodeFromDatabase(keyToPress.ToString(), (keyCode) =>
        {
            // Set the CircleText to display the retrieved key code
            CircleText.text = keyCode;

            keyToPressKeyCode = keyCode;
        });
    }
    public void GetKeyCodeFromDatabase(string keyName, System.Action<string> callback)
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

                    // Invoke the callback with the retrieved key code
                    callback(keyCode);
                }
            });
    }

    void SetDefaultKeyValues()
    {
        // Set default key values if the username is empty or null
        switch (keyToPress)
        {
            case KeyToPress.W:
                CircleText.text = KeyCode.W.ToString();
                break;
            case KeyToPress.A:
                CircleText.text = KeyCode.A.ToString();
                break;
            case KeyToPress.S:
                CircleText.text = KeyCode.S.ToString();
                break;
            case KeyToPress.D:
                CircleText.text = KeyCode.D.ToString();
                break;
            default:
                Debug.LogError("Invalid key code");
                break;
        }
    }
}
