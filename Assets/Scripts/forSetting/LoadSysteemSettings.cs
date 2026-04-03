using Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadSysteemSettings : MonoBehaviour
{
    [SerializeField]
    private SystemSettings m_SystemSettings;
    [SerializeField]
    private TMP_Text m_PetName;//显示昵称的text
    [SerializeField]
    private Toggle m_PlayMusic;//控制音乐开关的复选框
    [SerializeField]
    private Slider m_MusicVolume;//控制音量大小 的滑动条
    [SerializeField]
    private TMP_InputField m_NewName;//新昵称

    private void Awake()
    {
        LoadSystemSettings();

        m_NewName.text=m_PetName.text;
    }
    //加载系统设置
    private void LoadSystemSettings()
    {
        string filepath = Application.dataPath + "/SystemSettings.json";
        if (!File.Exists(filepath))
        {
            return;
        }
        //c#系统类  二进制数据到sr
        StreamReader sr = new StreamReader(filepath);
        string json= sr.ReadToEnd();
        if (!string.IsNullOrEmpty(json))
        {
            m_SystemSettings=JsonUtility.FromJson<SystemSettings>(json);
            //为系统设置赋值
            if(m_PetName != null)
            {
                m_PetName.text=m_SystemSettings.PetName;
            }
            if(m_PlayMusic != null)
            {
                m_PlayMusic.isOn=m_SystemSettings.PlayMusic;
            }
            if (m_MusicVolume != null)
            {
                m_MusicVolume.value=m_SystemSettings.MusicVolume;
            }
        }
    }
}
