using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AsyncUdpReceiver : MonoBehaviour
{
    [Serializable] public class UdpReceiveEvent : UnityEvent<UdpReceiveResult> {}
    [Serializable] public class ExceptionEvent  : UnityEvent<Exception> {}

    #region Field

    [SerializeField] protected int port          = 22222;
    [SerializeField] protected int listenerCount = 100;

    public UdpReceiveEvent OnReceive;
    public ExceptionEvent  OnException;

    protected UdpClient  udpClient;
    protected List<Task> listeners;

    // NOTE:
    // To receive some UDPs in same frame, make many listeners.
    // If not, this will receive only one UDP in a same frame.

    #endregion Field

    #region Property

    public bool IsListening { get; protected set; }

    #endregion Property

    #region Method

    protected void Awake()
    {
        this.udpClient = new UdpClient(this.port);
    }

    protected void OnEnable()
    {
        if (this.IsListening)
        {
            return;
        }

        this.IsListening = true;

        this.listeners = new List<Task>();

        for (int i = 0; i < this.listenerCount; i++)
        {
            this.listeners.Add(Listen());
        }
    }

    protected void OnDisable()
    {  
        if (!this.IsListening)
        {
            return;
        }

        this.IsListening = false;

        // NOTE:
        // Need to wait current receive-task close.

        foreach (var listener in this.listeners)
        {
            if (listener != null && listener.Status == TaskStatus.Running)
            {
                listener.Wait();
                listener.Dispose();
            }
        }

        this.udpClient.Dispose();
    }

    protected async Task Listen()
    {
        while(this.IsListening)
        {
            try
            {
                // NOTE:
                // Unity will process the functions after 'await' in a same thread. This is specification.
                // To make these multi-thread, we can use 'ReceiveAsync().ConfigureAwait(false)'.
                // However, non-main thread cannont access the 'UnityEvent.Invoke'.
                // 
                // var receive = await this.udpClient.ReceiveAsync().ConfigureAwait(false);

                var receive = await this.udpClient.ReceiveAsync();

                OnReceive.Invoke(receive);

                // NOTE:
                // Time.frameCount also accessible from only a main thread.
                // 
                // Debug.Log(System.Threading.Thread.CurrentThread.ManagedThreadId);
                // Debug.Log(Time.frameCount);
            }
            catch (Exception exception)
            {
                OnException.Invoke(exception);

                // Debug.Log(exception);
            }
        }
    }

    #endregion Method
}