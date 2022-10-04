using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public class Server : MonoBehaviour
{
    public NetworkDriver m_driver;
    private NativeList<NetworkConnection> m_Connections;

    void Start()
    {
        m_driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 4601;
        if (m_driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to port 4601");
        }
        else
        {
            m_driver.Listen();
        }

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    void OnDestroy()
    {
        m_driver.Dispose();
        m_Connections.Dispose();
    }

    void Update()
    {
        m_driver.ScheduleUpdate().Complete();
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        NetworkConnection c;
        while ((c = m_driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection");
        }

        DataStreamReader stream;
        DataStreamWriter streamwrite;

        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                continue;
            }

            NetworkEvent.Type cmd;
            while ((cmd = m_driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    uint number = stream.ReadUInt();
                    Debug.Log("Got " + number + " from the Client adding + 2 to it.");
                    number += 2;
                    var writer = m_driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out streamwrite, 1500);
                    m_driver.EndSend(streamwrite);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }


        }

    }
}
