# 五子棋游戏实现 #

1 导入资源图片。拖入棋盘后，将棋盘的大小进行缩放，使每一格间距一米（对齐背景虚线即可），方便鼠标点击时落子；黑白子也适当缩放成合适的大小。如下图：

![](https://raw.githubusercontent.com/PoorMonk/MarkDownPhotos/master/01chessBoard.png)

2 点击棋盘上的位置落子

	private void CreateChess(GameObject chess, int[] pos)
    {
        GameObject go = Instantiate(chess, new Vector3(pos[0] - 7, pos[1] - 7), Quaternion.identity);
        go.transform.SetParent(m_chessParent);
        m_grid[pos[0], pos[1]] = (int)m_turn;
        m_chessStack.Push(go);
    }

3 判断五子相连

	private bool CheckOneLine(int[] pos, int[] offset)
    {       
        int iTotal = 1;
        
		//从落点的右边开始扫描，直到不是同样颜色就停止
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

		//再接着扫描左边，也直到不是同样颜色就停止
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

4 加入悔棋操作

	//取出保存在栈顶的棋子，将对应位置清零之后销毁
	private void PopChess()
    {
        GameObject go = m_chessStack.Pop();
        m_grid[(int)(go.transform.position.x + 7), (int)(go.transform.position.y + 7)] = 0;
        Destroy(go.gameObject);        
    }

5 用一个红点标识刚落的子

	void Update ()
	{
        if (ChessBoard.Instance.m_chessStack.Count > 0)
        {
            transform.position = ChessBoard.Instance.m_chessStack.Peek().transform.position; //跟随栈顶棋子的位置
        }
	}

## AI ##

思路：不同的棋子组合有不同的分数，每次落子时考虑棋盘上每一个空位，根据最佳得分位置落子。

1 AIOne 初级难度

	protected Dictionary<string, float> m_toScore = new Dictionary<string, float>(); //用来保存每一种组合的分数
    protected float[,] m_score = new float[15, 15]; //用来保存每一个位置的分数

	//初始化每一种组合的得分比重
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
    }

	//扫描每一行，碰到己方的子，加a，空位加_后换方向扫描，左右两边都扫描完后对比扫描出的字符处是否存在表中，有则在该位置添加分数
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

	//不仅要扫描己方的最佳位置，还要扫描对方的最佳位置，才能可攻可守
	public void SetScore(int[] pos)
    {
        m_score[pos[0], pos[1]] = 0;  //扫描前将该位置分数清零
        CheckOneLine(pos, new int[2] { 1, 0 }, 1);
        CheckOneLine(pos, new int[2] { 0, 1 }, 1);
        CheckOneLine(pos, new int[2] { 1, 1 }, 1);
        CheckOneLine(pos, new int[2] { 1, -1}, 1);

        CheckOneLine(pos, new int[2] { 1, 0 }, 2);
        CheckOneLine(pos, new int[2] { 0, 1 }, 2);
        CheckOneLine(pos, new int[2] { 1, 1 }, 2);
        CheckOneLine(pos, new int[2] { 1, -1 }, 2);
    }

	//落子时，扫描棋盘每个空位，取得分最高的空位落子
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

2 AITwo 中级难度（继承自AIOne）

	扩展棋子组合
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

        m_toScore.Add("a_a_a", 1000);                     
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
    }

	//扫描时先从左边扫起，碰到己方则在左边加a，碰到空位则在左边加_，如果右边能扫就换到右边扫描，碰到敌方则左边扫描结束；右方扫描碰到己方在右边加a，碰到空位右边加_，如果左边还能扫则换到左边，不能则继续，碰到敌方则右边扫描结束；如果左右都不能扫或者已扫描棋子数达到七个则停止扫描
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
                        str += "a";
                    }
                    else if (ChessBoard.Instance.m_grid[rx, ry] == 0)
                    {
                        iAllNum++;
                        str += "_";
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
					//扫描后的字符处可能包含于多个key中，取分数大的那一个
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

3 AIThree 高级难度（与AITwo一样初始化棋子组合表）

	int m_deep = 3;
	public class MiniMaxNode //极大极小节点
	{
	    public int m_chess;
	    public int[] m_pos;
	    public List<MiniMaxNode> m_child;
	    public float m_value;
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

	// 获取四个极大极小节点值
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

	// 为每个节点构建一棵树
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

	// 为每棵树进行AlphaBeta剪枝
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

	// 与AITwo类似的扫描方法
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
                        str += "a";
                    }
                    else if (grid[rx, ry] == 0)
                    {
                        iAllNum++;
                        str += "_";
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

## 网络通信 ##

引用UnityEngine.Networking;

**注意点**：

1 由服务器生成的物体必须具有Network Identity组件

	private void CreateChess(GameObject chess, int[] pos)
    {
        GameObject go = Instantiate(chess, new Vector3(pos[0] - 7, pos[1] - 7), Quaternion.identity);
        go.transform.SetParent(m_chessParent);
        m_grid[pos[0], pos[1]] = (int)m_turn;
        m_chessStack.Push(go);
        NetworkServer.Spawn(go); //由服务器生成棋子
    }

2 客户端需要同步的值必须加[SyncVar]属性，比如：

	[SyncVar] public PlayerType m_turn;
    [SyncVar] public int m_playerNum = 0;
    [SyncVar] public bool m_isGameOver = true;

3 由客户端发送命令，服务器端执行的函数前必须加[Command]属性，且函数必须以Cmd开头，比如：

	[Command]
    public void CmdChess(Vector2 pos)
    {
        if (NetChessBoard.Instance.PlayChess(new int[2] { (int)(pos.x + 7.5f), (int)(pos.y + 7.5f) }))
        {
            NetChessBoard.Instance.m_timer = 0.0f;
        }
    }

## 知识点 ##

1 对于鼠标点击的超出棋盘范围的点，可以通过调用Mathf.Clamp函数来取得给定范围内的值

2 删除预制体后对象名字会变成红色，选中物体后在GameObject菜单下点击Break Prefab Instance即可恢复正常的颜色

3 调用EventSystem.IsPointerOverGameObject，鼠标点在UI对象上（例如Button）不响应其它操作（即不会生成棋子）

	public class MouseExample : MonoBehaviour
	{
	    void Update()
	    {
	        // Check if the left mouse button was clicked
	        if (Input.GetMouseButtonDown(0))
	        {
	            // Check if the mouse was clicked over a UI element
	            if (EventSystem.current.IsPointerOverGameObject())
	            {
	                Debug.Log("Clicked on the UI");
	            }
	        }
	    }
	}

4 设置玩家列表，不同的游戏模式设置不同的玩家

	在UI上设置好对应的index后再进入游戏
	public void SetPlayer1(int index)
    {
        PlayerPrefs.SetInt("Player1", index);
    }

    public void SetPlayer2(int index)
    {
        PlayerPrefs.SetInt("Player2", index);
    }

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