using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public string WeaponName;
    //弹夹容量
    public int TotalSize = 45;
    public int MagazineSize = 15;
    public int curBulletNum = 15;
    //伤害
    public int Damage = 60;
    public int CriticalDamage = 100;
    //射距
    public float FiringRange = 100;
    //冷却时间
    public float CollingTime = 1f;
    //准心贴图
    public Texture texture;
    //准心颜色
    public Color color;
    //音频源
    public AudioSource m_AudioSource;
    //音频片段
    [Space]
    [SerializeField] public AudioClip m_AudioClip_Fire0;
    [SerializeField] public AudioClip m_AudioClip_Fire1;
    [SerializeField] public AudioClip m_AudioClip_Reload;
}
