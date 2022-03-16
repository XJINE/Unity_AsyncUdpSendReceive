using System.Net;
using System.Net.Sockets;

public static class AsyncUdpSender
{
    #region Field

    private static readonly UdpClient UdpClient;

    #endregion Field

    #region Method

    static AsyncUdpSender()
    {
        UdpClient = new UdpClient();
    }

    public static void Send(string address, int port, byte[] data)
    {
       Send(new IPEndPoint(IPAddress.Parse(address), port), data);
    }

    public static async void Send(IPEndPoint endPoint, byte[] data)
    {
        await UdpClient.SendAsync(data, data.Length, endPoint);
    }

    #endregion Method
}