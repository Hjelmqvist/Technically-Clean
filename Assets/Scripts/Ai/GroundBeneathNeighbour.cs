using UnityEngine;

public class GroundBeneathNeighbour : MonoBehaviour
{
    [SerializeField] private NeighbourController neighbourCtrler;

    private void OnCollisionEnter(Collision other) => neighbourCtrler.GroundCollision(other);
}
