using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CircleTextUpdate : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public TMPro.TextMeshProUGUI currentKeyText;

    public enum KeyToPress { Up, Left, Down, Right }
    public KeyToPress keyToPress;

    void Start()
    {
        UpdateCurrentKeyText();
    }

    void UpdateCurrentKeyText()
    {
        string keyBinding = GetKeyBinding();
        currentKeyText.text = keyBinding;
    }

    string GetKeyBinding()
    {
        var actionMap = inputActionAsset.FindActionMap("Gameplay");

        switch (keyToPress)
        {
            case KeyToPress.Up:
                return GetFormattedBinding(actionMap.FindAction("UpCircle").bindings[0]);
            case KeyToPress.Down:
                return GetFormattedBinding(actionMap.FindAction("DownCircle").bindings[0]);
            case KeyToPress.Left:
                return GetFormattedBinding(actionMap.FindAction("LeftCircle").bindings[0]);
            case KeyToPress.Right:
                return GetFormattedBinding(actionMap.FindAction("RightCircle").bindings[0]);
            default:
                return "Unknown Key";
        }
    }

    string GetFormattedBinding(InputBinding binding)
    {
        string bindingString = binding.effectivePath;
        if (bindingString.StartsWith("<Keyboard>/"))
        {
            bindingString = bindingString.Substring("<Keyboard>/".Length);
        }
        return bindingString.ToUpper();
    }
}
