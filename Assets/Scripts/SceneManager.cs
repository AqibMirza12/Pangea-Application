using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject[] SceneGO = new GameObject[7];
    public GameObject[] SceneGO1 = new GameObject[7];
    public GameObject vitalSigns;
    public GameObject videoRelay;
    
    public static void VitalSignsFullScreen()
    {
        Debug.Log(MindrayConnect.GetLocalIPv4());
        MindrayConnect.GetLocalIPv4();
    }

    public void VideoFullScreen()
    {
        for (int i = 0; i < SceneGO1.Length; i++)
        {
            SceneGO1[i].gameObject.SetActive(i >= SceneGO1.Length - 1);
        }

        videoRelay.SetActive(true);
    }




}
