using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class JoinRoom : MonoBehaviour {

    NetworkManager m_networkManager;
    MatchInfoSnapshot m_matchInfo;
    public Text m_nameText;

    private void Start()
    {
        m_networkManager = NetworkManager.singleton;
        if (m_networkManager.matchMaker == null)
        {
            m_networkManager.StartMatchMaker();
        }
    }

    public void SetInfo(MatchInfoSnapshot info)
    {
        m_matchInfo = info;
        m_nameText.text = m_matchInfo.name;
    }

    public void OnJoinBtnClicked()
    {
        m_networkManager.matchMaker.JoinMatch(m_matchInfo.networkId, "", "", "", 0, 0, m_networkManager.OnMatchJoined);
    }
}
