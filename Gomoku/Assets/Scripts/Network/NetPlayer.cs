using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetPlayer : NetworkBehaviour {

    [SyncVar]public PlayerType m_playerType;
    bool m_isDoubleModel = false;
    Button m_undoBtn;

    void Start()
    {
        if (isLocalPlayer)
        {
            CmdSetPlayer();
            if (m_playerType == PlayerType.WATCH)
            {
                return;
            }
            m_undoBtn = GameObject.Find("UndoButton").GetComponent<Button>();
            m_undoBtn.onClick.AddListener(OnUndoBtnClicked);
        }
        Debug.Log(Network.player.ipAddress);
    }

    void FixedUpdate()
    {
        if (m_playerType == NetChessBoard.Instance.m_turn && NetChessBoard.Instance.m_timer > 0.3f && isLocalPlayer && NetChessBoard.Instance.m_playerNum > 1)
        {
            PlayChess();
        }

        if (m_playerType != PlayerType.WATCH && isLocalPlayer && NetChessBoard.Instance.m_isGameOver)
        {
            NetChessBoard.Instance.EndGame();
        }

        if (m_playerType != PlayerType.WATCH && isLocalPlayer)
        {
            ChangeBtnColor();
        }
    }

    public void PlayChess()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log((int)(pos.x) + "*" + (int)(pos.y));
            if (pos.x < 8 && -8 < pos.x && pos.y < 8 && -8 < pos.y)
            {
                CmdChess(pos);
            }
        }
    }

    private void ChangeBtnColor()
    {
        if (m_isDoubleModel)
        {
            return;
        }

        if (m_playerType == NetChessBoard.Instance.m_turn)
        {
            m_undoBtn.interactable = true;
        }
        else
        {
            m_undoBtn.interactable = false;
        }
    }

    public void OnUndoBtnClicked()
    {
        CmdUndoChess();
    }

    [Command]
    public void CmdUndoChess()
    {
        NetChessBoard.Instance.UndoChess();
    }

    [Command]
    public void CmdChess(Vector2 pos)
    {
        if (NetChessBoard.Instance.PlayChess(new int[2] { (int)(pos.x + 7.5f), (int)(pos.y + 7.5f) }))
        {
            NetChessBoard.Instance.m_timer = 0.0f;
        }
    }

    [Command]
    public void CmdSetPlayer()
    {
        NetChessBoard.Instance.m_playerNum++;
        if (NetChessBoard.Instance.m_playerNum == 1)
        {
            m_playerType = PlayerType.BLACK;
        }
        else if (NetChessBoard.Instance.m_playerNum == 2)
        {
            m_playerType = PlayerType.WHITE;
        }
        else
        {
            m_playerType = PlayerType.WATCH;
        }
    }
}
