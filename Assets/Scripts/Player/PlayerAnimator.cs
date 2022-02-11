using UnityEngine;

[RequireComponent( typeof( PlayerInteract ) )]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerController playerController;
    [SerializeField] ChoreTypeAnimation[] choreTypeAnimations;
    [SerializeField] private string walkingBoolName = "IsWalking";
    [SerializeField] private string carryingBoolName = "IsCarrying";
    [SerializeField] private string interactingBoolName = "IsInteracting";
    [SerializeField] private string phoneHangupAnimation = "HangUp";
    [SerializeField] private string pickupAnimation = "PickUp";
    private PlayerInteract playerInteract;

    [System.Serializable]
    public struct ChoreTypeAnimation
    {
        [SerializeField] ChoreType choreType;
        [SerializeField] string animation;

        public bool TryGetAnimationName(ChoreType choreType, out string animationName)
        {
            animationName = "";
            if (this.choreType == choreType)
            {
                animationName = animation;
                return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        playerInteract = GetComponent<PlayerInteract>();
    }

    private void Update()
    {
        animator.SetBool( walkingBoolName, playerController.Walking );
        animator.SetBool( carryingBoolName, playerInteract.IsCarrying );
    }

    public void OnInteractionStart(ChoreStation choreStation)
    {
        animator.SetBool( interactingBoolName, true );

        foreach (var choreTypeAnimation in choreTypeAnimations)
        {
            if (choreTypeAnimation.TryGetAnimationName( choreStation.ChoreType, out string animation ))
            {
                animator.SetTrigger( animation );
            }
        }
    }

    public void OnInteractionEnd()
    {
        animator.SetBool( interactingBoolName, false );
    }

    public void PlayAnimation(string animation)
    {
        animator.SetTrigger( animation );
    }

    private void OnEnable()
    {
        LandlineTelephone.OnHangup += LandlineTelephone_OnHangup;
        playerInteract.OnInteractStart.AddListener( OnInteractionStart );
        playerInteract.OnInteractStop.AddListener( OnInteractionEnd );
        playerInteract.OnPickUp.AddListener( PlayPickupAnimation );
    }

    private void OnDisable()
    {
        LandlineTelephone.OnHangup -= LandlineTelephone_OnHangup;
        playerInteract.OnInteractStart.RemoveListener( OnInteractionStart );
        playerInteract.OnInteractStop.RemoveListener( OnInteractionEnd );
        playerInteract.OnPickUp.RemoveListener( PlayPickupAnimation );
    }

    private void LandlineTelephone_OnHangup()
    {
        PlayAnimation( phoneHangupAnimation );
    }

    private void PlayPickupAnimation()
    {
        PlayAnimation( pickupAnimation );
    }
}