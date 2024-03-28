using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    // User class to represent user data
    public string username;
    public string email;
    public string password;

    // Constructor to initialize user data
    public User(string username, string email, string password)
    {
        this.username = username;
        this.email = email;
        this.password = password;
    }
}
