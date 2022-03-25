using UnityEngine;
using UnityEngine.Events;

//Using enum to prevent multiple events
public enum InteractState
{
    Up,
    Stay,
    Down,
    None
}

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] ControlSchemes schemes;
    [SerializeField] string horizontalName = "Horizontal";
    [SerializeField] string verticalName = "Vertical";
    [SerializeField] string primaryInteractName = "PrimaryInteract";
    [SerializeField] string secondaryInteractName = "SecondaryInteract";

    Vector2 movementInput;
    string schemeId = "keymouse";

    [HideInInspector] public UnityEvent<Vector2> OnMovementInput;
    [HideInInspector] public UnityEvent<InteractState> OnPrimaryInteract;
    [HideInInspector] public UnityEvent<InteractState> OnSecondaryInteract;

    private void Update()
    {
        if (Time.timeScale <= 0f)
        {
            return;
        }
        GetMovementInput();
        OnPrimaryInteract.Invoke(GetInteractInput(primaryInteractName));
        OnSecondaryInteract.Invoke(GetInteractInput(secondaryInteractName));
    }

    private void GetMovementInput()
    {
        movementInput.x = Input.GetAxisRaw(horizontalName);
        movementInput.y = Input.GetAxisRaw(verticalName);
        OnMovementInput.Invoke(movementInput);
    }

    private InteractState GetInteractInput(string interactName)
    {
        if (Input.GetButtonDown(interactName))
        {
            bool controllerConnected = Input.GetJoystickNames().Length > 0;
            schemeId = schemes.GetId(controllerConnected);
            
            return InteractState.Down;
        }
        else if (Input.GetButton(interactName))
        {
            return InteractState.Stay;
        }
        else if (Input.GetButtonUp(interactName))
        {
            return InteractState.Up;
        }

        return InteractState.None;
    }

    public string GetInteractKey(bool primary)
    {
        if (primary)
            return schemes.GetScheme(schemeId).interact;
        return schemes.GetScheme(schemeId).throwItem;
    }
}