using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System;
using System.Linq;
using Unity.Services.Core;
using System.Threading;

public class SceneManager : MonoBehaviour
{
    public GameObject[] SceneGO = new GameObject[7];
    public GameObject[] SceneGO1 = new GameObject[7];
    public GameObject vitalSigns;
    public GameObject videoRelay;

    public static async Task InitializeAsync(InitializationOptions options)
    {
        await Task.Yield();
    }

    public static async void CallEncyption(InitializationOptions options)
    {
        await InitializeAsync(options);
        await Encryption.AllocateRelayServerAndGetJoinCode(16);
    }

    public void Testing()
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
