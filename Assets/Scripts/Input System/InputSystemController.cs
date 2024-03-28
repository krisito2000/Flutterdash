using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemController : MonoBehaviour
{
    public static InputSystemController instance;

    // Reference to PlayerInput component
    private PlayerInput playerInput;

    // Input actions for different directions
    private InputAction upAction;
    private InputAction downAction;
    private InputAction leftAction;
    private InputAction rightAction;

    // Properties to track circle click and release for each direction
    public bool UpCircleClicked { get; private set; }
    public bool UpCircleRelease { get; private set; }

    public bool LeftCircleClicked { get; private set; }
    public bool LeftCircleRelease { get; private set; }

    public bool DownCircleClicked { get; private set; }
    public bool DownCircleRelease { get; private set; }

    public bool RightCircleClicked { get; private set; }
    public bool RightCircleRelease { get; private set; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Ensure there is only one instance of InputSystemController
        if (instance == null)
        {
            instance = this;
        }

        // Get reference to PlayerInput component
        playerInput = GetComponent<PlayerInput>();

        SetUpInputActions();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
    }

    // Set up input actions
    private void SetUpInputActions()
    {
        // Get input actions for each direction
        upAction = playerInput.actions["UpCircle"];
        downAction = playerInput.actions["DownCircle"];
        leftAction = playerInput.actions["LeftCircle"];
        rightAction = playerInput.actions["RightCircle"];
    }

    // Update inputs
    private void UpdateInputs()
    {
        // Check if each circle action was pressed or released this frame and update corresponding properties
        // Up Circle
        UpCircleClicked = upAction.WasPressedThisFrame();
        UpCircleRelease = upAction.WasReleasedThisFrame();

        // Down Circle
        DownCircleClicked = downAction.WasPressedThisFrame();
        DownCircleRelease = downAction.WasReleasedThisFrame();

        // Left Circle
        LeftCircleClicked = leftAction.WasPressedThisFrame();
        LeftCircleRelease = leftAction.WasReleasedThisFrame();

        // Right Circle
        RightCircleClicked = rightAction.WasPressedThisFrame();
        RightCircleRelease = rightAction.WasReleasedThisFrame();
    }
}
