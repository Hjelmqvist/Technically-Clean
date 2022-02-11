using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;
using UnityEngine.Events;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class FadeToBlack : MonoBehaviour
{
    [SerializeField] private Image blackOutSquare;
    [SerializeField] private int fadeTime = 2;
    [SerializeField] private bool fadeOnStart = false;

    [SerializeField] private UnityEvent OnFadeIn;
    [SerializeField] private UnityEvent OnFadeOut;

    private void Awake()
    {
        if (fadeOnStart)
        {
            StartCoroutine( Fade( 0, fadeTime ) );
        }
    }

    public void FadeBetweenScenes(string sceneName)
    {
        StopAllCoroutines();
        StartCoroutine( Fade( 1, fadeTime, sceneName ) );
    }

    IEnumerator Fade(float targetAlpha, float fadeTime, string sceneName = null)
    {
        bool fadeToBlack = targetAlpha == 1;
        if (fadeToBlack) OnFadeOut.Invoke();
        else OnFadeIn.Invoke();

        Color startColor = blackOutSquare.color;
        Color endColor = startColor;
        endColor.a = targetAlpha;

        if (fadeToBlack == false)
        {
            startColor.a = 1;
        }

        float startTime = Mathf.Lerp( 0, fadeTime, blackOutSquare.color.a );

        for (float time = startTime; time < fadeTime; time += Time.deltaTime)
        {
            Color color = Color.Lerp( startColor, endColor, time / fadeTime );
            blackOutSquare.color = color;
            yield return null;
        }

        blackOutSquare.color = endColor;

        if (string.IsNullOrEmpty(sceneName) == false)
        {
            RuntimeManager.GetBus( "bus:/" ).stopAllEvents( STOP_MODE.IMMEDIATE );
            SceneManager.LoadScene( sceneName );
        }
    }
}