using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInputs))]
public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform holdTransform;
    [SerializeField] private Transform throwTransform;
    [SerializeField] private bool canMixHeldItems;

    private PlayerInputs playerInputs;
    private List<ChoreObject> choreObjectsInRange = new List<ChoreObject>();
    private List<ChoreObject> heldChoreObjects = new List<ChoreObject>();
    private (ChoreObject obj, int index) closestChoreObject;
    private ChoreType heldChoreType;
    private bool heldClean;
    private float heldHeight;
    bool placedChoreObjectThisPress;

    private List<ChoreStation> choreInteractablesInRange = new List<ChoreStation>();
    private ChoreStation closestChoreStation;
    bool isInteracting = false;

    InteractState lastState = InteractState.None;

    public bool IsCarrying => heldChoreObjects.Count > 0;

    [HideInInspector] public UnityEvent<ChoreStation> OnStationEnteredRange;
    [HideInInspector] public UnityEvent<ChoreStation> OnStationExitedRange;
    [HideInInspector] public UnityEvent<ChoreStation> OnInteractStart;
    [HideInInspector] public UnityEvent OnInteractStop;
    [HideInInspector] public UnityEvent OnPickUp;
    [HideInInspector] public UnityEvent OnDrop;

    private void Awake()
    {
        playerInputs = GetComponent<PlayerInputs>();
    }

    private void OnEnable()
    {
        playerInputs.OnPrimaryInteract.AddListener( PrimaryInteract );
        playerInputs.OnSecondaryInteract.AddListener( SecondaryInteract );
    }

    private void OnDisable()
    {
        playerInputs.OnPrimaryInteract.RemoveListener( PrimaryInteract );
        playerInputs.OnSecondaryInteract.RemoveListener( SecondaryInteract );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent( out ChoreObject choreObject ))
        {
            choreObjectsInRange.Add( choreObject );
            SetClosest();
        }

        if (other.TryGetComponent( out ChoreStation choreStation ))
        {
            choreInteractablesInRange.Add( choreStation );
            SetClosest();
            OnStationEnteredRange.Invoke( choreStation );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent( out ChoreObject choreObject ))
        {
            choreObjectsInRange.Remove( choreObject );
            SetClosest();
        }

        if (other.TryGetComponent( out ChoreStation choreStation ))
        {
            choreInteractablesInRange.Remove( choreStation );
            SetClosest();
            OnStationExitedRange.Invoke( choreStation );
        }
    }

    private void Update()
    {
        // To prevent the player animation from keeping going when completing chore station task
        if (isInteracting == false)
        {
            OnInteractStop.Invoke();
        }
        else
        {
            isInteracting = false;
        }
    }

    public void PrimaryInteract(InteractState interactState)
    {
        switch (interactState)
        {
            case InteractState.Up:
                if (lastState == InteractState.Stay)
                {
                    TryCancelProgress();
                }
                placedChoreObjectThisPress = false;
                break;

            case InteractState.Stay:
                if (placedChoreObjectThisPress == false)
                {
                    isInteracting = TryHoldInteract( out ChoreStation choreStation );
                    if (isInteracting && lastState != InteractState.Stay)
                    {
                        OnInteractStart.Invoke( choreStation );
                    }
                }
                break;

            case InteractState.Down:
                bool pickedUp = TryPickUpClosestChoreObject();
                if (pickedUp == false && TryInteractWithClosestInteractable())
                {
                    placedChoreObjectThisPress = true;
                }
                break;
        }

        lastState = interactState;
    }

    private void TryCancelProgress()
    {
        if (closestChoreStation != null)
        {
            closestChoreStation.CancelProgress();
        }
    }

    private bool TryHoldInteract(out ChoreStation choreStation)
    {
        if (heldChoreObjects.Count > 0)
        {
            choreStation = null;
            return false;
        }

        choreStation = closestChoreStation;
        return choreStation != null && choreStation.TryProgressInteract();
    }

    private bool TryPickUpClosestChoreObject()
    {
        if (choreObjectsInRange.Count > 0 &&
            (heldChoreObjects.Count == 0 || heldChoreObjects.Count < heldChoreObjects[0].MaxStack))
        {
            if (closestChoreObject.obj != null)
            {
                choreObjectsInRange.RemoveAt( closestChoreObject.index );
                heldChoreObjects.Add( closestChoreObject.obj );
                heldChoreType = closestChoreObject.obj.ChoreType;
                heldClean = closestChoreObject.obj.Clean;
                heldHeight = closestChoreObject.obj.OnInteract( holdTransform, heldHeight );
                OnPickUp.Invoke();
                SetClosest();
                return true;
            }
        }
        return false;
    }

    private bool TryInteractWithClosestInteractable()
    {
        if (heldChoreObjects.Count > 0 && choreInteractablesInRange.Count > 0)
        {
            int index = heldChoreObjects.Count - 1;
            ChoreObject choreObject = heldChoreObjects[index];

            if (closestChoreStation.AddChoreObject( choreObject ))
            {
                heldHeight -= choreObject.Height;
                heldChoreObjects.RemoveAt( index );
                SetClosest();
                return true;
            }
        }
        return false;
    }

    private void SetClosest()
    {
        if (heldChoreObjects.Count > 0 && canMixHeldItems == false)
            closestChoreObject.obj =
                FindClosestChoreObjectOfType( choreObjectsInRange, out closestChoreObject.index );
        else
            closestChoreObject.obj = FindClosest( choreObjectsInRange, out closestChoreObject.index );

        closestChoreStation = FindClosest( choreInteractablesInRange, out _ );

        MonoBehaviour closest = closestChoreObject.obj;
        if (closest == null) closest = closestChoreStation;

        if (closest != null)
        {
            HighlightEffect highlight = closest.GetComponent<HighlightEffect>();
            CustomHighlightManager.SetSelected( highlight );

            InteractText interactText = closest.GetComponent<InteractText>();
            int index = heldChoreObjects.Count - 1;
            ChoreObject co = index >= 0 ? heldChoreObjects[index] : null;
            InteractText.SetSelected( interactText, co );
            return;
        }

        InteractText.SetSelected();
        CustomHighlightManager.SetSelected( null );
    }

    private T FindClosest<T>(List<T> list, out int index) where T : MonoBehaviour
    {
        T closest = null;
        float closestDistance = float.MaxValue;
        index = -1;

        for (int i = 0; i < list.Count; i++)
        {
            T current = list[i];
            if (current == null) continue;

            float distance = Vector3.Distance( current.transform.position, transform.position );
            if (closest == null || distance < closestDistance)
            {
                closest = current;
                closestDistance = distance;
                index = i;
            }
        }
        return closest;
    }

    private ChoreObject FindClosestChoreObjectOfType(List<ChoreObject> list, out int index)
    {
        ChoreObject closest = null;
        float closestDistance = float.MaxValue;
        index = -1;

        for (int i = 0; i < list.Count; i++)
        {
            ChoreObject current = list[i];
            if (current == null) continue;
            if (heldChoreObjects.Count != 0 &&
                heldChoreObjects.Count + current.Amount > heldChoreObjects[0].MaxStack) continue;

            if (heldChoreType == current.ChoreType && heldClean == current.Clean)
            {
                float distance = Vector3.Distance( current.transform.position, transform.position );
                if (closest == null || distance < closestDistance)
                {
                    closest = current;
                    closestDistance = distance;
                    index = i;
                }
            }
        }
        return closest;
    }

    public void SecondaryInteract(InteractState interactState)
    {
        switch (interactState)
        {
            case InteractState.Down:
                TryDropChoreObject();
                break;
        }
    }

    private bool TryDropChoreObject()
    {
        if (heldChoreObjects.Count > 0)
        {
            int index = heldChoreObjects.Count - 1;
            ChoreObject topOfStack = heldChoreObjects[index];
            topOfStack.OnInteract( throwTransform );
            heldHeight -= topOfStack.OnInteract( null );
            heldChoreObjects.RemoveAt( index );
            SetClosest();
            OnDrop.Invoke();
            return true;
        }
        return false;
    }
}