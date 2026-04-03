using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperty : NetworkBehaviour
{
    CameraControl m_CameraControl;

    public string playername;
    private float reborn_time = 5f;
    public const int max_HP = 100;
    //血量同步
    [SyncVar(hook =nameof(HP_slider_change))]
    public int cur_HP = max_HP;
    public float speed = 6;
    public float mouseSensitivity = 1000;

    [SyncVar]
    public bool isAlive = true;

    public Slider hp_slider;

    public float timer = 0f;

    //
    public Vector3 RespawnPosition;
    public override void OnStartLocalPlayer()
    {
        m_CameraControl = GetComponent<CameraControl>();

        RespawnPosition = transform.position;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (isAlive) { return; }
        timer += Time.deltaTime;

        if(timer >= reborn_time)
        {
            timer = 0f;
            CmdRespawn();
            CameraComeback();

            return;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("切换视角");
            Spectator();
        }
    }

    public void GetHurt(int damage)
    {
        cur_HP -= damage;
        if (cur_HP <= 0) { 
            cur_HP = 0;
            isAlive = false;
        }
    }
    //重生 状态 血量
    [Command]
    public void CmdRespawn()
    {
        isAlive = true;
        cur_HP = max_HP;
        RpcRespawn();
    }
    [ClientRpc]
    public void RpcRespawn()
    {
        transform.position = RespawnPosition;
    }
    //状态改变时触发
    public void IsAlive_change()
    {
        //复活 切换摄像机
        if (isAlive)
        {
            for(int i=0;i< m_CameraControl.otherCamera.Length; i++)
            {
                m_CameraControl.otherCamera[0].gameObject.SetActive(false);
            }
            m_CameraControl.myCamera.gameObject.SetActive(true);
        }
        //死亡 n秒后复活
        else
        {
            Invoke(nameof(CmdRespawn), reborn_time);
        }
    }

    public void HP_slider_change(int oldhp,int newhp)
    {
        hp_slider.value = newhp/(float)max_HP;
    }
    //旁观视角
    public void Spectator()
    {
        //禁用当前相机
        m_CameraControl.myCamera.gameObject.SetActive(false);
        //激活旁观者相机
        if (m_CameraControl.otherCamera.Length > 0)
        {
            m_CameraControl.otherCamera[0].gameObject.SetActive(true);
        }
    }

    //自身视角
    public void CameraComeback()
    {
        for (int i = 0; i < m_CameraControl.otherCamera.Length; i++)
        {
            m_CameraControl.otherCamera[0].gameObject.SetActive(false);
        }
        m_CameraControl.myCamera.gameObject.SetActive(true);
    }
}

