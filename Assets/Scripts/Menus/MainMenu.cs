using UnityEngine;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        RuntimeManager.GetBus("bus:/").setPaused(false);
    }

    public void StartGameNoIntro()
    {
        Time.timeScale = 1f;
        RuntimeManager.GetBus("bus:/").stopAllEvents(STOP_MODE.IMMEDIATE);
        RuntimeManager.StudioSystem.setParameterByName("TimeCounter(100%-0%)", 1f);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        RuntimeManager.GetBus("bus:/").stopAllEvents(STOP_MODE.IMMEDIATE);
        RuntimeManager.StudioSystem.setParameterByName("TimeCounter(100%-0%)", 1f);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}