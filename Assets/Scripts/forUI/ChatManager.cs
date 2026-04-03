using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    SceneControl m_SceneControl;
    public string localPlayerName;

    [Header("Chat UI")]
    [SerializeField] GameObject ChatPanel;
    [SerializeField] Text chatHistory;
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] InputField chatMessage;
    [SerializeField] Button sendButton;

    internal static readonly Dictionary<NetworkConnectionToClient, string> connNames = new Dictionary<NetworkConnectionToClient, string>();

    private void Start()
    {
        m_SceneControl = GameObject.Find("SceneControl").GetComponent<SceneControl>();
        localPlayerName = m_SceneControl.Playername;
        ChatPanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            OpenAndCloseChatPanel();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
    }
    public void OpenAndCloseChatPanel()
    {
        //activeSelf 自身+父物体     activeInHierarchy  自身
        ChatPanel.SetActive(!ChatPanel.activeSelf);//ChatPanel.activeInHierarchy
    }

    //回车 or ClickBtn 
    public void SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(chatMessage.text))
        {
            CmdSend(chatMessage.text.Trim());
            chatMessage.text = string.Empty;
            chatMessage.ActivateInputField();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSend(string text)
    {
        RpcSend(localPlayerName, text.Trim());
    }

    [ClientRpc]
    public void RpcSend(string playerName,string text)
    {
        string finalText = playerName == localPlayerName ?
                $"<color=red>{playerName}:</color> {text}" :
                $"<color=blue>{playerName}:</color> {text}";

        StartCoroutine(AppendAndScroll(finalText));
    }

    IEnumerator AppendAndScroll(string text)
    {
        chatHistory.text += text + "\n";

        yield return null;
        yield return null;

        scrollbar.value = 0;
    }
}
