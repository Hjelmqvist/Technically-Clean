using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerEvent : MonoBehaviour
{
    [SerializeField] UnityEvent OnVideoEnded;
    VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();       
    }

    private void OnEnable()
    {
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= VideoPlayer_loopPointReached;
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        OnVideoEnded.Invoke();
    }
}