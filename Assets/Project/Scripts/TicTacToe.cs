using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TicTacToeState
{
    Initial,
    Idle,
    Active,
    Finished
}
public enum GameMode
{
    Single,
    Duo
}
public enum Difficulty
{
    Easy,
    Difficult,
    Legendary
}

public class TicTacToe : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float finishedToIdleTransitionTime = 3;
    [SerializeField] private GameMode gameMode = GameMode.Single;
    [SerializeField] private Difficulty difficulty = Difficulty.Easy;
    [Header("Game State")]
    [SerializeField] private TicTacToeState gameState = TicTacToeState.Initial; // since the game states/game itself are super simple i'm just going to use enums instead of a proper state pattern
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
    [SerializeField] private Image[] boardWinningBarUI = new Image[8]; // using a 1D array here because i can serialize it to easily drag my references

    void Start()
    {
        remainingTime = maxTimePerTurn;
        TransitionState(TicTacToeState.Idle);
    }

    void Update()
    {
        TickState();
        UpdateScreenSpaceUI();
    }

    public void UpdateScreenSpaceUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        remainingTimeText.text = "Remaining Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");

        if (currentTurn == 1)
        {
            turnText.text = player1Name;
        }
        else turnText.text = player2Name;
    }

    public void ClickUpdateBoardData(int index, int player)
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
            NextTurn();
            elapsedGameTime = 0;
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

    public void ResetBars()
    {
        for (int i = 0; i < boardWinningBarUI.Length; i++)
        {
            TweenImageFill(boardWinningBarUI[i], boardWinningBarUI[i].fillAmount, 0, .25f);
        }
    }

    public void UpdateBoardUI()
    {
        Debug.Log("calling update board UI");
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                int index = x * 3 + y; // convert to 1D array
                if (boardData[x, y] == 1)
                {
                    boardUI[index].text = "X"; // set to "X" if player 1
                    if (boardUI[index].alpha < 1)
                        TweenTextAlpha(boardUI[index], 0f, 1f, .1f);
                }
                else if (boardData[x, y] == 2)
                {
                    boardUI[index].text = "O"; // set to "O" if player 2
                    if (boardUI[index].alpha < 1)
                        TweenTextAlpha(boardUI[index], 0f, 1f, .1f);

                }
                else if (boardData[x, y] == 0)
                {
                    //boardUI[index].text = ""; // clear the text if the cell is empty
                    if (boardUI[index].alpha > 0)
                        TweenTextAlpha(boardUI[index], 1f, 0f, .25f);
                    // boardUI[index].alpha = 0f;
                }
                //TweenTextAlpha(boardUI[index].text, boardUI[index].alpha)
            }
        }
    }
    public void CheckForCompletion()
    {
        int winner = 0;
        int barIndex = -1; // this will keep track of which bar to activate. i don't love this approach but it's pretty fast

        // check rows for a win
        for (int x = 0; x < 3; x++)
        {
            if (boardData[x, 0] == boardData[x, 1] && boardData[x, 1] == boardData[x, 2] && boardData[x, 0] != 0)
            {
                winner = boardData[x, 0];
                barIndex = x; // 0, 1, 2 correspond to Horizontal 1, 2, 3
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
                    barIndex = 3 + y; // 3, 4, 5 correspond to Vertical 1, 2, 3
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
                barIndex = 6; // diagonal 1 from bottom to top
            }
            else if (boardData[0, 2] == boardData[1, 1] && boardData[1, 1] == boardData[2, 0] && boardData[0, 2] != 0)
            {
                winner = boardData[2, 0];
                barIndex = 7; // diagonal 2 from top to bottom
            }
        }

        if (winner != 0)
        {
            Debug.Log("Player " + winner + " wins!");
            if (barIndex != -1)
                TweenImageFill(boardWinningBarUI[barIndex], boardWinningBarUI[barIndex].fillAmount, 1, 1f);

            TransitionState(TicTacToeState.Finished);
        }
    }
    private void NextTurn()
    {
        StartCoroutine(CoNextTurn());
    }
    private IEnumerator CoNextTurn()
    {
        if (gameMode == GameMode.Duo)
            currentTurn = (currentTurn == 1) ? 2 : 1;
        else if (gameMode == GameMode.Single)
        {
            yield return new WaitForSeconds(1f);
            currentTurn = (currentTurn == 1) ? 2 : 1;
            PickRandomCell();
            currentTurn = (currentTurn == 1) ? 2 : 1;
            elapsedGameTime = 0;
        }
        yield break;
    }

    private void IdlePenaltyCheck()
    {
        if (remainingTime <= 0)
        {
            PickRandomCell();
            NextTurn();
            elapsedGameTime = 0;
        }
    }

    private void PickRandomCell()
    {
        bool anyFreeCells = false;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boardData[i, j] == 0)
                {
                    anyFreeCells = true;
                    break;
                }
            }
            if (anyFreeCells) break;
        }

        // only proceed if there is at least one free cell
        if (anyFreeCells)
        {
            while (true)
            {
                int x = Random.Range(0, 3);
                int y = Random.Range(0, 3);

                if (boardData[x, y] == 0) // check if the spot is available
                {
                    boardData[x, y] = currentTurn; // place the current player's mark
                    UpdateBoardUI();
                    CheckForCompletion();
                    break;
                }
            }
        }
        else
        {
            Debug.Log("All cells are filled. No moves possible.");
            TransitionState(TicTacToeState.Finished);
        }
    }


    public void ClickedCell(int cellIndex)
    {
        if (gameMode == GameMode.Single && currentTurn == 1) // if it's an AI game only let them click when it's their turn
            ClickUpdateBoardData(cellIndex, currentTurn);
        else ClickUpdateBoardData(cellIndex, currentTurn);
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
            IdlePenaltyCheck();
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
        if (gameState == toState) //same state
            yield break;

        //handle what needs to happen when we transition from a state
        if (gameState == TicTacToeState.Idle)
        {
            Debug.Log("transitioning from idle");
            //animate the idle UI away
        }
        else if (gameState == TicTacToeState.Active)
        {
            Debug.Log("transitioning from active");
            //animate the game UI away
        }
        else if (gameState == TicTacToeState.Finished)
        {
            Debug.Log("transitioning from finished");
        }

        //handle what needs to happen when we transition to a state
        if (toState == TicTacToeState.Active)
        {
            Debug.Log("transitioning to idle");
            gameState = TicTacToeState.Active;
            currentTurn = firstTurn;
            // play title screen closed logic
            // do camera transition
            // animate UI elements
            // play some character animations
            // play countdown
        }
        else if (toState == TicTacToeState.Finished)
        {
            Debug.Log("transitioning to finished");
            gameState = TicTacToeState.Finished;
            // play congratulations
            // play some character animations
            yield return new WaitForSeconds(finishedToIdleTransitionTime);
            TransitionState(TicTacToeState.Idle);
        }
        else if (toState == TicTacToeState.Idle)
        {
            Debug.Log("transitioning to idle");
            gameState = TicTacToeState.Idle;
            firstTurn = (firstTurn == 1) ? 2 : 1; // alternate the player that goes first
            ResetBoardData();
            ResetBars();
            // fly camera back to the idle state
            // animate UI elements
            // play title screen open logic
            // swap the player who gets to start the game
        }
        yield break;
    }

    private void TweenImageFill(Image image, float startFill, float endFill, float time)
    {
        StartCoroutine(CoTweenImageFill(image, startFill, endFill, time));
    }
    private IEnumerator CoTweenImageFill(Image image, float startFill, float endFill, float time)
    {
        float elapsed = 0f;
        image.fillAmount = startFill;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            image.fillAmount = Mathf.SmoothStep(startFill, endFill, elapsed / time);
            yield return null;
        }

        image.fillAmount = endFill; // Ensure it sets to endFill exactly at the end
    }
    private void TweenTextAlpha(TextMeshProUGUI text, float startAlpha, float endAlpha, float time)
    {
        StartCoroutine(CoTweenTextAlpha(text, startAlpha, endAlpha, time));
    }
    private IEnumerator CoTweenTextAlpha(TextMeshProUGUI text, float startAlpha, float endAlpha, float time)
    {
        float elapsed = 0f;
        text.alpha = startAlpha;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            text.alpha = Mathf.SmoothStep(startAlpha, endAlpha, elapsed / time);
            yield return null;
        }

        text.alpha = endAlpha; // Ensure it sets to endFill exactly at the end
    }

}
