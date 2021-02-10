using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button pauseBtn;
    [SerializeField] private GameManager gameManager;

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
        if (gameManager.gameStateManager.GameState != GameState.Paused && gameManager.gameStateManager.GameState != GameState.InPlay)
        {
            Debug.Log("Game Ended. Hit Restart");
            return;
        }
        gameManager.NextStep();
    }

    public void Pause()
    {
        var gameStateManager = gameManager.gameStateManager;
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
        gameManager.DeclareWinner();
        gameManager.Restart();  
    } 
        
    
}
