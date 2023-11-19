using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardController : MonoBehaviour
{
    public int boardSize;

    [SerializeField] GameObject tile;
    [SerializeField] GameObject edgeTile;
    [SerializeField] GameObject cornerTile;
    [SerializeField] GameObject piece;

    [SerializeField] TextMeshProUGUI blackCountText;
    [SerializeField] TextMeshProUGUI whiteCountText;

    private Transform backgroundHolder;
    private Transform pieceHolder;

    private int cellSize = 1;

    private GameObject[,] visualBoard;
    private int[,] board;

    private int blackCount = 0;
    private int whiteCount = 0;

    //Piece Colours
    private Color whiteColor = new Color(1f, 1f, 1f, 1f);
    private Color blackColor = new Color(0, 0, 0, 1f);
    private Color halfWhiteColor = new Color(1f, 1f, 1f, 0.39f);
    private Color halfBlackColor = new Color(0, 0, 0, 0.78f);

    private Color[] colors;

    void Awake()
    {
        backgroundHolder = transform.GetChild(0);
        pieceHolder = transform.GetChild(1);
        visualBoard = new GameObject[boardSize, boardSize];
        board = new int[boardSize, boardSize];
        colors = new Color[4] { whiteColor, blackColor, halfWhiteColor, halfBlackColor };
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera camera = Camera.main;
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (i == 0)
                {
                    if (j == 0)
                    {
                        Instantiate(cornerTile, new Vector3(i * cellSize, j * cellSize, 5), Quaternion.identity * Quaternion.Euler(0, 0, 180f), backgroundHolder);
                    }
                    else if (j == boardSize - 1)
                    {
                        Instantiate(cornerTile, new Vector3(i * cellSize, j * cellSize, 5), Quaternion.identity*Quaternion.Euler(0,0,90f), backgroundHolder);
                    }
                    else
                    {
                        Instantiate(edgeTile, new Vector3(i * cellSize, j * cellSize, 5), Quaternion.identity * Quaternion.Euler(0, 0, 270f), backgroundHolder);
                    }
                }
                else if (i == boardSize - 1)
                {
                    if (j == 0)
                    {
                        Instantiate(cornerTile, new Vector3(i * cellSize, j * cellSize, 5), Quaternion.identity * Quaternion.Euler(0, 0, 270f), backgroundHolder);
                    }
                    else if (j == boardSize - 1)
                    {
                        Instantiate(cornerTile, new Vector3(i * cellSize, j * cellSize, 5), Quaternion.identity, backgroundHolder);
                    }
                    else
                    {
                        Instantiate(edgeTile, new Vector3(i * cellSize, j * cellSize, 5), Quaternion.identity * Quaternion.Euler(0, 0, 90f), backgroundHolder);
                    }
                }
                else if (j == 0)
                {
                    Instantiate(edgeTile, new Vector3(i * cellSize, j * cellSize, 5), Quaternion.identity, backgroundHolder);
                }
                else if (j == boardSize - 1)
                {
                    Instantiate(edgeTile, new Vector3(i * cellSize, j * cellSize, 5), Quaternion.identity * Quaternion.Euler(0, 0, 180f), backgroundHolder);
                }
                else
                {
                    Instantiate(tile, new Vector3(i * cellSize, j * cellSize, 5), Quaternion.identity, backgroundHolder);
                }
            }
        }
        camera.orthographicSize = boardSize*cellSize / 2.0f;
        camera.transform.position = new Vector3(cellSize * (boardSize - 1) / 2f, cellSize * (boardSize - 1) / 2f, -10);
        UpdatePieceCount();
    }

    bool IsBoardFull()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (board[i, j] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void UpdatePieceCount()
    {
        blackCount = 0;
        whiteCount = 0;
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (board[i, j] == 1 || board[i, j] == 3)
                {
                    whiteCount++;
                }
                else if (board[i, j] == 2 || board[i, j] == 4)
                {
                    blackCount++;
                }
            }
        }
        blackCountText.text = string.Format("BLACK COUNT: {0}", blackCount);
        whiteCountText.text = string.Format("WHITE COUNT: {0}", whiteCount);
    }

    public int CheckVictory()
    {
        if (IsBoardFull())
        {
            int balance = whiteCount - blackCount;
            balance = balance > 0 ? 0 : 1;
            return balance;
        }
        return -1;
    }

    public int GetCellSize()
    {
        return cellSize;
    }

    public bool CellFree(int X, int Y)
    {
        return board[X, Y] == 0;
    }

    public bool Place(int X, int Y,int turn)
    {
        if (board[X, Y] != 0) return false;
        board[X, Y] = turn+1;
        visualBoard[X, Y] = Instantiate(piece, new Vector3(X * cellSize, Y * cellSize, 0), Quaternion.identity, pieceHolder);
        visualBoard[X, Y].GetComponent<SpriteRenderer>().color = colors[turn];
        List<Tuple<int, int>> toFills = FindAllFills(X, Y);
        for (int i = 0; i < toFills.Count; i++)
        {
            int tmp_x = toFills[i].Item1;
            int tmp_y = toFills[i].Item2;
            if (board[tmp_x, tmp_y] == turn + 1) { continue; }
            board[tmp_x, tmp_y] = turn + 3;
            if (visualBoard[tmp_x * cellSize, tmp_y * cellSize] == null)
            {
                visualBoard[tmp_x * cellSize, tmp_y * cellSize] = Instantiate(piece, new Vector3(tmp_x * cellSize, tmp_y * cellSize, 0), Quaternion.identity, pieceHolder);
            }
            visualBoard[tmp_x * cellSize, tmp_y * cellSize].GetComponent<SpriteRenderer>().color = colors[turn+2];
        }
        UpdatePieceCount();
        return true;
    }

    List<Tuple<int,int>> FindAllFills(int X, int Y)
    {
        List<Tuple<int,int>> fillList = new List<Tuple<int,int>>();
        if (CheckDir(X,Y,0)  || CheckDir(X, Y, 1))
        {
            for (int y = Y - 1; y >= 0; y--)
            {
                if (board[X, y] == 3 - board[X, Y])
                {
                    break;
                }
                fillList.Add(new Tuple<int,int>(X, y));
            }

            for (int y = Y + 1; y < boardSize; y++)
            {
                if (board[X, y] == 3 - board[X, Y])
                {
                    break;
                }
                fillList.Add(new Tuple<int, int>(X, y));
            }
        }

        if (CheckDir(X, Y, 2) || CheckDir(X, Y, 3))
        {
            for (int d = 1; d <= Mathf.Min(X, Y); d++)
            {
                if (board[X - d, Y - d] == 3 - board[X, Y])
                {
                    break;
                }
                fillList.Add(new Tuple<int, int>(X - d, Y - d));
            }

            for (int d = 1; d < boardSize - Mathf.Max(X, Y); d++)
            {
                if (board[X + d, Y + d] == 3 - board[X, Y])
                {
                    break;
                }
                fillList.Add(new Tuple<int, int>(X + d, Y + d));
            }
        }

        if (CheckDir(X, Y, 4) || CheckDir(X, Y, 5))
        {
            for (int x = X - 1; x >= 0; x--)
            {
                if (board[x, Y] == 3 - board[X, Y])
                {
                    break;
                }
                fillList.Add(new Tuple<int, int>(x, Y));
            }

            for (int x = X + 1; x < boardSize; x++)
            {
                if (board[x, Y] == 3 - board[X, Y])
                {
                    break;
                }
                fillList.Add(new Tuple<int, int>(x, Y));
            }
        }

        if (CheckDir(X, Y, 6) || CheckDir(X, Y, 7))
        {
            for (int d = 1; d <= Mathf.Min(boardSize - X - 1, Y); d++)
            {
                if (board[X + d, Y - d] == 3 - board[X, Y])
                {
                    break;
                }
                fillList.Add(new Tuple<int, int>(X + d, Y - d));
            }

            for (int d = 1; d <= Mathf.Min(X,boardSize - Y - 1); d++)
            {
                if (board[X - d, Y + d] == 3 - board[X, Y])
                {
                    break;
                }
                fillList.Add(new Tuple<int, int>(X - d, Y + d));
            }
        }
        return fillList;
    }

    bool CheckDir(int X, int Y, int dir)
    {
        if (dir == 0)
        {
            for (int y = Y - 1; y >= 0; y--)
            {
                if (board[X, y] == board[X, Y])
                {
                    return true;
                }
                if (board[X, y] == 3 - board[X, Y])
                {
                    return false;
                }
            }
        }
        if (dir== 1) 
        { 
            for (int y = Y + 1; y < boardSize; y++)
            {
                if (board[X, y] == board[X, Y])
                {
                    return true;
                }
                if (board[X, y] == 3 - board[X, Y])
                {
                    return false;
                }
            }
        }

        if (dir == 2)
        {
            for (int d = 1; d <= Mathf.Min(X, Y); d++)
            {
                if (board[X - d, Y - d] == board[X, Y])
                {
                    return true;
                }
                if (board[X - d, Y - d] == 3 - board[X, Y])
                {
                    return false;
                }
            }
        }
        if (dir == 3)
        {
            for (int d = 1; d < boardSize - Mathf.Max(X, Y); d++)
            {
                if (board[X + d, Y + d] == board[X, Y])
                {
                    return true;
                }
                if (board[X + d, Y + d] == 3 - board[X, Y])
                {
                    return false;
                }
            }
        }

        if (dir == 4)
        {
            for (int x = X - 1; x >= 0; x--)
            {
                if (board[x, Y] == board[X, Y])
                {
                    return true;
                }
                if (board[x, Y] == 3 - board[X, Y])
                {
                    return false;
                }
            }
        }
        if (dir == 5)
        {
            for (int x = X + 1; x < boardSize; x++)
            {
                if (board[x, Y] == board[X, Y])
                {
                    return true;
                }
                if (board[x, Y] == 3 - board[X, Y])
                {
                    return false;
                }
            }
        }

        if (dir == 6)
        {
            for (int d = 1; d <= Mathf.Min(boardSize - X - 1, Y); d++)
            {
                if (board[X + d, Y - d] == board[X, Y])
                {
                    return true;
                }
                if (board[X + d, Y - d] == 3 - board[X, Y])
                {
                    return false;
                }
            }
        }
        if (dir == 7)
        {
            for (int d = 1; d <= Mathf.Min(X, boardSize - Y - 1); d++)
            {
                if (board[X - d, Y + d] == board[X, Y])
                {
                    return true;
                }
                if (board[X - d, Y + d] == 3 - board[X, Y])
                {
                    return false;
                }
            }
        }
        return false;
    }
}
