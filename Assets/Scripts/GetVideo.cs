using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using System.Collections;

public class GetVideo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequest("https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/1080/Big_Buck_Bunny_1080_10s_1MB.mp4"));
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest request = UnityWebRequest.Get(uri);

        yield return request.SendWebRequest();

        //VideoPlayer vp = new VideoPlayer();
    }
}
