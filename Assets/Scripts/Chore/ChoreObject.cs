using UnityEngine;

public enum ChoreType
{
    Dishes,
    Trash,
    Laundry,
    Bed,
    Phone
}

[RequireComponent(typeof( Rigidbody ) )]
public class ChoreObject : MonoBehaviour
{
    [SerializeField] Collider collider;
    [SerializeField] private ChoreType choreType;
    [SerializeField] private int maxStack = 3;
    [SerializeField] ChoreObjectSet choreObjectSet;
    [SerializeField] private int scoreValue;
    [SerializeField] private bool clean;
    [SerializeField] private int amount = 1;

    [Space(10)] [Header("X for forward strength. Y for upward strength.")]
    [SerializeField] Vector2 throwStrength;

    private Rigidbody body;

    public ChoreType ChoreType => choreType;
    public int ScoreValue => scoreValue;
    public bool Clean => clean;
    public int Amount => amount;
    public float Height { get; private set; }
    public int MaxStack => maxStack;

    public float OnInteract(Transform parent, float totalHeight = 0)
    {
        bool dropped = parent == null;

        body.isKinematic = !dropped;
        collider.enabled = dropped;
        enabled = dropped;

        if (dropped)
        {
            body.AddForce(transform.forward * throwStrength.x + Vector3.up * throwStrength.y, ForceMode.Impulse);
        }

        transform.SetParent(parent);

        if (!dropped)
        {
            transform.localPosition = Vector3.up * (totalHeight + Height / 2);
            transform.localRotation = Quaternion.identity;
        }

        return totalHeight + Height;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        Height = collider.bounds.size.y;
    }

    private void OnEnable()
    {
        if (choreObjectSet)
        {
            choreObjectSet.AddObject(this);
        }
    }

    private void OnDestroy()
    {
        if (choreObjectSet)
        {
            choreObjectSet.RemoveObject(this);
        }
    }
}