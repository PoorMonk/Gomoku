using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMaxNode
{
    public int m_chess;
    public int[] m_pos;
    public List<MiniMaxNode> m_child;
    public float m_value;
}

public class AILevelThree : Player {

    Dictionary<string, float> m_toScore = new Dictionary<string, float>();
    int m_deep = 3;

    protected override void Start()
    {
        m_toScore.Add("aa___", 100);                      //眠二
        m_toScore.Add("a_a__", 100);
        m_toScore.Add("___aa", 100);
        m_toScore.Add("__a_a", 100);
        m_toScore.Add("a__a_", 100);
        m_toScore.Add("_a__a", 100);
        m_toScore.Add("a___a", 100);


        m_toScore.Add("__aa__", 500);                     //活二 "_aa___"
        m_toScore.Add("_a_a_", 500);
        m_toScore.Add("_a__a_", 500);

        m_toScore.Add("_aa__", 500);
        m_toScore.Add("__aa_", 500);


        m_toScore.Add("a_a_a", 1000);                     // bool lfirst = true, lstop,rstop = false  int AllNum = 1
        m_toScore.Add("aa__a", 1000);
        m_toScore.Add("_aa_a", 1000);
        m_toScore.Add("a_aa_", 1000);
        m_toScore.Add("_a_aa", 1000);
        m_toScore.Add("aa_a_", 1000);
        m_toScore.Add("aaa__", 1000);                     //眠三

        m_toScore.Add("_aa_a_", 9000);                    //跳活三
        m_toScore.Add("_a_aa_", 9000);

        m_toScore.Add("_aaa_", 10000);                    //活三       


        m_toScore.Add("a_aaa", 15000);                    //冲四
        m_toScore.Add("aaa_a", 15000);                    //冲四
        m_toScore.Add("_aaaa", 15000);                    //冲四
        m_toScore.Add("aaaa_", 15000);                    //冲四
        m_toScore.Add("aa_aa", 15000);                    //冲四        


        m_toScore.Add("_aaaa_", 1000000);                 //活四

        m_toScore.Add("aaaaa", float.MaxValue);           //连五


        if (m_playerType != PlayerType.WATCH)
        {
            Debug.Log("AIThree...\n");
        }
    }

    public override void PlayChess()
    {
        if (ChessBoard.Instance.m_isGameOver)
        {
            return;
        }
        if (ChessBoard.Instance.m_chessStack.Count == 0)
        {
            if (ChessBoard.Instance.PlayChess(new int[2] { 7, 7 }))
            {
                ChessBoard.Instance.m_timer = 0;
            }
            return;
        }

        MiniMaxNode node = null;
        foreach (var item in GetList(ChessBoard.Instance.m_grid, (int)m_playerType, true))
        {
            CreateTree(item, (int[,])ChessBoard.Instance.m_grid.Clone(), m_deep, false);
            float a = float.MinValue;
            float b = float.MaxValue;
            item.m_value += AlphaBeta(item, m_deep, false, a, b);
            if (node == null)
            {
                node = item;
            }
            else
            {
                if (node.m_value < item.m_value)
                {
                    node = item;
                }
            }
        }
        ChessBoard.Instance.PlayChess(node.m_pos);
    }

    float CheckOneLine(int[,] grid, int[] pos, int[] offset, int iChess)
    {
        float score = 0.0f;
        int iAllNum = 1;
        bool lStop = false, rStop = false, lFirst = true;
        string str = "a";
        int rx = pos[0] + offset[0], ry = pos[1] + offset[1];
        int lx = pos[0] - offset[0], ly = pos[1] - offset[1];
        while (iAllNum < 7 && (!lStop || !rStop))
        {
            if (lFirst)
            {
                if (lx <= 14 && 0 <= lx && ly <= 14 && 0 <= ly && !lStop)
                {
                    if (grid[lx, ly] == iChess)
                    {
                        iAllNum++;
                        str = "a" + str;
                    }
                    else if (grid[lx, ly] == 0)
                    {
                        iAllNum++;
                        str = "_" + str;
                        if (!rStop)
                        {
                            lFirst = false;
                        }
                    }
                    else
                    {
                        if (!rStop)
                        {
                            lFirst = false;
                        }
                        lStop = true;
                    }
                    lx -= offset[0];
                    ly -= offset[1];
                }
                else
                {
                    if (!rStop)
                    {
                        lFirst = false;
                    }
                    lStop = true;
                }
            }
            else
            {
                if (rx <= 14 && 0 <= rx && ry <= 14 && 0 <= ry && !lFirst && !rStop)
                {
                    if (grid[rx, ry] == iChess)
                    {
                        iAllNum++;
                        str = "a" + str;
                    }
                    else if (grid[rx, ry] == 0)
                    {
                        iAllNum++;
                        str = "_" + str;
                        if (!lStop)
                        {
                            lFirst = true;
                        }
                    }
                    else
                    {
                        if (!lStop)
                        {
                            lFirst = true;
                        }
                        rStop = true;
                    }
                    rx += offset[0];
                    ry += offset[1];
                }
                else
                {
                    if (!lStop)
                    {
                        lFirst = true;
                    }
                    rStop = true;
                }
            }
        }

        string cmpStr = "";
        foreach (var keyInfo in m_toScore.Keys)
        {
            if (str.Contains(keyInfo))
            {
                if (cmpStr == "")
                {
                    cmpStr = keyInfo;
                }
                else
                {
                    if (m_toScore[keyInfo] > m_toScore[cmpStr])
                    {
                        cmpStr = keyInfo;
                    }
                }
            }
        }
        if (cmpStr != "")
        {
            score += m_toScore[cmpStr];
        }
        return score;
    }

