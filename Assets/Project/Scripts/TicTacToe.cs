using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[SerializeField]
public enum TicTacToeState
{
    Idle,
    Active,
    Finished
}

public class TicTacToe : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float finishedToIdleTransitionTime = 3;
    [Header("Game State")]
    [SerializeField] private TicTacToeState gameState = TicTacToeState.Idle; // since the game states/game itself are super simple i'm just going to use enums instead of a proper state pattern
    [SerializeField] private int currentTurn = 1;
    [SerializeField] private int firstTurn = 1;
    [SerializeField] private float elapsedGameTime = 0;
    [SerializeField] private float maxTimePerTurn = 120;
    [SerializeField] private float remainingTime = 120;
    [Header("Board State")]
    private int[,] boardData = new int[3, 3]; // 0 = empty, 1 = player 1 or x, 2 = player 2 or o
    [Header("Players")]
    [SerializeField] private string player1Name = "Player 1";
    [SerializeField] private string player2Name = "Player 2";
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI remainingTimeText = null;
    [SerializeField] private TextMeshProUGUI turnText = null;
    [SerializeField] private TextMeshProUGUI[] boardUI = new TextMeshProUGUI[9]; // using a 1D array here because i can serialize it to easily drag my references
    [SerializeField] private TextMeshProUGUI[] boardWinningBarUI = new TextMeshProUGUI[8]; // using a 1D array here because i can serialize it to easily drag my references

    void Start()
    {
        remainingTime = maxTimePerTurn;
    }

    void Update()
    {
        TickState();
        UpdateScreenSpaceUI();
    }

    public void UpdateScreenSpaceUI()
    {
        remainingTimeText.text = "Remaining Time: " + Mathf.FloorToInt(remainingTime / 60).ToString("00") + ":" + (remainingTime % 60).ToString("00");
        if (currentTurn == 1)
        {
            turnText.text = player1Name;
        }
        else turnText.text = player2Name;
    }

    public void UpdateBoardData(int index, int player)
    {
        int numRows = 3;
        int row = index / numRows;
        int col = index % numRows;

        // if the spot they clicked is empty
        if (boardData[row, col] == 0)
        {
            boardData[row, col] = player; // set the cell to the current player's number (1 or 2)
            UpdateBoardUI();
            CheckForCompletion();
        }
        else
        {
            Debug.LogWarning("cell is already occupied."); // log error or handle the situation when the cell is not empty
        }
    }

    public void ResetBoardData()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                boardData[x, y] = 0;
            }
        }
        UpdateBoardUI();
    }

    public void UpdateBoardUI()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                int index = x * 3 + y; // convert to 1D array
                if (boardData[x, y] == 1)
                {
                    boardUI[index].text = "X"; // set to "X" if player 1
                }
                else if (boardData[x, y] == 2)
                {
                    boardUI[index].text = "O"; // set to "O" if player 2
                }
                else
                {
                    boardUI[index].text = ""; // clear the text if the cell is empty
                }
            }
        }
    }
    public void CheckForCompletion()
    {
        int winner = 0;

        // check rows for a win
        for (int x = 0; x < 3; x++)
        {
            if (boardData[x, 0] == boardData[x, 1] && boardData[x, 1] == boardData[x, 2] && boardData[x, 0] != 0)
            {
                winner = boardData[x, 0];
                break;
            }
        }

        // check columns for a win only if no winner yet
        if (winner == 0)
        {
            for (int y = 0; y < 3; y++)
            {
                if (boardData[0, y] == boardData[1, y] && boardData[1, y] == boardData[2, y] && boardData[0, y] != 0)
                {
                    winner = boardData[0, y];
                    break;
                }
            }
        }

        // check diagonals for a win only if no winner yet
        if (winner == 0)
        {
            if (boardData[0, 0] == boardData[1, 1] && boardData[1, 1] == boardData[2, 2] && boardData[0, 0] != 0)
            {
                winner = boardData[0, 0];
            }
            else if (boardData[0, 2] == boardData[1, 1] && boardData[1, 1] == boardData[2, 0] && boardData[0, 2] != 0)
            {
                winner = boardData[2, 0];
            }
        }

        if (winner != 0)
        {
            Debug.Log("Player " + winner + " wins!");
            TransitionState(TicTacToeState.Finished);
        }
        else
        {
            // toggle turn if no win
            currentTurn = (currentTurn == 1) ? 2 : 1;
        }
    }

    public void ClickedCell(int cellIndex)
    {
        Debug.Log("clicked Tic Tac Toe cell number " + cellIndex);
        UpdateBoardData(cellIndex, currentTurn);
    }

    public void StartGame()
    {
        TransitionState(TicTacToeState.Active);
    }

    public void EndGame()
    {
        TransitionState(TicTacToeState.Finished);
    }

    public void TickState()
    {
        if (gameState == TicTacToeState.Active)
        {
            elapsedGameTime += Time.deltaTime;
            remainingTime = maxTimePerTurn - elapsedGameTime;
        }
        else if (gameState == TicTacToeState.Idle)
        {
            elapsedGameTime = 0;
            remainingTime = maxTimePerTurn;
        }
    }
    public void TransitionState(TicTacToeState toState)
    {
        StartCoroutine(CoTransitionState(toState));
    }
    public IEnumerator CoTransitionState(TicTacToeState toState)
    {
        if (gameState == toState)
            yield break;
        else if (gameState == TicTacToeState.Idle && toState == TicTacToeState.Active)
        {
            // play title screen closed logic
            // do camera transition
            // animate UI elements
            // play some character animations
            // play countdown
            currentTurn = firstTurn;
            gameState = TicTacToeState.Active;
        }
        else if (gameState == TicTacToeState.Active && toState == TicTacToeState.Finished)
        {
            gameState = TicTacToeState.Finished;
            // play congratulations
            // play some character animations
            yield return new WaitForSeconds(finishedToIdleTransitionTime);
            TransitionState(TicTacToeState.Idle);
        }
        else if (gameState == TicTacToeState.Finished && toState == TicTacToeState.Idle)
        {
            gameState = TicTacToeState.Idle;
            firstTurn = (firstTurn == 1) ? 2 : 1;
            ResetBoardData();
            // fly camera back to the idle state
            // animate UI elements
            // play title screen open logic
            // swap the player who gets to start the game
        }
        else if (gameState == TicTacToeState.Active && toState == TicTacToeState.Idle)
        {
            // this is a restart state
            gameState = TicTacToeState.Idle;
        }

        yield break;
    }
}
