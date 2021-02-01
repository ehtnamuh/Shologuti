
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Text RedScore;
    [SerializeField] private Text GreenScore;
    [SerializeField] private Text GameStatusText;
    [SerializeField] private Button ReplayBtn;
    [SerializeField] private Button PauseBtn;
    [SerializeField] private Button StepBtn;

    private Text PauseBtnText;
    private GameManager _gameManager;

    
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("UI Loaded");
        _gameManager = gameObject.GetComponent<GameManager>();
        PauseBtnText = PauseBtn.GetComponentInChildren<Text>();
        GameStatusText.enabled = false;
        // ReplayBtn.enabled = false;
    }

    public void UpdateScore(GutiType gutiType, String details)
    {
        if (gutiType == GutiType.GreenGuti)
            GreenScore.text = "Player 2\n" + details;
        else
            RedScore.text = "Player 1\n"+ details;
    }

    public void UpdateScore(GutiType gutiType, int score)
    {
        if (gutiType == GutiType.GreenGuti)
            GreenScore.text = $"GreenScore: {score}";
        else
            RedScore.text = $"RedScore: {score}";
    }

    public void UpdateGameStatus(GameState gameState)
    {
        GameStatusText.enabled = true;
        switch (gameState)
        {
            case GameState.GreenWin:
                GameStatusText.text = "Green Wins";
                GameStatusText.color = Color.green;
                break;
            case GameState.RedWin:
                GameStatusText.text = "Red Wins";
                GameStatusText.color = Color.red;
                break;
            case GameState.Paused:
                GameStatusText.text = "Game Paused";
                GameStatusText.color = Color.white;
                PauseBtnText.text = "Resume";
                break;
            case GameState.InPlay:
                GameStatusText.enabled = false;
                GameStatusText.color = Color.white;
                PauseBtnText.text = "Pause";
                break;
            case GameState.Draw:
                GameStatusText.text = "Draw";
                GameStatusText.color = Color.white;
                break;
        }
    }

    public void Step()
    {
        if (_gameManager.GetGameState() != GameState.Paused && _gameManager.GetGameState() != GameState.InPlay)
        {
            Debug.Log("Game Ended. Hit Restart");
            return;
        }
        _gameManager.NextStep();
    }

    public void Pause()
    {
        if (_gameManager.GetGameState() == GameState.InPlay)
        {
            // Time.timeScale =  0f;
            _gameManager.SetGameState(GameState.Paused);
        }
        else if(_gameManager.GetGameState() == GameState.Paused)
        {
            Time.timeScale = 2.0f;
            _gameManager.SetGameState(GameState.InPlay);
        }
        else
        {
            Debug.Log("From PauseBtn: Game already ended");
        }
    }

    public void Restart() => _gameManager.Restart();
}
