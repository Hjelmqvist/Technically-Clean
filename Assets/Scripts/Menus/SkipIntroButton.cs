using FMODUnity;
using UnityEngine;

public class SkipIntroButton : MonoBehaviour
{
    public void SkipIntro()
    {
        Time.timeScale = 1f;
        RuntimeManager.GetBus("bus:/").setPaused(false);
    }
}