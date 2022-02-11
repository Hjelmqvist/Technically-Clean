using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputs), typeof(PlayerInteract))]
public class PlayerController : MonoBehaviour
{
    public bool Walking { get; private set; }

    [SerializeField] private float walkSpeed = 6.66f;
    [SerializeField, Range(0, 1)] private float turnSpeed = .2f;
    [SerializeField] private float accelerationTime = .666f;
    [SerializeField] private float decelerationTime = .42f;

    private CharacterController playerController;
    private PlayerInputs playerInputs;
    private PlayerInteract playerInteract;
    private Vector3 walkDirection;
    bool canWalk = true;
    private float stopWalkTime;
    private float startWalkTime;
    private Vector3 inputDir;
    private float currWalkSpeed;

    public void SetWalkDirection(Vector2 movementInput)
    {
        Vector3 newInputDir = new Vector3(movementInput.x, 0f, movementInput.y);

        if (newInputDir.magnitude == 0f && inputDir.magnitude > 0f) stopWalkTime = Time.time;
        if (newInputDir.magnitude > 0f && inputDir.magnitude == 0f) startWalkTime = Time.time;
        Walking = newInputDir.magnitude > 0f;
        inputDir = newInputDir;
    }

    public void LockPosition(ChoreStation choreStation)
    {
        Transform interactPosition = choreStation.InteractPosition;
        if (interactPosition)
        {
            transform.position = interactPosition.position;
            transform.rotation = interactPosition.rotation;
        }

        canWalk = false;
    }

    public void UnlockPosition()
    {
        canWalk = true;
    }

    private void FixedUpdate()
    {
        Walk();
        Gravity();
    }

    private void Walk()
    {
        Vector3 walkVector;

        if (canWalk && Walking)
        {
            walkVector = Accelerate();
            Rotate();
        }
        else
        {
            walkVector = Decelerate();
        }

        playerController.Move(walkVector);
    }

    private Vector3 Accelerate()
    {
        Vector3 walkVector;
        walkDirection = inputDir.normalized;
        float speedPercent = Mathf.Clamp01( (Time.time - startWalkTime) / accelerationTime );
        currWalkSpeed = Mathf.Lerp( currWalkSpeed, walkSpeed, speedPercent );
        walkVector = walkDirection * currWalkSpeed * Time.fixedDeltaTime;
        return walkVector;
    }

    private Vector3 Decelerate()
    {
        Vector3 walkVector;
        float speedPercent = Mathf.Clamp01( (Time.time - stopWalkTime) / decelerationTime );
        currWalkSpeed = Mathf.Lerp( currWalkSpeed, 0f, speedPercent );
        walkVector = walkDirection * currWalkSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.LookRotation( walkDirection, Vector3.up );
        return walkVector;
    }

    private void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation( walkDirection, Vector3.up );
        transform.rotation = Quaternion.Lerp( transform.rotation, targetRotation, turnSpeed );
    }

    private void Gravity()
    {
        playerController.Move(Physics.gravity);
    }

    private void Awake()
    {
        playerController = GetComponent<CharacterController>();
        playerInputs = GetComponent<PlayerInputs>();
        playerInteract = GetComponent<PlayerInteract>();
    }

    private void OnEnable()
    {
        playerInputs.OnMovementInput.AddListener( SetWalkDirection );
        playerInteract.OnInteractStart.AddListener( LockPosition );
        playerInteract.OnInteractStop.AddListener( UnlockPosition );
    }

    private void OnDisable()
    {
        playerInputs.OnMovementInput.RemoveListener( SetWalkDirection );
        playerInteract.OnInteractStart.RemoveListener( LockPosition );
        playerInteract.OnInteractStop.RemoveListener( UnlockPosition );
    }
}