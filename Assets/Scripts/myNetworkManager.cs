using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myNetworkManager : MonoBehaviour
{
    NetworkManager manager;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    public void ClickBuildRoomBtn()
    {
        if (!NetworkClient.active)
        {
            //当正在运行的播放程序  运行时在WebGLPlayer上的播放器
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                manager.StartHost();
                manager.networkAddress = "localhost";
            }
        }
    }

    public void ClickJoinRoomBtn()
    {
        manager.StartClient();
        manager.networkAddress = "localhost";
    }
}
