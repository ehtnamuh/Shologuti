
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button pauseBtn;

    private Text _pauseBtnText;

    public void Awake()
    {
        _pauseBtnText = pauseBtn.GetComponentInChildren<Text>();
    }

    public void Init()
    {
        _pauseBtnText.text = "Pause";
    }
    

    public void Step()
    {
        if (GameManager.instance.gameStateManager.GameState != GameState.Paused && GameManager.instance.gameStateManager.GameState != GameState.InPlay)
        {
            Debug.Log("Game Ended. Hit Restart");
            return;
        }
        GameManager.instance.NextStep();
    }

    public void Pause()
    {
        var gameStateManager = GameManager.instance.gameStateManager;
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
        GameManager.instance.DeclareWinner();
        GameManager.instance.Restart();  
    } 
        
    
}
