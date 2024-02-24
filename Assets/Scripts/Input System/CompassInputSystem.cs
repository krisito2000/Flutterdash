using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CompassInputSystem : MonoBehaviour
{
    public static CompassInputSystem instance;

    private PlayerInput playerInput;

    private InputAction circleClickAction;
    private InputAction upAction;
    private InputAction downAction;
    private InputAction leftAction;
    private InputAction rightAction;

    public bool UpCircleClicked { get; private set; }
    public bool LeftCircleClicked { get; private set; }
    public bool DownCircleClicked { get; private set; }
    public bool RightCircleClicked { get; private set; }


    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();

        SetUpInputActions();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
    }

    private void SetUpInputActions()
    {
        upAction = playerInput.actions["UpCircle"];
        downAction = playerInput.actions["DownCircle"];
        leftAction = playerInput.actions["LeftCircle"];
        rightAction = playerInput.actions["RightCircle"];

        Debug.Log(circleClickAction);
    }

    private void UpdateInputs()
    {
        UpCircleClicked = upAction.WasPressedThisFrame();
        Debug.Log($"Up Circle: {UpCircleClicked}");

        DownCircleClicked = downAction.WasPressedThisFrame();
        Debug.Log($"Down Circle: {DownCircleClicked}");

        LeftCircleClicked = leftAction.WasPressedThisFrame();
        Debug.Log($"Left Circle: {LeftCircleClicked}");

        RightCircleClicked = rightAction.WasPressedThisFrame();
        Debug.Log($"Right Circle: {RightCircleClicked}");
    }
}
