using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    public PlayerType m_playerType;
    Button m_regretBtn;
    bool m_isDoubleModel = false;

    protected virtual void Start()
    {
        m_regretBtn = GameObject.Find("UndoButton").GetComponent<Button>();
        if (PlayerPrefs.GetInt("DoubleModel") == 24)
        {
            m_isDoubleModel = true; ;
        }

    }

    protected virtual void FixedUpdate()
    {
        if (m_playerType == ChessBoard.Instance.m_turn && ChessBoard.Instance.m_timer > 0.3f)
        {
            PlayChess();
        }
        if (!m_isDoubleModel)
        {
            ChangeBtnColor();
        }
    }

    public virtual void PlayChess()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log((int)(pos.x + 7.5f) + "*" + (int)(pos.y + 7.5f));
            if (ChessBoard.Instance.PlayChess(new int[2] { (int)(pos.x + 7.5f), (int)(pos.y + 7.5f) }))
            {
                ChessBoard.Instance.m_timer = 0.0f;
            }
        }
    }

    protected virtual void ChangeBtnColor()
    {
        if (m_isDoubleModel)
        {
            return;
        }

        if (m_playerType == ChessBoard.Instance.m_turn)
        {
            m_regretBtn.interactable = true;
        }
        else
        {
            m_regretBtn.interactable = false;
        }
    }
}
