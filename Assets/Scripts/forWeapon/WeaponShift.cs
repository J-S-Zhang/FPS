using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(PlayerProperty))]
public class WeaponShift : NetworkBehaviour
{
    private PlayerProperty m_PlayerProperty;
    ////准心UI
    //public RawImage Crosshair;
    //子弹数量UI
    public TMP_Text UI_cur_bullet;
    public TMP_Text UI_total_bullet;

    public GameObject MainWeaponPfb;
    public GameObject CoWeaponPfb;
    public GameObject SwordPfb;
    public GameObject AmmunitionPfb;

    public GameObject MainWeapon;
    public GameObject CoWeapon;
    public GameObject Sword;
    public GameObject Ammunition;

    [SyncVar(hook =nameof(OnChangeWeapon))]
    public int curWeaponSynced;

    private Weapon weaponScript;
    //准心位置
    private Vector3 ScreenCenterPoint;
    //准心UI
    public RawImage Crosshair;
    //相机
    public Camera cam;
    //射击冷却计时器
    private float timer = 0f;

    //记录基本速度 鼠标灵敏度
    private float curspeed;
    private float curmouseSensitivity;

    private void Awake()
    {
        MainWeapon = Instantiate(MainWeaponPfb, transform.position, transform.rotation);
        MainWeapon.transform.parent = transform;
        MainWeapon.SetActive(true);
        CoWeapon = Instantiate(CoWeaponPfb, transform.position, transform.rotation);
        CoWeapon.transform.parent = transform;
        CoWeapon.SetActive(false);
        Sword=Instantiate(SwordPfb, transform.position, transform.rotation);
        Sword.transform.parent = transform;
        Sword.SetActive(false);
        Ammunition = Instantiate(AmmunitionPfb, transform.position, transform.rotation);
        Ammunition.transform.parent = transform;
        Ammunition.SetActive(false);
    }

    public override void OnStartLocalPlayer()
    {
        

        m_PlayerProperty = GetComponent<PlayerProperty>();
        //获取准心UI
        //Crosshair = GameObject.Find("Crosshair").GetComponent<RawImage>();
        //获取屏幕中心点
        ScreenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        //InitWeapons();
        InitialState();

        curspeed = m_PlayerProperty.speed;
        curmouseSensitivity = m_PlayerProperty.mouseSensitivity;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (!m_PlayerProperty.isAlive) return;
        timer -= Time.deltaTime;
        if (timer < 0f) timer = 0f;
        if (Shift()) {
            SetBulletUI();
            SetCrosshair();

            //切枪后校正速度 鼠标灵敏度 视野
            m_PlayerProperty.speed = curspeed;
            m_PlayerProperty.mouseSensitivity = curmouseSensitivity;
            cam.fieldOfView = 60f;

            timer = 0.2f; //换枪后0.2秒禁止射击  用于播放动画
        }
        if(Input.GetMouseButtonDown(0)&&timer==0f&&weaponScript.curBulletNum>0)
        {
            Fire0();
            SetBulletUI();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Fire1();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
            SetBulletUI();
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            Discard();
        }
    }


