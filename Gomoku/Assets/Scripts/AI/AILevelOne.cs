using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILevelOne : Player {

    protected Dictionary<string, float> m_toScore = new Dictionary<string, float>();
    protected float[,] m_score = new float[15, 15];
    //public PlayerType m_chessColor;

    protected override void Start()
    {
        m_toScore.Add("_aa", 50);
        m_toScore.Add("aa_", 50);
        m_toScore.Add("_aa_", 100);

        m_toScore.Add("_aaa", 500);
        m_toScore.Add("aaa_", 500);
        m_toScore.Add("_aaa_", 1000);

        m_toScore.Add("_aaaa", 5000);
        m_toScore.Add("aaaa_", 5000);
        m_toScore.Add("_aaaa_", 10000);

        m_toScore.Add("_aaaaa", float.MaxValue);
        m_toScore.Add("aaaaa_", float.MaxValue);
        m_toScore.Add("aaaaa", float.MaxValue);
        m_toScore.Add("_aaaaa_", float.MaxValue);

        if (m_playerType != PlayerType.WATCH)
        {
            Debug.Log("AIOne...\n");
        }
    }

    protected virtual void CheckOneLine(int[] pos, int[] offset, int iChess)
    {
        string str = "a";
        for (int x = pos[0] + offset[0], y = pos[1] + offset[1]; x <= 14 && 0 <= x && y <= 14 && 0 <= y; x += offset[0], y += offset[1])
        {
            if (ChessBoard.Instance.m_grid[x, y] == iChess)
            {
                str += "a";
            }
            else if (ChessBoard.Instance.m_grid[x, y] == 0)
            {
                str += "_";
                break;
            }
            else
            {
                break;
            }
        }

        for (int x = pos[0] - offset[0], y = pos[1] - offset[1]; x <= 14 && 0 <= x && y <= 14 && 0 <= y; x -= offset[0], y -= offset[1])
        {
            if (ChessBoard.Instance.m_grid[x, y] == iChess)
            {
                str = "a" + str ;
            }
            else if (ChessBoard.Instance.m_grid[x, y] == 0)
            {
                str = "_" + str;
                break;
            }
            else
            {
                break;
            }
        }  
        if (m_toScore.ContainsKey(str))
        {
            m_score[pos[0], pos[1]] += m_toScore[str];
        }
    }

    public void SetScore(int[] pos)
    {
        m_score[pos[0], pos[1]] = 0;
        CheckOneLine(pos, new int[2] { 1, 0 }, 1);
        CheckOneLine(pos, new int[2] { 0, 1 }, 1);
        CheckOneLine(pos, new int[2] { 1, 1 }, 1);
        CheckOneLine(pos, new int[2] { 1, -1}, 1);

        CheckOneLine(pos, new int[2] { 1, 0 }, 2);
        CheckOneLine(pos, new int[2] { 0, 1 }, 2);
        CheckOneLine(pos, new int[2] { 1, 1 }, 2);
        CheckOneLine(pos, new int[2] { 1, -1 }, 2);
    }

    public override void PlayChess()
    {
        if (ChessBoard.Instance.m_chessStack.Count == 0)
        {
            if (ChessBoard.Instance.PlayChess(new int[2] { 7, 7}))
            {
                ChessBoard.Instance.m_timer = 0.0f;
            }
            return;
        }

        float maxScore = 0;
        int[] maxPos = new int[2] { 0, 0 };
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if (ChessBoard.Instance.m_grid[i, j] == 0)
                {
                    SetScore(new int[2] { i, j });
                    if (m_score[i, j] >= maxScore)
                    {
                        maxPos[0] = i;
                        maxPos[1] = j;
                        maxScore = m_score[i, j];
                    }
                }
            }
        }
        if (ChessBoard.Instance.PlayChess(maxPos))
        {
            ChessBoard.Instance.m_timer = 0.0f;
        }
    }

    protected override void ChangeBtnColor()
    {
        
    }

}
