using UnityEngine;
using UnityEngine.Events;

public class AutoInteractor : MonoBehaviour
{
    [SerializeField] private UnityEvent OnAutoInteract;

    private void Update() => OnAutoInteract.Invoke();
}