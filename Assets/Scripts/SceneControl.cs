using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;//场景

public class SceneControl : MonoBehaviour
{
    public string Playername;
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void ComeToScenen_Library()
    {
        GetPlayerName();
        SceneManager.LoadScene("FPS_Library");
    }

    public void ComeToScenen_2()
    {
        GetPlayerName();
        SceneManager.LoadScene("FPS_Library");
    }

    //获取玩家姓名
    public void GetPlayerName()
    {
        GameObject m_PetName = GameObject.Find("PetName");
        if (m_PetName != null) Playername = m_PetName.transform.GetChild(0).GetComponent<TMP_Text>().text;
    }
}
