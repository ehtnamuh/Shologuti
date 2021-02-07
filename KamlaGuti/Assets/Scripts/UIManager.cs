
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button pauseBtn;

    private Text _pauseBtnText;
    private GameManager _gameManager;
    
    public void Awake()
    {
        _gameManager = gameObject.GetComponent<GameManager>();
        _pauseBtnText = pauseBtn.GetComponentInChildren<Text>();
    }

    public void Init()
    {
        _pauseBtnText.text = "Pause";
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
                _pauseBtnText.text = "Resume";
                gameStateManager.SetGameState(GameState.Paused);
                break;
            case GameState.Paused:
                _pauseBtnText.text = "Pause";
                gameStateManager.SetGameState(GameState.InPlay);
                break;
            default:
                _pauseBtnText.text = "Pause";
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
