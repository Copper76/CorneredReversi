using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] BoardController boardController;
    GameObject selectSquare;
    GameObject hoveringPiece;
    [SerializeField] TextMeshProUGUI finalMessage;

    private int selectX;
    private int selectY;

    //available and unavailable colours
    private Color freeColor = new Color(0, 1f, 0, 0.19f);
    private Color occupiedColor = new Color(1f, 0, 0, 0.19f);

    //Player Colours
    private Color whiteColor = new Color(1f, 1f, 1f, 1f);
    private Color blackColor = new Color(0, 0, 0, 1f);

    private int turn = 0;
    private Color[] turnColors;

    void Awake()
    {
        selectSquare = transform.GetChild(0).gameObject;
        hoveringPiece = selectSquare.transform.GetChild(0).gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        turnColors = new Color[2] { whiteColor, blackColor };
        selectSquare.GetComponent<SpriteRenderer>().color = turnColors[turn];
    }

    void UpdateSelection(int X, int Y)
    {
        selectX = X;
        selectY = Y;
        selectSquare.transform.position = new Vector3(X, Y, -2);
        if (boardController.CellFree(X, Y))
        {
            selectSquare.GetComponent<SpriteRenderer>().color = freeColor;
            hoveringPiece.SetActive(true);
        }
        else
        {
            selectSquare.GetComponent<SpriteRenderer>().color = occupiedColor;
            hoveringPiece.SetActive(false);
        }
    }

    public void MouseMove(InputAction.CallbackContext context)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
        int adjustedPosX = Mathf.RoundToInt(mousePos.x);
        int adjustedPosY = Mathf.RoundToInt(mousePos.y);
        adjustedPosX = adjustedPosX<0? 0: adjustedPosX >= boardController.boardSize ? boardController.boardSize - 1 : adjustedPosX;
        adjustedPosY = adjustedPosY<0? 0: adjustedPosY >= boardController.boardSize ? boardController.boardSize - 1 : adjustedPosY;
        UpdateSelection(adjustedPosX, adjustedPosY);
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (boardController.Place(selectX, selectY, turn)){
            turn = 1 - turn;
            hoveringPiece.GetComponent<SpriteRenderer>().color = turnColors[turn];
            int winner = boardController.CheckVictory();
            if (winner != -1) {
                EndGame(winner);
            }
        }
    }

    public void End(InputAction.CallbackContext context)
    {
        InterruptGame();
    }

    void EndGame(int winner)
    {
        if (winner == 0)
        {
            finalMessage.text = string.Format("WHITE WINS");
        }
        else
        {
            finalMessage.text = string.Format("BLACK WINS");
        }
        finalMessage.transform.parent.gameObject.SetActive(true);
        Destroy(gameObject);
    }

    void InterruptGame()
    {
        if (turn == 0)
        {
            finalMessage.text = string.Format("WHITE FORFEITS");
        }
        else
        {
            finalMessage.text = string.Format("BLACK FORFEITS");
        }
        finalMessage.transform.parent.gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
