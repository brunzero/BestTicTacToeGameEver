using System.Collections;
using System.Collections.Generic;
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
    [Header("Game State")]
    [SerializeField]
    private TicTacToeState gameState = TicTacToeState.Idle;
    [SerializeField]
    private float elapsedGameTime = 0;
    //since the game states/game itself are super simple i'm just going to use enums instead of a proper state pattern
    void Start()
    {

    }

    void Update()
    {
        if (gameState == TicTacToeState.Active)
        {
            elapsedGameTime += Time.deltaTime;
        }
    }

    public void ClickedCell(int cellIndex)
    {
        Debug.Log("clicked Tic Tac Toe cell number " + cellIndex);
    }

    public void StartGame()
    {
        TransitionState(TicTacToeState.Active);
    }

    public void TransitionState(TicTacToeState toState)
    {
        if (gameState == toState)
            return;
        else if (gameState == TicTacToeState.Idle && toState == TicTacToeState.Active)
        {
            //play title screen closed logic
            //do camera transition
            //animate UI elements
            //play some character animations
            //play countdown
        }
        else if (gameState == TicTacToeState.Active && toState == TicTacToeState.Finished)
        {
            //play congratulations
            //play some character animations
        }
        else if (gameState == TicTacToeState.Finished && toState == TicTacToeState.Idle)
        {
            //fly camera back to the idle state
            //animate UI elements
            //play title screen open logic
        }
        else if (gameState == TicTacToeState.Active && toState == TicTacToeState.Idle)
        {
            //this is a restart state
        }
    }
}
