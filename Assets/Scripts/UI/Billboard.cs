using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camTransform;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        camTransform = cam.transform;
    }

    void LateUpdate()
    {
        transform.LookAt( transform.position + camTransform.forward );
    }
}