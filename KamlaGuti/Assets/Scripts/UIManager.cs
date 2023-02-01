using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button stepBtn;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Canvas hud;
    [SerializeField] private Canvas settings;

    private Text _pauseBtnText;

    public void Awake()
    {
        settings.enabled = false;
        _pauseBtnText = pauseBtn.GetComponentInChildren<Text>();
        
    }

    public void Init()
    {
        _pauseBtnText.text = "Pause";
        if (gameManager.settingsManager.gameManagerParams.stepping)
        {
            pauseBtn.interactable = false;
            stepBtn.interactable = true;
        }
        else
        {
            pauseBtn.interactable = true;
            stepBtn.interactable = false;
        }
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
        }
    }

    public void ShowSetting()
    {
        gameManager.gameStateManager.SetGameState(GameState.Paused);
        settings.enabled = true;
        gameManager.settingsManager.InitializeSettingsPage();
        gameManager.GetBoard().enabled = false;
        hud.enabled = false;
    }

    public void HideSettings()
    {
        gameManager.gameStateManager.SetGameState(GameState.InPlay);
        settings.enabled = false;
        gameManager.GetBoard().enabled = true;
        hud.enabled = true;
    }

    public void Restart()
    {
        gameManager.DeclareWinner();
        gameManager.Restart();  
    } 
        
    
}
