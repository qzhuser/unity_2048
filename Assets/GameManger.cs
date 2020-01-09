using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    public Vector2[] piecesPos;
    //二维数组下标 用来标记棋子的位置 
    int[,] IndexPos=new int[4,4];
    //二维数组坐标 用来绘制棋子 把对应坐标填充进去
    Vector2[,] piecesPosArray = new Vector2[4,4];
    GameObject[,] piecesObj = new GameObject[4,4];
    public GameObject piece;
    Vector2 newPos, oldPos;
    //每次生成两个棋子
    public int count = 2;

    bool isMove = true;
    bool isRight=false, isLeft = false, isUp = false, isDown = false;
    // Start is called before the first frame update
    void Start()
    {
        int a=0;
        for (int i = 0; i < piecesPosArray.GetLength(0); i++)
        {
            for (int j = 0; j < piecesPosArray.GetLength(1); j++)
            {
                piecesPosArray[j,i] = piecesPos[a];
                a++;
            }
        }
        RandomPieces();
       
        DrawPieces();
    }
    /// <summary>
    /// 随机生成棋子
    /// </summary>
    void RandomPieces() {
        int num = 0;
        while(num < 2)
        { 
            int row = Random.Range(0, IndexPos.GetLength(0));
            int cloumn = Random.Range(0, IndexPos.GetLength(1));
            if (IndexPos[row, cloumn] == 0)
            {
                num++;
                IndexPos[row, cloumn] = 2;
            }
        }
    }
    
    /// <summary>
    /// 绘制棋子
    /// </summary>
    void DrawPieces() {
        for (int i = 0; i < piecesObj.GetLength(0); i++)
        {
            for (int j = 0; j < piecesObj.GetLength(1); j++)
            {
                if(piecesObj[i,j]!=null)
                Destroy(piecesObj[i,j]);
            }
        }
        for (int i = 0; i < IndexPos.GetLength(0); i++)
        {
            for (int j = 0; j < IndexPos.GetLength(1); j++)
            {
                if (IndexPos[i, j] > 0)
                {
                    GameObject go = Instantiate(piece, transform.Find("main"));
                    piecesObj[i, j] = go;
                    go.GetComponent<RectTransform>().sizeDelta = new Vector2(125, 120);
                    go.GetComponent<RectTransform>().anchoredPosition = piecesPosArray[i, j];
                    go.transform.GetChild(0).GetComponent<Text>().text = IndexPos[i, j].ToString();
                }
                else { if (piecesObj[i, j] != null) Destroy(piecesObj[i, j]); }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        #region 手指滑动
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                oldPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) {
                newPos = touch.position;
                if (newPos.x - oldPos.x > 0.5f)
                {
                    print("向右滑");
                }
                else if(newPos.x - oldPos.x < -0.5f) {
                    print("向左滑");
                }
            }

        }
        #endregion

        #region 鼠标滑动

        if (Input.GetMouseButton(0))
        {
            float mouseOffsetX = Input.GetAxis("Mouse X");
            float mouseOffsetY=Input.GetAxis("Mouse Y");


            if (mouseOffsetX > 1) {
                isRight = true;
            }
            else if (mouseOffsetX < -1 ) {isLeft = true; }
            else if (mouseOffsetY > 1 ) { isUp = true; }
            else if (mouseOffsetY < -1 ) {   
                isDown = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isDown) PieceMoveDown();
            else if (isUp) PieceMoveUp();
            else if (isRight) PieceMoveRight();
            else if (isLeft) PieceMoveLeft();

            RandomPieces();
            DrawPieces();
        }
        #endregion
    }

    //棋子滑动
    void PieceMoveDown() {
        print("向下滑");
        isDown = false;
        
        //移动棋子
        for (int i = 0; i < IndexPos.GetLength(1); i++)
        {
            //从下向上检测 先移动下面的棋子
            for (int j = 0; j<IndexPos.GetLength(0) ; j++)
            {
                if (IndexPos[i, j] == 0) continue;
                //如果棋子就在最边 不用移动
                if (j == IndexPos.GetLength(0) - 1) continue;
                    //检测下一格是否有棋子
                if (IndexPos[i, j+1] == 0)
                {
                    IndexPos[i, j+1] = IndexPos[i, j];
                    IndexPos[i, j] = 0;
                }
                else
                {
                    ////如果是相同的棋子 融合
                    //if (IndexPos[i, j] == IndexPos[i, j + 1])
                    //{
                    //    IndexPos[i, j + 1] *= 2;
                    //    IndexPos[i, j] = 0;
                    //}
                }
            }
        }
        //融合棋子
        for (int i = 0; i < IndexPos.GetLength(1); i++)
        {
            //从下向上检测 先融合下面的棋子
            for (int j = IndexPos.GetLength(0)-1; j >=0 ; j--)
            {
                if (IndexPos[i, j] == 0) continue;
                if (j - 1 <= 0) continue;
                //如果是相同的棋子 融合
                if (IndexPos[i, j] == IndexPos[i, j - 1])
                {
                    IndexPos[i, j] *= 2;
                    IndexPos[i, j - 1] = 0;
                }
                //j-1肯定是0 从j-2开始检测
                for (int k = j-2; k >=0; k--)
                {
                    if (IndexPos[i, k] == 0) break;

                    IndexPos[i, k+1] = IndexPos[i, k];
                    IndexPos[i, k] = 0;
                }
            }
        }
    }
    void PieceMoveUp()
    {
        print("向上滑");
        isUp = false;
        for (int i = 0; i < IndexPos.GetLength(1); i++)
        {
            for (int j = IndexPos.GetLength(0)-1; j >=0; j--)
            {
                if (IndexPos[i, j] == 0) continue;
                //如果棋子就在最边 不用移动
                if (j == 0) continue;

                //检测下一格是否有棋子
                if (IndexPos[i, j-1] == 0)
                {
                    IndexPos[i, j - 1] = IndexPos[i, j];
                    IndexPos[i, j] = 0;
                }
                else
                {
                    ////如果是相同的棋子 融合
                    //if (IndexPos[i, j] == IndexPos[i, j - 1])
                    //{
                    //    IndexPos[i, j - 1] *= 2;
                    //    IndexPos[i, j] = 0;
                    //}
                }
                   
            }
        }
        //融合棋子
        for (int i = 0; i < IndexPos.GetLength(1); i++)
        {
            //从上向下检测 先融合上面的棋子
            for (int j = 0; j <IndexPos.GetLength(0); j++)
            {
                if (IndexPos[i, j] == 0) continue;
                if (j + 1 > IndexPos.GetLength(0)-1) continue;
                //如果是相邻相同的棋子 融合
                if (IndexPos[i, j] == IndexPos[i, j + 1])
                {
                    IndexPos[i, j] *= 2;
                    IndexPos[i, j + 1] = 0;
                    //j+1肯定是0 从j+2开始检测 下面的棋子依次向上
                    for (int k = j + 2; k < IndexPos.GetLength(0); k++)
                    {
                        if (IndexPos[i, k] == 0) break;

                        IndexPos[i, k - 1] = IndexPos[i, k];
                        IndexPos[i, k] = 0;
                    }
                }
                //如果中间是0 隔着一个或两个格子 出现相同的棋子
                //中间隔一个格子
                //if(IndexPos[i, j + 1]==0)
            }
        }
    }
    void PieceMoveRight()
    {
        print("向右滑");
        isRight = false;
        for (int j = 0; j < IndexPos.GetLength(0); j++)
        {
            for (int i = 0; i < IndexPos.GetLength(1); i++)
            {
                if (IndexPos[i, j] == 0) continue;
                //不能超出总行数
                if (i + 1 <= IndexPos.GetLength(1) - 1)
                {
                    //检测下一格是否有棋子
                    if (IndexPos[i+1,j] == 0)
                    {
                        IndexPos[i+1,j] = IndexPos[i, j];
                        IndexPos[i, j] = 0;
                    }
                    else
                    {
                        //如果是相同的棋子 融合
                        if (IndexPos[i,j] == IndexPos[i+1,j])
                        {
                            IndexPos[i+1,j] *= 2;
                            IndexPos[i, j] = 0;
                        }
                    }
                }
            }
        }
    }

    void PieceMoveLeft()
    {
        print("向左滑");
        isLeft = false;
        for (int j = 0; j < IndexPos.GetLength(0) ; j++)
        {
            for (int i = IndexPos.GetLength(1) - 1; i >= 0; i--)
            {
                if (IndexPos[i, j] == 0) continue;
                //print(IndexPos[i,j]+" "+i+","+j );
                //不能超出列数
                if (i - 1 >= 0)
                {
                    //检测上一列是否有棋子
                    if (IndexPos[i-1, j] == 0)
                    {
                        IndexPos[i-1, j] = IndexPos[i, j];
                        IndexPos[i, j] = 0;
                    }
                    else
                    {
                        //如果是相同的棋子 融合
                        if (IndexPos[i, j] == IndexPos[i-1, j])
                        {
                            IndexPos[i-1, j] *= 2;
                            IndexPos[i, j] = 0;
                        }
                    }
                }
            }
        }
    }
}
