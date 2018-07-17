using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILevelTwo : AILevelOne {

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
            Debug.Log("AITwo...\n");
        }
    }

    protected override void CheckOneLine(int[] pos, int[] offset, int iChess)
    {
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
                    if (ChessBoard.Instance.m_grid[lx, ly] == iChess)
                    {
                        iAllNum++;
                        str = "a" + str;
                    }
                    else if (ChessBoard.Instance.m_grid[lx, ly] == 0)
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
                    if (ChessBoard.Instance.m_grid[rx, ry] == iChess)
                    {
                        iAllNum++;
                        str = "a" + str;
                    }
                    else if (ChessBoard.Instance.m_grid[rx, ry] == 0)
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
            m_score[pos[0], pos[1]] += m_toScore[cmpStr];
        }
    }

    protected override void ChangeBtnColor()
    {

    }
}
