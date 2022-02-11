using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuWindow;
    [SerializeField] private List<GameObject> childrenOfPause;
    private bool isPaused;
    private bool ended;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown( KeyCode.Escape ))
        {
            if (isPaused)
            {
                DisableChildren();
            }

            isPaused = !isPaused;
            PauseOrResume( isPaused );
        }
    }

    // Used to disable child objects and activate the pause menu so that it's ready for next time :)
    private void DisableChildren()
    {
        bool haveSetFalse = false;
        foreach (GameObject child in childrenOfPause)
        {
            if (child.activeInHierarchy)
            {
                haveSetFalse = true;
            }

            child.SetActive( false );
        }

        if (haveSetFalse)
        {
            pauseMenuWindow.SetActive( true );
            return;
        }
    }

    public void PauseOrResume(bool isPaused)
    {
        if (ended) return;

        pauseMenuWindow.SetActive( isPaused );
        RuntimeManager.GetBus( "bus:/FakeMasterBus" ).setPaused( isPaused );
        this.isPaused = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void PauseForEndMenu(bool isPaused)
    {
        ended = true;
        RuntimeManager.GetBus( "bus:/" ).stopAllEvents( STOP_MODE.IMMEDIATE );
        this.isPaused = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ExitToMainMenu()
    {
        this.isPaused = false;
        RuntimeManager.GetBus( "bus:/" ).setPaused( false );
        Time.timeScale = 1f;
    }
}