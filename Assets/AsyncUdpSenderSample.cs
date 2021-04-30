using System.Text;
using UnityEngine;

public class AsyncUdpSenderSample : MonoBehaviour
{
    #region Field

    string message = "Message";
    string address = "127.0.0.1";
    int    port    = 22222;
    int    count   = 1;

    //float frameStart;
    //float frameEnd;

    #endregion Field

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 200, 200));

        message = GUILayout.TextField(message);
        address = GUILayout.TextField(address);
        port    = int.Parse(GUILayout.TextField(port.ToString()));
        count   = int.Parse(GUILayout.TextField(count.ToString()));

        if(GUILayout.Button("Send Message"))
        {
            //frameStart = Time.frameCount;

            for (int i = 0; i < count; i++)
            {
                AsyncUdpSender.Send(address, port, Encoding.UTF8.GetBytes(message + ":" + i));
            }

            //frameEnd = Time.frameCount;
        }

        //GUILayout.Label("Send Frame Start : " + frameStart);
        //GUILayout.Label("Send Frame End   : " + frameEnd);

        GUILayout.EndArea();
    }
}