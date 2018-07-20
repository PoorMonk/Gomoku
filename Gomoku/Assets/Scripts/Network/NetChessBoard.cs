using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetChessBoard : NetworkBehaviour {

    private static NetChessBoard m_instance;
    public int[,] m_grid;
    public GameObject[] m_chesses;
    public float m_timer = 0.0f;
    Transform m_chessParent;
    [HideInInspector] public Stack<GameObject> m_chessStack = new Stack<GameObject>();
    public Text m_winnerText;

    [SyncVar] public PlayerType m_turn;
    [SyncVar] public int m_playerNum = 0;
    [SyncVar] public bool m_isGameOver = true;

    public static NetChessBoard Instance
    {
        get
        {
            return m_instance;
        }
    }

    public void UndoChess()
    {
        if (m_chessStack.Count > 2)
        {
            PopChess();
            PopChess();
        }
    }

    private void PopChess()
    {
        GameObject go = m_chessStack.Pop();
        m_grid[(int)(go.transform.position.x + 7), (int)(go.transform.position.y + 7)] = 0;
        //NetworkServer.UnSpawn(go);
        Destroy(go.gameObject);
        Debug.Log("Destroy Chess");
    }

    public bool PlayChess(int[] pos)
    {
        if (m_isGameOver) return false;

        pos[0] = Mathf.Clamp(pos[0], 0, 14);
        pos[1] = Mathf.Clamp(pos[1], 0, 14);

        if (m_grid[pos[0], pos[1]] != 0)
        {
            return false;
        }

        if (m_turn == PlayerType.BLACK)
        {
            //生成棋子
            CreateChess(m_chesses[0], pos);
            //判断输赢
            if (IsWin(pos))
            {
                EndGame();
            }
            else
            {
                m_turn = PlayerType.WHITE;
            }
        }
        else if (m_turn == PlayerType.WHITE)
        {
            CreateChess(m_chesses[1], pos);
            if (IsWin(pos))
            {
                EndGame();
            }
            else
            {
                m_turn = PlayerType.BLACK;
            }
        }

        return true;
    }

    public void EndGame()
    {
        m_isGameOver = true;
        m_winnerText.transform.parent.parent.gameObject.SetActive(true);
        switch (m_turn)
        {
            case PlayerType.WATCH:
                break;
            case PlayerType.BLACK:
                m_winnerText.text = "黑棋胜！";
                break;
            case PlayerType.WHITE:
                m_winnerText.text = "白棋胜！";
                break;
            default:
                break;
        }

    }

    private bool IsWin(int[] pos)
    {
        if (CheckOneLine(pos, new int[2] { 1, 0 })) return true;
        if (CheckOneLine(pos, new int[2] { 0, 1 })) return true;
        if (CheckOneLine(pos, new int[2] { 1, 1 })) return true;
        if (CheckOneLine(pos, new int[2] { 1, -1 })) return true;
        return false;
    }

    private bool CheckOneLine(int[] pos, int[] offset)
    {
        int iTotal = 1;

        for (int x = pos[0] + offset[0], y = pos[1] + offset[1]; x <= 14 && 0 <= x && y <= 14 && 0 <= y; x += offset[0], y += offset[1])
        {
            if (m_grid[x, y] == (int)m_turn)
            {
                iTotal++;
            }
            else
            {
                break;
            }
        }

        for (int x = pos[0] - offset[0], y = pos[1] - offset[1]; x <= 14 && 0 <= x && y <= 14 && 0 <= y; x -= offset[0], y -= offset[1])
        {
            if (m_grid[x, y] == (int)m_turn)
            {
                iTotal++;
            }
            else
            {
                break;
            }
        }

        if (iTotal >= 5)
        {
            return true;
        }
        return false;
    }

    private void CreateChess(GameObject chess, int[] pos)
    {
        GameObject go = Instantiate(chess, new Vector3(pos[0] - 7, pos[1] - 7), Quaternion.identity);
        go.transform.SetParent(m_chessParent);
        m_grid[pos[0], pos[1]] = (int)m_turn;
        m_chessStack.Push(go);
        NetworkServer.Spawn(go);
    }

    private void FixedUpdate()
    {
        m_timer += Time.deltaTime;
    }

    private void Start()
    {
        m_grid = new int[15, 15];
        m_chessParent = GameObject.Find("Parent").transform;
    }

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
    }

    #region 按钮事件
    public void OnReturnBtnClicked()
    {
        NetworkManager.singleton.matchMaker.DropConnection(NetworkManager.singleton.matchInfo.networkId, NetworkManager.singleton.matchInfo.nodeId,
            0, NetworkManager.singleton.OnDropConnection);
        NetworkManager.singleton.StopHost();
    }
    #endregion
}
