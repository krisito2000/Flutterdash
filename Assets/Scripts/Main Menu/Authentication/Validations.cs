using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Validations : MonoBehaviour
{
    public static Validations instance;

    void Start()
    {
        instance = this;
    }

    // Method to validate registration fields before submitting
    public bool RegisterButton()
    {
        bool isUsernameValid = ValidateUsername();
        bool isEmailValid = ValidateEmail();
        bool isPasswordValid = ValidatePassword();
        bool isConfirmPasswordValid = ValidateConfirmPassword();

        // Return true if all fields are valid, false otherwise
        return isUsernameValid && isEmailValid && isPasswordValid && isConfirmPasswordValid;
    }

    // Method to validate username field
    public bool ValidateUsername()
    {
        if (!string.IsNullOrEmpty(DatabaseManager.instance.RegisterUsernameField.text))
        {
            if (DatabaseManager.instance.RegisterUsernameField.text.Length >= 6)
            {
                // Username valid
                DatabaseManager.instance.RegisterUsernameErrorMessage.text = "";
                DatabaseManager.instance.RegisterUsernameErrorCanvasGroup.alpha = 0;
                return true;
            }
        }
        return false;
    }

    // Method to validate email field
    public bool ValidateEmail()
    {
        if (!string.IsNullOrEmpty(DatabaseManager.instance.RegisterEmailField.text))
        {
            string emailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                // Matches the local part of the email address before the '@' symbol.
                                // This section allows word characters, hyphens, periods in the local part.
                                // It handles subdomains or a single letter domain name.
                                // @ symbol separates the local part from the domain part.
                                + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                // Matches the IP address in IPv4 format.
                                // This section allows numbers in each segment separated by periods.
                                // Ensures each segment is in the range of 0-255.
                                // {1} allows the IP address to occur only once.
                                + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                // Alternation operator to match either the IP address pattern or the domain name pattern.
                                + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
                                // Matches domain names - allows letters, hyphens, and periods.
                                // Validates the last segment of the domain name, ensuring it's between 2 to 4 characters.
                                // End of string anchor ($).
            if (System.Text.RegularExpressions.Regex.IsMatch(DatabaseManager.instance.RegisterEmailField.text, emailPattern))
            {
                // Email valid
                DatabaseManager.instance.RegisterEmailErrorMessage.text = "";
                DatabaseManager.instance.RegisterEmailErrorCanvasGroup.alpha = 0;
                return true;
            }
        }
        return false;
    }

    // Method to validate password field
    public bool ValidatePassword()
    {
        if (!string.IsNullOrEmpty(DatabaseManager.instance.RegisterPasswordField.text))
        {
            if (DatabaseManager.instance.RegisterPasswordField.text.Length >= 6)
            {
                // Password valid
                DatabaseManager.instance.RegisterPasswordErrorMessage.text = "";
                DatabaseManager.instance.RegisterPasswordErrorCanvasGroup.alpha = 0;
                return true;
            }
        }
        return false;
    }

    // Method to validate confirm password field
    public bool ValidateConfirmPassword()
    {
        if (DatabaseManager.instance.RegisterConfirmPasswordField.text == DatabaseManager.instance.RegisterPasswordField.text)
        {
            return true;
        }
        return false;
    }
}
