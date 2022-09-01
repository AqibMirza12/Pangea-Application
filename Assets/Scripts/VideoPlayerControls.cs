using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerControls : MonoBehaviour
{
    public GameObject ExitButton;
    public VideoPlayer vitalSigns;

    public Camera PiPCamera;

    public void Exit()
    {
        Application.Quit();
    }

    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }


}
