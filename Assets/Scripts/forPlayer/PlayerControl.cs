using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerProperty))]
public class PlayerControl : NetworkBehaviour
{

    //玩家属性
    PlayerProperty m_PlayerProperty;
    CharacterController cc_Player;
    //跳跃高度
    public float JumpHeight=1f;
    //重力加速度
    private float Gravity=-9.8f;
    //竖直速度
    public float Vy = 0f;
    //站立高度
    private float StandHeight;
    //下蹲高度
    private float CrouchHeight;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        m_PlayerProperty = GetComponent<PlayerProperty>();//获取玩家属性
        cc_Player = GetComponent<CharacterController>();//获取角色控制器


        StandHeight = cc_Player.height;
        CrouchHeight = cc_Player.height/2;
    }

    void Update()
    {
        if (!isLocalPlayer){ return; }
        if (!m_PlayerProperty.isAlive) { return; }
        Movement();
        
    }



    private void Movement()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 dir = transform.forward * vertical + transform.right * horizontal;
        //角色平移
        cc_Player.Move(m_PlayerProperty.speed * Time.deltaTime * dir);
        //模拟重力
        Vy += Gravity * Time.deltaTime;
        cc_Player.Move(Time.deltaTime * Vy * Vector3.up);
        if (cc_Player.isGrounded)
        {
            if (Vy<-1) Vy = -1f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && cc_Player.isGrounded)
        {
            Vy = Mathf.Sqrt(-2 * Gravity * JumpHeight);//计算跳跃速度
        }
        //下蹲
        Crouch();
    }


    private void Crouch()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            cc_Player.height=Mathf.Lerp(cc_Player.height, CrouchHeight, 0.1f);//cc_Player.height /= 2;//
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            cc_Player.height =StandHeight;//cc_Player.height=Mathf.Lerp(cc_Player.height, StandHeight, 0.1f);//
        }
    }
    //触底检测
    //private void IsGrounded()
    //{
    //    //射线检测
    //    if(Physics.SphereCast(transform.position,cc_Player.radius,Vector3.down,out RaycastHit hit,2*cc_Player.skinWidth)) 
    //    { 
    //        isGrounded = true;
    //    }
    //    else
    //    {
    //        isGrounded = false;
    //    }
    //}
}
