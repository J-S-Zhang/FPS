using Settings;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SavaSystemSettings : MonoBehaviour
{
    //设置脚本类 对象
    SystemSettings m_SystemSettings;//目前包含 昵称 音量开关 大小
    [SerializeField]
    private TMP_Text m_PetName;//显示昵称的text
    [SerializeField]
    private Toggle m_PlayMusic;//控制音乐开关的复选框
    [SerializeField]
    private Slider m_MusicVolume;//控制音量大小 的滑动条
    [SerializeField]
    private TMP_InputField m_NewName;//新昵称


    public void SaveSettingBtnClick()
    {
        updatePlayerName();
        SavaSystemSettingsToJson();
    }
    public void updatePlayerName()
    {
        if(m_NewName.text != m_PetName.text) m_PetName.text = m_NewName.text;
    }

    //保存信息到json
    public void SavaSystemSettingsToJson()
    {
        m_SystemSettings = new SystemSettings();
        //获取 设置信息
        m_SystemSettings.PetName = m_PetName.text;
        m_SystemSettings.PlayMusic = m_PlayMusic.isOn;
        m_SystemSettings.MusicVolume = m_MusicVolume.value;

        string systemsetting=JsonUtility.ToJson(m_SystemSettings);
        //保存在 永久数据文件夹下
        //File.WriteAllText(Application.persistentDataPath+"/SystemSettings.json",systemsetting+"\n");
        //保存在资源文件下
        File.WriteAllText(Application.dataPath+"/SystemSettings.json",systemsetting+"\n");
    }
}
