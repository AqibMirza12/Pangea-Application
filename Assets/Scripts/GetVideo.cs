using UnityEngine;
using UnityEngine.Networking;

public class GetVideo : MonoBehaviour
{

    public UnityWebRequest uwr;
    //public VLCWrapper VLCinstance;

    void Start()
    {
        //VLCWrapper.Test.Run();
        RTSPRequest();
    }

    public void RTSPRequest()
    {
        uwr.url = "rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mp4";
    }
}
