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