using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public List<Player> m_playerList = new List<Player>();
    //public bool m_isDoubleModel = false;

    private void Awake()
    {
        int iPlayer1 = PlayerPrefs.GetInt("Player1");
        int iPlayer2 = PlayerPrefs.GetInt("Player2");
        
        for (int i = 0; i < m_playerList.Count; i++)
        {
            if (i == iPlayer1)
            {
                m_playerList[i].m_playerType = PlayerType.BLACK;
            }
            else if (i == iPlayer2)
            {
                m_playerList[i].m_playerType = PlayerType.WHITE;
            }
            else
            {
                m_playerList[i].m_playerType = PlayerType.WATCH;
            }
        }
    }

    public void SetPlayer1(int index)
    {
        PlayerPrefs.SetInt("Player1", index);
    }

    public void SetPlayer2(int index)
    {
        PlayerPrefs.SetInt("Player2", index);
    }

    public void ReturnStartUI()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayNetGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ChangePlayerColor()
    {
        for (int i = 0; i < m_playerList.Count; i++)
        {
            if (m_playerList[i].m_playerType == PlayerType.BLACK)
            {
                SetPlayer2(i);
            }
            else if (m_playerList[i].m_playerType == PlayerType.WHITE)
            {
                SetPlayer1(i);
            }
            else
            {
                m_playerList[i].m_playerType = PlayerType.WATCH;
            }
        }
        PlayGame();
    }

    public void SetDoubleModel()
    {
        PlayerPrefs.SetInt("DoubleModel", 24);
    }

    public void OnBackBtnClicked()
    {
        PlayerPrefs.SetInt("DoubleModel", 1);
        ReturnStartUI();
    }
}
