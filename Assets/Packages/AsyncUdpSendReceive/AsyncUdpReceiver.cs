using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AsyncUdpReceiver : MonoBehaviour
{
    [Serializable] public class UdpReceiveEvent : UnityEvent<UdpReceiveResult> {}
    [Serializable] public class ExceptionEvent  : UnityEvent<Exception> {}

    #region Field

    // NOTE:
    // To receive some UDPs in same frame, make many listeners.
    // If not, this will receive only one UDP in a same frame.

    // NOTE:
    // Default buffer size can be obtained from UdpClient.Client.ReceiveBufferSize.
    // In most cases, it is 65536 (64KB).
    
    [SerializeField] protected int port          = 22222;
    [SerializeField] protected int bufferSize    = 65536;
    [SerializeField] protected int listenerCount = 10;

    public UdpReceiveEvent onReceive;
    public ExceptionEvent  onException;

    protected UdpClient  UdpClient;
    protected List<Task> Listeners;

    #endregion Field

    #region Property

    public bool IsListening { get; protected set; }

    #endregion Property

    #region Method

    protected void Awake()
    {
        UdpClient = new UdpClient(port);
        UdpClient.Client.ReceiveBufferSize = bufferSize;
    }

    protected void OnEnable()
    {
        if (IsListening)
        {
            return;
        }

        IsListening = true;

        Listeners = new List<Task>();

        for (var i = 0; i < listenerCount; i++)
        {
            Listeners.Add(Listen());
        }
    }

    protected void OnDisable()
    {  
        if (!IsListening)
        {
            return;
        }

        IsListening = false;

        // NOTE:
        // Need to wait current receive-task close.

        foreach (var listener in Listeners.Where(listener => listener is {Status: TaskStatus.Running}))
        {
            listener.Wait();
            listener.Dispose();
        }

        UdpClient.Dispose();
    }

    protected async Task Listen()
    {
        while(IsListening)
        {
            try
            {
                // NOTE:
                // Unity will process the functions after 'await' in a same thread. This is specification.

                // NOTE:
                // To make these multi-thread, we can use 'ReceiveAsync().ConfigureAwait(false)'.
                // However, non-main thread cannot access the 'UnityEvent.Invoke'.
                // var receive = await this.udpClient.ReceiveAsync().ConfigureAwait(false);

                var receive = await UdpClient.ReceiveAsync();

                onReceive.Invoke(receive);

                // NOTE:
                // Time.frameCount also accessible from only a main thread.
                // Debug.Log(System.Threading.Thread.CurrentThread.ManagedThreadId);
                // Debug.Log(Time.frameCount);
            }
            catch (Exception exception)
            {
                onException.Invoke(exception);
                // Debug.Log(exception);
            }
        }
    }

    #endregion Method
}