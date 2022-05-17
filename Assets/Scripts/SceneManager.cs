using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public void VitalSignsFullScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("VitalSigns");
    }

    public void VideoFullScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("VideoScene");
    }

    public void BackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("App");
    }


}
