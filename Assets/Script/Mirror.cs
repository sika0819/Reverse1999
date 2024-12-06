using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using Unity.SharpZipLib.GZip;
public class Mirror : MonoBehaviour
{
    private UdpClient receiveClient;
    private UdpClient sendClient;
    private IPEndPoint remoteIpEndPoint;
    RawImage rawImage;
    void Start()
    {
        rawImage = GetComponent<RawImage>();

        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 9876);
        receiveClient = new UdpClient(9876);
        receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        receiveClient.EnableBroadcast = true;
        receiveClient.BeginReceive(OnDataReceived, receiveClient);

    }
    void Update()
    {
        if (buffer != null && buffer.Length > 0 && isReceived)
        {
            Debug.LogFormat("Receive:{0}", buffer.Length);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(buffer);
            rawImage.texture = tex;
            isReceived = false;
        }
    }
    byte[] buffer;
    bool isReceived = false;
    private void OnDataReceived(IAsyncResult result)
    {
        receiveClient = result.AsyncState as UdpClient;
        byte[] dataBuff = receiveClient.EndReceive(result, ref remoteIpEndPoint);
        Debug.LogFormat("Receive:{0},address:{1},port:{2}", dataBuff.Length, remoteIpEndPoint.Address, remoteIpEndPoint.Port);
        if (sendClient == null)
        {
            sendClient = new UdpClient(remoteIpEndPoint);
        }
        receiveClient.BeginReceive(OnDataReceived, receiveClient);
        Send("Received");
        byte[] data =  Decompress(dataBuff);
        buffer = new byte[data.Length];
        Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
        isReceived = true;
    }
    public void Send(string message)
    {
        if (sendClient != null)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            IPEndPoint endpoint = new IPEndPoint(remoteIpEndPoint.Address, 2333);
            Debug.LogFormat("SendMessage:{0},{1}", message, endpoint.Port);
            sendClient.Send(data, data.Length, endpoint);
        }
    }


    void OnDestory()
    {
        receiveClient.Close();
    }
    // 解压缩数据
    public byte[] Decompress(byte[] compressedData)
    {
        Debug.Log(compressedData.Length);
        using (MemoryStream ms = new MemoryStream(compressedData))
        {
            using (GZipInputStream gzipStream = new GZipInputStream(ms))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    byte[] buffer = new byte[4096];
                    int read;
                    while ((read = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outStream.Write(buffer, 0, read);
                    }
                    return outStream.ToArray();
                }
            }
        }
    }
}
