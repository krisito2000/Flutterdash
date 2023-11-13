using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        User newUser = new User(Username.text, Password.text);
        string json = JsonUtility.ToJson(newUser);

        databaseReference.Child("users").Child(userID).SetRawJsonValueAsync(json);
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
        var userNameData = databaseReference.Child("users").Child(userID).Child("username").GetValueAsync();
        yield return new WaitUntil(() => userNameData.IsCompleted);

        if (userNameData != null)
        {
            DataSnapshot snapshot = userNameData.Result;
            onCallBack.Invoke(snapshot.Value.ToString());
        }
    }

    public IEnumerator GetPassword(Action<string> onCallBack)
    {
        var userPasswordData = databaseReference.Child("users").Child(userID).Child("password").GetValueAsync();
        yield return new WaitUntil(() => userPasswordData.IsCompleted);

        if (userPasswordData != null)
        {
            DataSnapshot snapshot = userPasswordData.Result;
            onCallBack.Invoke(snapshot.Value.ToString());
        }
    }
}
