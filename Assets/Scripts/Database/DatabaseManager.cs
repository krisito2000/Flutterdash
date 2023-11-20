using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using System.Security.Cryptography;
using System.Text;

public class DatabaseManager : MonoBehaviour
{
    public InputField Username;
    public InputField Password;

    public Text UsernameText;
    public Text PasswordText;

    // Make a unice id
    private string userID;
    // Reference to the database
    private DatabaseReference databaseReference;

    // Start is called before the first frame update
    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void CreateUser()
    {
        // Hash the password before storing it
        string hashedPassword = PasswordHashSystem.HashPassword(Password.text);

        User newUser = new User(Username.text, hashedPassword);
        string json = JsonUtility.ToJson(newUser);

        databaseReference.Child("Users").Child(userID).Child("Authentication").SetRawJsonValueAsync(json);
    }


    public void GetUserInfo()
    {
        StartCoroutine(GetUsername((string username) =>
        {
            UsernameText.text = username;
        }));

        StartCoroutine(GetPassword((string password) =>
        {
            PasswordText.text = password;
        }));
    }

    //private void StartCoroutine(IEnumerable enumerable)
    //{
    //    throw new NotImplementedException();
    //}

    public IEnumerator GetUsername(Action<string> onCallBack)
    {
        var userNameData = databaseReference.Child("Users").Child(userID).Child("Authentication").Child("username").GetValueAsync();

        yield return new WaitUntil(() => userNameData.IsCompleted);

        if (userNameData.Exception != null)
        {
            Debug.LogError("Failed to retrieve username: " + userNameData.Exception.Message);
            yield break;
        }

        DataSnapshot snapshot = userNameData.Result;

        if (snapshot != null && snapshot.Exists)
        {
            onCallBack.Invoke(snapshot.Value.ToString());
        }
        else
        {
            Debug.LogWarning("Username data does not exist.");
        }
    }


    public IEnumerator GetPassword(Action<string> onCallBack)
    {
        var userPasswordData = databaseReference.Child("Users").Child(userID).Child("Authentication").Child("password").GetValueAsync();

        yield return new WaitUntil(() => userPasswordData.IsCompleted);

        if (userPasswordData.Exception != null)
        {
            Debug.LogError("Failed to retrieve password: " + userPasswordData.Exception.Message);
            yield break;
        }

        DataSnapshot snapshot = userPasswordData.Result;

        if (snapshot != null && snapshot.Exists)
        {
            onCallBack.Invoke(snapshot.Value.ToString());
        }
        else
        {
            Debug.LogWarning("Password data does not exist.");
        }
    }

}
