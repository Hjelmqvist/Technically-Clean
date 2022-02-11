using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof( CinemachineVirtualCamera ) )]
public class CameraController : MonoBehaviour
{
    [Header( "Zooming" )]
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float zoomDistance = 5f;

    private float normalDistance = 10f;
    private CinemachineFramingTransposer cineCam;
    private Coroutine movingCamera;

    public void ZoomIn() => Zoom( zoomDistance );

    public void ZoomOut() => Zoom( normalDistance );

    private void Zoom(float zoomTo)
    {
        if (movingCamera != null)
        {
            StopCoroutine( movingCamera );
            movingCamera = null;
        }

        movingCamera = StartCoroutine( ZoomAnimation( zoomTo ) );
    }

    private IEnumerator ZoomAnimation(float moveTo)
    {
        float distToGo = moveTo - cineCam.m_CameraDistance;
        float movePerFrame = zoomSpeed * Time.fixedDeltaTime * (distToGo / Mathf.Abs( distToGo ));
        while (Mathf.Abs( distToGo ) > Mathf.Abs( movePerFrame ))
        {
            distToGo -= movePerFrame;
            cineCam.m_CameraDistance += movePerFrame;
            yield return new WaitForSeconds( Time.fixedDeltaTime );
        }

        movingCamera = null;
    }

    private void Awake()
    {
        cineCam = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        normalDistance = cineCam.m_CameraDistance;
    }

    private void OnEnable()
    {
        playerInteract.OnStationEnteredRange.AddListener( StationEnteredRange );
        playerInteract.OnStationExitedRange.AddListener( StationExitedRange );
    }

    private void OnDisable()
    {
        playerInteract.OnStationEnteredRange.RemoveListener( StationEnteredRange );
        playerInteract.OnStationExitedRange.RemoveListener( StationExitedRange );
    }

    public void StationEnteredRange(ChoreStation choreStation)
    {
        choreStation.OnTaskStarted.AddListener( ZoomIn );
        choreStation.OnTaskCompleted.AddListener( ZoomOut );
        choreStation.OnTaskStopped.AddListener( ZoomOut );
    }

    public void StationExitedRange(ChoreStation choreStation)
    {
        if (choreStation.ProgressOngoing)
            ZoomOut();
        choreStation.OnTaskStarted.RemoveListener( ZoomIn );
        choreStation.OnTaskCompleted.RemoveListener( ZoomOut );
        choreStation.OnTaskStopped.RemoveListener( ZoomOut );
    }
}