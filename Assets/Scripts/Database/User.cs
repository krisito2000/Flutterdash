using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class User
{
    public string username;
    public string email;
    public string password;

    public User(string username, string email, string password)
    {
        this.username = username;
        this.email = email;
        this.password = password;
    }
}
