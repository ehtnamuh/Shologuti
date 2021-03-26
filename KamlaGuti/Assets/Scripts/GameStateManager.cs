using Board.Guti;
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
    [SerializeField] private Text gameStatusText;
    public GameState GameState { get; set; } = GameState.InPlay;
    
    public void Awake() => gameStatusText.enabled = false;

    public void SetGameState(GameState gameState)
    {
        GameState = gameState;
        GameStateGuiUpdater.UpdateGameStateGui(gameStatusText, gameState);
    }
    
    public void SetGameEndState(GutiType gutiType)
    {
        var gameState = GameState.Draw;
        switch (gutiType)
        {
            case GutiType.GreenGuti:
                gameState = GameState.GreenWin;
                break;
            case GutiType.RedGuti:
                gameState = GameState.RedWin;
                break;
            default:
                gameState = GameState.Draw;
                break;
        }
        SetGameState(gameState);
    }

    public bool HasGameEnded() => GameState == GameState.Draw || GameState == GameState.GreenWin || GameState == GameState.RedWin;
}

internal static class GameStateGuiUpdater
{
    public static void UpdateGameStateGui(Text gameStatusText, GameState gameState)
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
                gameStatusText.color = Color.yellow;
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


