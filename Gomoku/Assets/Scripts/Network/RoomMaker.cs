using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class RoomMaker : MonoBehaviour {

    NetworkManager m_networkManager;
    string m_roomName;
    List<GameObject> m_roomList = new List<GameObject>();
    [SerializeField]GameObject m_room;
    [SerializeField]Transform m_parent;

    private void Start()
    {
        m_networkManager = NetworkManager.singleton;
        if (m_networkManager.matchMaker == null)
        {
            m_networkManager.StartMatchMaker();
        }
    }

    public void OnSetRoomName(string name)
    {
        m_roomName = name;
    }

    public void OnCreateRoomBtnClicked()
    {
        m_networkManager.matchMaker.CreateMatch(m_roomName, 3, true, "", "", "", 0, 0, m_networkManager.OnMatchCreate);
    }

    public void OnRefreshBtnClicked()
    {
        m_networkManager.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (!success)
        {
            Debug.Log("Error...\n");
            return;
        }
        ClearRoomList();
        foreach (var match in matches)
        {
            GameObject go = Instantiate(m_room, m_parent);
            go.GetComponent<JoinRoom>().SetInfo(match);
            m_roomList.Add(go);
            
        }
    }

    void ClearRoomList()
    {
        for (int i = 0; i < m_roomList.Count; i++)
        {
            Destroy(m_roomList[i]);
        }
        m_roomList.Clear();
    }
}
