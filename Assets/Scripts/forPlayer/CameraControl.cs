using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerProperty))]
public class CameraControl : NetworkBehaviour
{
    //玩家属性
    PlayerProperty m_PlayerProperty;

    public GameObject cam;

    private GameObject[] otherPlayer;
    public Camera[] otherCamera;

    public Camera myCamera;

    private float mouseX, mouseY;
    private float xRotation;

    public override void OnStartLocalPlayer() { 
        base.OnStartLocalPlayer();

        m_PlayerProperty = GetComponent<PlayerProperty>();//获取玩家属性
        cam.SetActive(true);


        GameObject[] otherPlayers = GameObject.FindGameObjectsWithTag("Player");
        //获取其他玩家相机对象，并向其他玩家注册
        for (int i = 0; i < otherPlayers.Length; i++)
        {
            otherPlayers[i].GetComponent<CameraControl>().CmdCameraGetAndSet();
        }
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer) { return; }
        if (!m_PlayerProperty.isAlive) { return; }
        
        CameraController();
    }
    //相机旋转
    private void CameraController()
    {
        mouseX = Input.GetAxis("Mouse X") * m_PlayerProperty.mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * m_PlayerProperty.mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f);
        transform.Rotate(Vector3.up * mouseX);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    [Command(requiresAuthority = false)]
    public void CmdCameraGetAndSet()
    {
        RpcCameraGetAndSet();
    }
    [ClientRpc]
    public void RpcCameraGetAndSet()
    {
        CameraGetAndSet();
    }

    //可死亡后在获取摄像头

    public void CameraGetAndSet()
    {
        Debug.Log("调用CameraGetAndSet");

        //相机
        myCamera = GetComponentInChildren<Camera>();

        //获取其他玩家相机
        otherPlayer = GameObject.FindGameObjectsWithTag("Player");
        otherCamera = new Camera[otherPlayer.Length];

        Debug.Log("找到玩家数量：" + otherPlayer.Length);


        for (int i = 0; i < otherPlayer.Length; i++)
        {
            otherCamera[i] = otherPlayer[i].transform.Find("Camera").GetComponent<Camera>();
        }
    }
}
