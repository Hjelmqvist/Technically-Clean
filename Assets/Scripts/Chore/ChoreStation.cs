using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct ChoreMultiplier
{
    public ChoreType type;
    public float multi;
    public bool isDirty;
}

public class ChoreStation : MonoBehaviour
{
    [SerializeField] private int maxAmountOfContainedObjects;
    [SerializeField] private List<ChoreMultiplier> choreMultipliers;
    [SerializeField] private int timeToComplete;
    [SerializeField] private GameObject completedStackPlaceholder;
    [SerializeField] private ChoreObjectPrefabReferences choreObjectPrefabReferences;
    [SerializeField] private ChoreObjectPrefabReferences choreObjectStackPrefabReferences;
    [SerializeField] private ChoreObjectPrefabReferences dirtyChoreObjectPrefabReferences;
    [SerializeField] private bool takesClean;
    [SerializeField] private bool takesAny;
    [SerializeField] private bool takesNoChoreObjects;
    [SerializeField] private bool shouldAddScore = true;
    [SerializeField] private ChoreType choreType;
    [SerializeField] private Transform interactPosition;
    [SerializeField] private bool canCancelProgress = true;
    [SerializeField] private float popupDelay = 0.5f;
    [SerializeField] private bool canAddWhileProgressing;

    public bool canInteract;
    [SerializeField] private int amountRequiredForInteract;
    [SerializeField] private bool canInteractWhileProgressing = true;

    private List<ChoreType> containedChoreTypes = new List<ChoreType>();
    private float taskProgress = 0f;
    private bool progressOngoing;

    public Action<ChoreType, int> OnShowObject;
    public Action OnHideObject;

    public static event Action<ChoreType, int> OnScored;
    public Action<int> OnScoredIndividual;
    public UnityEvent<ChoreObject> OnChoreObjectAdded;
    public UnityEvent OnInteractableFull;
    public UnityEvent OnTaskStarted;
    public UnityEvent<float> OnTaskProgressed;
    public UnityEvent OnTaskStopped;
    public UnityEvent OnTaskCompleted;
    public UnityEvent OnGameEnd;

    public int CurrentContainedCount => containedChoreTypes.Count;
    public bool IsFull => containedChoreTypes.Count >= maxAmountOfContainedObjects;

    public bool EnoughForInteraction => canInteract && (progressOngoing == false || canInteractWhileProgressing) &&
                                        containedChoreTypes.Count >= amountRequiredForInteract;

    public bool TakesItems => maxAmountOfContainedObjects > 0;
    public bool ProgressOngoing => progressOngoing;
    public string AmountContained 
    {
        get
        {
            if(containedChoreTypes.Count < maxAmountOfContainedObjects)
                return $"{containedChoreTypes.Count} / {maxAmountOfContainedObjects}" ;
            return "Full!";
        }
    }
    public ChoreType ChoreType => choreType;
    public Transform InteractPosition => interactPosition;

    public bool CanPutAway(ChoreObject obj) =>
        obj != null && !IsFull && (takesAny || takesClean == obj.Clean) && (taskProgress == 0f || canAddWhileProgressing);

    public bool AddChoreObject(ChoreObject choreObject)
    {
        if (CanPutAway(choreObject))
        {
            OnShowObject?.Invoke(choreObject.ChoreType, containedChoreTypes.Count);
            containedChoreTypes.Add(choreObject.ChoreType);
            if (IsFull)
            {
                OnInteractableFull.Invoke();
            }

            AddScore(choreObject);

            OnChoreObjectAdded?.Invoke(choreObject);
            Destroy(choreObject.gameObject);

            return true;
        }

        return false;
    }

    public void ProgressInteract()
    {
        if (!progressOngoing)
        {
            OnTaskStarted?.Invoke();
            progressOngoing = true;
        }

        taskProgress = Mathf.Clamp01(taskProgress + Time.deltaTime / timeToComplete);
        OnTaskProgressed.Invoke(taskProgress);
        if (taskProgress == 1f)
        {
            TaskCompleted();
            OnTaskCompleted.Invoke();
        }
    }

