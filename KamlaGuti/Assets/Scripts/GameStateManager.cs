using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    InPlay = 0,
    RedWin = 1,
    GreenWin = 2,
    Paused = 3,
    Draw = 4
}


public  class GameStateManager: MonoBehaviour
{
    public GameState GameState { get; set; } = GameState.InPlay;
    [SerializeField] private Text gameStatusText;

    public void Awake() => gameStatusText.enabled = false;

    public void SetGameState(GameState gameState)
    {
        GameState = gameState;
        UpdateGameStatus(gameState);
    }
    
    public void SetGameEndState(GutiType gutiType)
    {
        var gameState = gutiType == GutiType.GreenGuti ? GameState.GreenWin : GameState.RedWin;
        SetGameState(gameState);
    }


    private void UpdateGameStatus(GameState gameState)
    {
        gameStatusText.enabled = true;
        switch (gameState)
        {
            case GameState.GreenWin:
                gameStatusText.text = "Green Wins";
                gameStatusText.color = Color.green;
                break;
            case GameState.RedWin:
                gameStatusText.text = "Red Wins";
                gameStatusText.color = Color.red;
                break;
            case GameState.Paused:
                gameStatusText.text = "Game Paused";
                gameStatusText.color = Color.white;
                break;
            case GameState.InPlay:
                gameStatusText.enabled = false;
                gameStatusText.color = Color.white;
                break;
            case GameState.Draw:
                gameStatusText.text = "Draw";
                gameStatusText.color = Color.white;
                break;
        }
    }
    
}