    //获取玩家持有的武器
    private void InitWeapons()
    {
        //GameObject a = Resources.Load<GameObject>("Rifle");
        //MainWeapon = Instantiate(a);

        //NetworkClient.RegisterPrefab(MainWeapon);
        //MainWeapon.transform.SetParent(transform);
        //NetworkServer.Spawn(MainWeapon);

        Transform[] myTransforms = GetComponentsInChildren<Transform>();
        foreach (var child in myTransforms)
        {
            if (child.gameObject.tag == "MainWeapon")
            {
                MainWeapon = child.gameObject;
                
                child.gameObject.SetActive(false);
            }
            if (child.gameObject.tag == "CoWeapon")
            {
                CoWeapon = child.gameObject;
                child.gameObject.SetActive(false);
            }
            if (child.gameObject.tag == "Sword")
            {
                Sword = child.gameObject;
                child.gameObject.SetActive(false);
            }
            if (child.gameObject.tag == "Ammunition")
            {
                Ammunition = child.gameObject;
                child.gameObject.SetActive(false);
            }
        }
    }
    //设置初始持枪状态   ......
    private void InitialState()
    {
        if (MainWeapon != null)
        {
            MainWeapon.SetActive(true);
            weaponScript = MainWeapon.GetComponent<Weapon>();
            if(weaponScript.WeaponName=="Rifle") Crosshair.enabled = false;
            //...播放持枪动画

        }
        else if (CoWeapon != null)
        {
            CoWeapon.SetActive(true);
            weaponScript = CoWeapon.GetComponent<Weapon>();

        }
        else if (Sword != null)
        {
            Sword.SetActive(true);
            weaponScript = Sword.GetComponent<Weapon>();
            Debug.Log("获取刀");
        }
        else if (Ammunition != null)
        {
            Ammunition.SetActive(true);
            weaponScript = Ammunition.GetComponent<Weapon>();
        }
        if(weaponScript == null)
        {
            Debug.Log("武器脚本为空");
        }
        else timer = weaponScript.CollingTime;

        SetCrosshair();
        SetBulletUI();
    }
    //切换武器     .......
    private bool Shift()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (curWeaponSynced != 0 && MainWeapon != null) { 
                CmdGetActiveWeapon(0);
                return true;
            }
            return false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (curWeaponSynced != 1 && CoWeapon != null) {
                CmdGetActiveWeapon(1);
               
                return true;
            }
            return false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (curWeaponSynced != 2 && Sword != null) {
                CmdGetActiveWeapon(2);
                
                return true;
            }
            return false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (curWeaponSynced != 3 && Ammunition != null) {;
                CmdGetActiveWeapon(3);
                
                return true;
            }
            return false;
        }
        return false;
    }

    //切换武器获取准信
    private void SetCrosshair()
    {
        if (weaponScript != null&& weaponScript.texture != null)
        {
            Crosshair.texture = weaponScript.texture;
            Crosshair.SetNativeSize();
            Crosshair.color = weaponScript.color;
            if (weaponScript.WeaponName == "Rifle") Crosshair.enabled = false; 
            else Crosshair.enabled=true;
        }
    }
    //子弹数量UI显示
    private void SetBulletUI()
    {
        UI_cur_bullet.text = weaponScript.curBulletNum.ToString();
        UI_total_bullet.text=weaponScript.TotalSize.ToString();
    }
    
    private void Fire0()
    {
        //开枪音效
        if(weaponScript.m_AudioClip_Fire0!=null) weaponScript.m_AudioSource.PlayOneShot(weaponScript.m_AudioClip_Fire0);
        if(weaponScript.tag == "Ammunition")
        {
            return;
        }
        //射击武器处理
        if (Physics.Raycast(cam.ScreenPointToRay(ScreenCenterPoint), out RaycastHit hit, weaponScript.FiringRange))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                //使玩家掉血
                CmdHurtOthers(hit.collider.gameObject,weaponScript.Damage);
            }
        }

        weaponScript.curBulletNum--;
        timer = weaponScript.CollingTime;
    }

    private void Fire1()
    {
        //主武器为狙击枪
        if (weaponScript.WeaponName == "Rifle") {
            CameraScale();
            return; 
        }
        Crosshair.enabled = true;
        if (weaponScript.m_AudioClip_Fire1 != null)
        {
            weaponScript.m_AudioSource.PlayOneShot(weaponScript.m_AudioClip_Fire1);
        }
    }
    //换弹......
    private void Reload()
    {
        if (weaponScript.curBulletNum == weaponScript.MagazineSize) return;
        if(weaponScript.TotalSize==0) return;
        int neednum = weaponScript.MagazineSize - weaponScript.curBulletNum;//所需补充子弹数
        if (weaponScript.TotalSize >= neednum)
        {
            //播放装弹动画  音效......
            weaponScript.TotalSize -= neednum;
            weaponScript.curBulletNum += neednum;
        }
        else
        {
            //播放装弹动画  音效......
            weaponScript.curBulletNum += weaponScript.TotalSize;
            weaponScript.TotalSize = 0;
        }
    }

    //丢枪
    private void Discard()
    {
        switch (curWeaponSynced)
        {
            case 0://丢主武器
                //当前武器脱离父子关系

                //切武器
                if(CoWeapon!=null)
                {
                    CmdGetActiveWeapon(1);
                }
                else CmdGetActiveWeapon(2);

                CmdDiscard(MainWeapon);
                break;
            case 1://丢副武器
                if (MainWeapon != null)
                {
                    CmdGetActiveWeapon(0);
                }
                else CmdGetActiveWeapon(2);
                CmdDiscard(CoWeapon);
                break;
        }
    }

    //枪 脱离
    [Command(requiresAuthority = false)]
    public void CmdDiscard(GameObject gameObject)
    {
        RpcDiscard(gameObject);
    }
    [ClientRpc]
    public void RpcDiscard(GameObject gameObject)
    {
        //gameObject.SetActive(true);
        gameObject.transform.parent = null;
        MainWeapon = null;
        //脱离所有子物体
        //transform.DetachChildren();
    }

    //镜头瞄准
    private void CameraScale()
    {
        Crosshair.enabled = !Crosshair.enabled;
        if(Crosshair.enabled)
        {
            cam.fieldOfView = 10f; //Mathf.Lerp(cam.fieldOfView, 10f, 0.1f);//
            m_PlayerProperty.mouseSensitivity /= 8;
            m_PlayerProperty.speed /= 3;
        }
        else
        {
            cam.fieldOfView = 60f;//Mathf.Lerp(cam.fieldOfView, 60f, 0.1f);// 
            m_PlayerProperty.mouseSensitivity *= 8;
            m_PlayerProperty.speed *= 3;
        }
    }
    //玩家掉血由服务端处理
    [Command]
    public void CmdHurtOthers(GameObject obj,int damage)
    {
        obj.GetComponent<PlayerProperty>().GetHurt(damage);
    }

    [Command]
    private void CmdGetActiveWeapon(int index)
    {
        curWeaponSynced = index;
    }

    private void OnChangeWeapon(int oldIndex,int newIndex)
    {
        switch (oldIndex)
        {
            case 0:MainWeapon.SetActive(false); break;
            case 1:CoWeapon.SetActive(false); break;
            case 2:Sword.SetActive(false); break;
            case 3:Ammunition.SetActive(false); break;
        }
        switch(newIndex)
        {
            case 0: MainWeapon.SetActive(true);
                weaponScript = MainWeapon.GetComponent<Weapon>();
                break; 
            case 1: CoWeapon.SetActive(true);
                weaponScript = CoWeapon.GetComponent<Weapon>();
                break;
            case 2: Sword.SetActive(true); 
                weaponScript = Sword.GetComponent<Weapon>();
                break;
            case 3: Ammunition.SetActive(true); 
                weaponScript = Ammunition.GetComponent<Weapon>();
                break;
        }
    }


}
