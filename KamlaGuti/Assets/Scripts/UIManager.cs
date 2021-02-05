
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Text RedScore;
    [SerializeField] private Text GreenScore;
    [SerializeField] private Text GameStatusText;
    [SerializeField] private Button PauseBtn;

    private Text PauseBtnText;
    private GameManager _gameManager;


    // Start is called before the first frame update
    public void Awake()
    {
        _gameManager = gameObject.GetComponent<GameManager>();
        PauseBtnText = PauseBtn.GetComponentInChildren<Text>();
        GameStatusText.enabled = false;
    }

    public void UpdateScoreboard(GutiType gutiType, String details)
    {
        if (gutiType == GutiType.GreenGuti)
            GreenScore.text = "Player 2\n" + details;
        else
            RedScore.text = "Player 1\n"+ details;
    }

    public void UpdateScoreboard(GutiType gutiType, int score)
    {
        if (gutiType == GutiType.GreenGuti)
            GreenScore.text = $"GreenScore: {score}";
        else
            RedScore.text = $"RedScore: {score}";
    }

    public void Step()
    {
        if (_gameManager.gameStateManager.GameState != GameState.Paused && _gameManager.gameStateManager.GameState != GameState.InPlay)
        {
            Debug.Log("Game Ended. Hit Restart");
            return;
        }
        _gameManager.NextStep();
    }

    public void Pause()
    {
        var gameStateManager = _gameManager.gameStateManager;
        switch (gameStateManager.GameState)
        {
            case GameState.InPlay:
                PauseBtnText.text = "Resume";
                gameStateManager.SetGameState(GameState.Paused);
                break;
            case GameState.Paused:
                PauseBtnText.text = "Pause";
                gameStateManager.SetGameState(GameState.InPlay);
                break;
        }
    }

    public void Restart()
    {
        _gameManager.DeclareWinner();
        _gameManager.Restart();  
    } 
        
    
}
