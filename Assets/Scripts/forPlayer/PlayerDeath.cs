using Mirror;
using UnityEngine;
[RequireComponent(typeof(PlayerProperty))]
public class PlayerDeath : NetworkBehaviour
{

    PlayerProperty m_PlayerProperty;
    CameraControl m_CameraControl;


    public override void OnStartLocalPlayer()
    {
        m_PlayerProperty = GetComponent<PlayerProperty>();
        m_CameraControl=GetComponent<CameraControl>();

        
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (m_PlayerProperty.isAlive) { return; }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("切换视角");
            Spectator(); 
        }
    }



    public void Spectator()
    {
        //禁用当前相机
        m_CameraControl.myCamera.gameObject.SetActive(false);
        //激活旁观者相机
        if(m_CameraControl.otherCamera.Length > 0)
        {
            m_CameraControl.otherCamera[0].gameObject.SetActive(true);
        }
    }
}
