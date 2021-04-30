using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class AsyncUdpReceiverSample : MonoBehaviour
{
    List<UdpReceiveResult> receives = new List<UdpReceiveResult>();

    int count = 0;

    public void OnReceive(UdpReceiveResult result)
    {
        receives.Insert(0, result);

        if (receives.Count >= 21)
        {
            receives.RemoveAt(20);
        }

        count++;
    }

    void OnGUI()
    {
        GUILayout.Label("Receives : " + count);

        for (int i = receives.Count - 1; i >= 0; i--)
        {
            GUILayout.Label(i + " : " + receives[i].RemoteEndPoint.Address
                              + " : " + Encoding.UTF8.GetString(receives[i].Buffer));
        }
    }
}