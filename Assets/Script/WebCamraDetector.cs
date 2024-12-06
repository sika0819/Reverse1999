using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine.Video;
using Unity.SharpZipLib.GZip;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.UI;
public class WebCamraDetector : MonoBehaviour
{
    private UdpClient receiveClient;
    private IPEndPoint remoteIpEndPoint;
    private UdpClient udpBroadcastServer;
    private IPEndPoint broadcastEndPoint;
    public RawImage rawImage;

#if UNITY_IOS || UNITY_WEBGL
    private bool CheckPermissionAndRaiseCallbackIfGranted(UserAuthorization authenticationType, Action authenticationGrantedAction)
    {
        if (Application.HasUserAuthorization(authenticationType))
        {
            if (authenticationGrantedAction != null)
                authenticationGrantedAction();

            return true;
        }
        return false;
    }

    private IEnumerator AskForPermissionIfRequired(UserAuthorization authenticationType, Action authenticationGrantedAction)
    {
        if (!CheckPermissionAndRaiseCallbackIfGranted(authenticationType, authenticationGrantedAction))
        {
            yield return Application.RequestUserAuthorization(authenticationType);
            if (!CheckPermissionAndRaiseCallbackIfGranted(authenticationType, authenticationGrantedAction))
                Debug.LogWarning($"Permission {authenticationType} Denied");
        }
    }
#elif UNITY_ANDROID
    private void PermissionCallbacksPermissionGranted(string permissionName)
    {
        StartCoroutine(DelayedCameraInitialization());
    }

    private IEnumerator DelayedCameraInitialization()
    {
        yield return null;
        InitializeCamera();
    }

    private void PermissionCallbacksPermissionDenied(string permissionName)
    {
        Debug.LogWarning($"Permission {permissionName} Denied");
    }

    private void AskCameraPermission()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacksPermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacksPermissionGranted;
        Permission.RequestUserPermission(Permission.Camera, callbacks);
    }
#endif


    void Start()
    {

        udpBroadcastServer = new UdpClient(10086);
        broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, 9876); // 使用广播地址
        BroadCastMessage("SendIP");
#if UNITY_IOS || UNITY_WEBGL
        StartCoroutine(AskForPermissionIfRequired(UserAuthorization.WebCam, () => { InitializeCamera(); }));
        return;
#elif UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            AskCameraPermission();
            return;
        }
#endif
        InitializeCamera();

        receiveClient = new UdpClient(2333);
        receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        receiveClient.BeginReceive(OnDataReceived, receiveClient);
    }
    private void OnDataReceived(IAsyncResult result)
    {
        receiveClient = result.AsyncState as UdpClient;
        byte[] buffer = receiveClient.EndReceive(result, ref remoteIpEndPoint);
        Debug.LogFormat("Receive:{0},address:{1},port:{2}", buffer.Length, remoteIpEndPoint.Address, remoteIpEndPoint.Port);

        receiveClient.BeginReceive(OnDataReceived, receiveClient);
        isClientConnected = true;
    }
    public void BroadCastMessage(string message)
    {
        Debug.LogFormat("BroadCastMessage:{0},{1}", message, broadcastEndPoint.Port);
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpBroadcastServer.SendAsync(data, data.Length, broadcastEndPoint);
    }
    public void BroadCastMessage(byte[] data)
    {
        try
        {
            Debug.LogFormat("BroadCastMessage:{0},{1}", data.Length, broadcastEndPoint.Port);
            udpBroadcastServer.SendAsync(data, data.Length, broadcastEndPoint);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
    bool isClientConnected = false;


    Texture2D snap;
    void Update()
    {

        if (!isClientConnected)
        {
            BroadCastMessage("SendIP");
        }
        else
        {
            if (snap == null)
            {
                snap = new Texture2D(Screen.width,Screen.height);
                snap = new Texture2D(webcamTexture.width, webcamTexture.height);
            }
            
            snap.SetPixels(webcamTexture.GetPixels());
            snap.Apply();
            byte[] data = snap.EncodeToJPG();
            byte[] compressData = Compress(data);
            BroadCastMessage(compressData);
            if (rawImage != null)
            {
                rawImage.texture = snap;
            }

        }
    }
    WebCamTexture webcamTexture;
    private void InitializeCamera()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
    }


    // <summary>
    /// 压缩数据
    /// </summary>
    /// <param name="noCompressDatas">未压缩的数据</param>
    /// <returns></returns>
    public byte[] Compress(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (GZipOutputStream gzipStream = new GZipOutputStream(ms))
            {
                gzipStream.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
    }

    void OnDestroy()
    {

    }
}