    public bool TryProgressInteract()
    {
        if (canInteract && (progressOngoing == false || canInteractWhileProgressing) &&
            containedChoreTypes.Count >= amountRequiredForInteract)
        {
            ProgressInteract();
            return true;
        }

        return false;
    }

    public virtual void TaskCompleted()
    {
        taskProgress = 0f;
        progressOngoing = false;
        if (takesNoChoreObjects) return;

        List<ChoreType> containedChoreTypes = new List<ChoreType>( this.containedChoreTypes );
        this.containedChoreTypes.Clear();
        SpawnCleanObjects( containedChoreTypes );

        OnHideObject.Invoke();
    }

    private void SpawnCleanObjects(List<ChoreType> containedChoreTypes)
    {
        bool instantiateStack = ShouldInstantiateStack( containedChoreTypes, out ChoreType stackType );
        var prefabReferences = instantiateStack ? choreObjectStackPrefabReferences : choreObjectPrefabReferences;
        float height = 0f;
        foreach (ChoreType ct in containedChoreTypes)
        {
            ChoreType newChoreType = instantiateStack ? stackType : ct;
            ChoreObject prefab = prefabReferences.ChoreTypeToChoreObjectPrefab( newChoreType );
            if (prefab == null)
                prefab = dirtyChoreObjectPrefabReferences.ChoreTypeToChoreObjectPrefab( newChoreType );
            Vector3 newChoreObjectPosition = completedStackPlaceholder.transform.position;
            newChoreObjectPosition.y += height;
            ChoreObject co = Instantiate( prefab, newChoreObjectPosition, completedStackPlaceholder.transform.rotation );
            if (instantiateStack) break;
            height += co.Height;
        }
    }

    private bool ShouldInstantiateStack(List<ChoreType> containedChoreTypes, out ChoreType outChoreType)
    {
        bool instantiateStack = true;
        ChoreType stackType = choreType;
        foreach (ChoreType ct in containedChoreTypes)
        {
            if (ct != stackType)
            {
                instantiateStack = false;
                break;
            }
        }

        int amountInStack = completedStackPlaceholder.GetComponent<ChoreObject>().Amount;
        if (amountInStack != containedChoreTypes.Count)
        {
            instantiateStack = false;
        }

        outChoreType = stackType;
        return instantiateStack;
    }

    // Used by UnityEvent
    public void ResetTaskProgress()
    {
        taskProgress = 0f;
    }

    public void SetCanInteract(bool b) => canInteract = b;

    public void CancelProgress()
    {
        if (canCancelProgress)
        {
            OnTaskStopped?.Invoke();
            progressOngoing = false;
        }
    }

    IEnumerator AddScoreWithDelay(ChoreType choreType, int scoreToAdd)
    {
        OnScored?.Invoke(choreType, scoreToAdd);
        yield return new WaitForSecondsRealtime(popupDelay);
        OnScoredIndividual?.Invoke(scoreToAdd);
    }
    
    /// <summary>
    /// Add score based on ChoreObject
    /// </summary>
    public void AddScore(ChoreObject choreObject, bool extraPass = false, float extraMultiplier = 1)
    {
        if (!shouldAddScore && !extraPass) return;

        ChoreType choreType = choreObject.ChoreType;
        float multiplier = 1;
        foreach (ChoreMultiplier c in choreMultipliers)
        {
            if (choreType == c.type && choreObject.Clean != c.isDirty)
            {
                multiplier = c.multi;
                break;
            }
        }

        int scoreToAdd = (int) (choreObject.ScoreValue * multiplier * choreObject.Amount * extraMultiplier);
        StartCoroutine(AddScoreWithDelay(choreObject.ChoreType, scoreToAdd));
    }

    /// <summary>
    /// Add any score through inspector
    /// </summary>
    public void AddScore(int scoreToAdd)
    {
        StartCoroutine(AddScoreWithDelay(ChoreType, scoreToAdd));
    }

    private void OnEnable()
    {
        EndMenu.OnGameEnd += GameEnd;
    }

    private void OnDisable()
    {
        EndMenu.OnGameEnd -= GameEnd;
    }

    private void GameEnd()
    {
        OnGameEnd.Invoke();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}