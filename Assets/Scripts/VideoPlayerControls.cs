using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerControls : MonoBehaviour
{
    public GameObject PlayButton;
    public GameObject PauseButton;
    public GameObject RewindButton;
    public GameObject ForwardButton;
    public GameObject ExitButton;

    public VideoPlayer videoPlayer;
    public VideoPlayer vitalSigns;
    private float speed = 10f;

    public Camera PiPCamera;

    public void Play()
    {
        videoPlayer.Play();
        vitalSigns.Play();
    }

    public void Pause()
    {
        videoPlayer.Pause();
        vitalSigns.Pause();
    }

    public void Rewind()
    {
        videoPlayer.time -= speed;
    }

    public void Forward()
    {
        videoPlayer.time += speed;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }


}
