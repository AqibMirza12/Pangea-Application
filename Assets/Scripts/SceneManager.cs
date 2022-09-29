using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject[] SceneGO = new GameObject[7];
    public GameObject[] SceneGO1 = new GameObject[7];
    public GameObject vitalSigns;
    public GameObject videoRelay;
    private static bool appLan;

    public static void VitalSignsFullScreen()
    {
        Debug.Log(MindrayConnect.GetLocalIPv4());
        Debug.Log(VSCaptureMRay.MRayTCPclient.m_FrameList.Count);
        Debug.Log(VSCaptureMRay.MRayTCPclient.m_transmissionstart);
        Debug.Log(VSCaptureMRay.MRayTCPclient.m_NumericValList.Count);
        Debug.Log(VSCaptureMRay.MRayTCPclient.m_NumValHeaders.Count);
        Debug.Log(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
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
