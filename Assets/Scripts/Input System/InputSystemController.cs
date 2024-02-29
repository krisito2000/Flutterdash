using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemController : MonoBehaviour
{
    public static InputSystemController instance;

    private PlayerInput playerInput;

    private InputAction circleClickAction;
    public InputAction upAction;
    public InputAction downAction;
    public InputAction leftAction;
    public InputAction rightAction;

    public bool UpCircleClicked { get; private set; }
    public bool UpCircleRelease { get; private set; }

    public bool LeftCircleClicked { get; private set; }
    public bool LeftCircleRelease { get; private set; }

    public bool DownCircleClicked { get; private set; }
    public bool DownCircleRelease { get; private set; }

    public bool RightCircleClicked { get; private set; }
    public bool RightCircleRelease { get; private set; }



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
        UpCircleRelease = upAction.WasReleasedThisFrame();
        Debug.Log($"Up Circle: {UpCircleClicked}");

        DownCircleClicked = downAction.WasPressedThisFrame();
        DownCircleRelease = downAction.WasReleasedThisFrame();
        Debug.Log($"Down Circle: {DownCircleClicked}");

        LeftCircleClicked = leftAction.WasPressedThisFrame();
        LeftCircleRelease = leftAction.WasReleasedThisFrame();
        Debug.Log($"Left Circle: {LeftCircleClicked}");

        RightCircleClicked = rightAction.WasPressedThisFrame();
        RightCircleRelease = rightAction.WasReleasedThisFrame();
        Debug.Log($"Right Circle: {RightCircleClicked}");
    }
}