    public float GetScore(int[,] grid, int[] pos)
    {
        float score = 0;
        score += CheckOneLine(grid, pos, new int[2] { 1, 0 }, 1);
        score += CheckOneLine(grid, pos, new int[2] { 0, 1 }, 1);
        score += CheckOneLine(grid, pos, new int[2] { 1, 1 }, 1);
        score += CheckOneLine(grid, pos, new int[2] { 1, -1 }, 1);

        score += CheckOneLine(grid, pos, new int[2] { 1, 0 }, 2);
        score += CheckOneLine(grid, pos, new int[2] { 0, 1 }, 2);
        score += CheckOneLine(grid, pos, new int[2] { 1, 1 }, 2);
        score += CheckOneLine(grid, pos, new int[2] { 1, -1 }, 2);

        return score;
    }

    List<MiniMaxNode> GetList(int[,] grid, int iChess, bool IsMySelf)
    {
        List<MiniMaxNode> nodeList = new List<MiniMaxNode>();
        MiniMaxNode node;
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                int[] pos = new int[2] { i, j };
                if (grid[pos[0], pos[1]] != 0)
                {
                    continue;
                }

                node = new MiniMaxNode();
                node.m_chess = iChess;
                node.m_pos = pos;
                if (IsMySelf)
                {
                    node.m_value = GetScore(grid, pos);
                }
                else
                {
                    node.m_value = -GetScore(grid, pos);
                }
                

                if (nodeList.Count < 4)
                {
                    nodeList.Add(node);
                }
                else
                {
                    foreach (var item in nodeList)
                    {
                        if (IsMySelf)
                        {
                            if (node.m_value > item.m_value)
                            {
                                nodeList.Remove(item);
                                nodeList.Add(node);
                                break;
                            }
                        }
                        else
                        {
                            if (node.m_value < item.m_value)
                            {
                                nodeList.Remove(item);
                                nodeList.Add(node);
                                break;
                            }
                        }
                    }
                }
            }
        }

        return nodeList;
    }

    public void CreateTree(MiniMaxNode node, int[,] grid, int deep, bool IsMySelf)
    {
        if (deep == 0 || node.m_value == float.MaxValue)
        {
            return;
        }

        grid[node.m_pos[0], node.m_pos[1]] = node.m_chess;
        node.m_child = GetList(grid, node.m_chess, !IsMySelf);
        foreach (var item in node.m_child)
        {
            CreateTree(item, (int[,])grid.Clone(), deep - 1, !IsMySelf);
        }
    }

    public float AlphaBeta(MiniMaxNode node, int deep, bool IsMySelf, float alpha, float beta)
    {
        if (deep == 0 || node.m_value == float.MaxValue || node.m_value == float.MinValue)
        {
            return node.m_value;
        }

        if (IsMySelf)
        {
            foreach (var child in node.m_child)
            {
                alpha = Mathf.Max(alpha, AlphaBeta(child, deep - 1, !IsMySelf, alpha, beta));
                if (alpha >= beta)
                {
                    return alpha;
                }
            }
            return alpha;
        }
        else
        {
            foreach (var child in node.m_child)
            {
                beta = Mathf.Min(beta, AlphaBeta(child, deep - 1, !IsMySelf, alpha, beta));
                if (alpha >= beta)
                {
                    return beta;
                }
            }
            return beta;
        }
    }

    protected override void ChangeBtnColor()
    {

    }
}
