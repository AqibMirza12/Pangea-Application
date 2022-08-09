using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLink : MonoBehaviour
{
    public GameObject vitalSigns;

    public void BackButton()
    {
        vitalSigns.SetActive(false);

        for (int i = 0; i < FindObjectOfType<SceneManager>().SceneGO.Length; i++)
        {
            FindObjectOfType<SceneManager>().SceneGO[i].gameObject.SetActive(true);
        }
    }

    //public void VideoBackButton()
    //{
    //    videoRelay.SetActive(false);

    //    for (int i = 0; i < FindObjectOfType<SceneManager>().SceneGO.Length; i++)
    //    {
    //        FindObjectOfType<SceneManager>().SceneGO[i].gameObject.SetActive(true);
    //    }
    //}


}
