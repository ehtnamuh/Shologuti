using Player;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] public SettingsParams settingsParams;
    [SerializeField] public GameManagerParams gameManagerParams;
    [SerializeField] private GameManager gameManager;

    // Settings UI Hooks
    // Options
    [SerializeField] private Dropdown redPlType;
    [SerializeField] private Dropdown greenPlType;
    [SerializeField] private Dropdown redDifficulty;
    [SerializeField] private Dropdown greenDifficulty;
    [SerializeField] private Dropdown redAgentType;
    [SerializeField] private Dropdown greenAgentType;
    // Toggles
    [SerializeField] private Toggle steppingToggle;
    [SerializeField] private Toggle autoplayToggle;
    

    private void Start()
    {
        redPlType.onValueChanged.AddListener(RedPlayerTypeChanged);
        greenPlType.onValueChanged.AddListener(GreenPlayerTypeChanged);
        autoplayToggle.onValueChanged.AddListener(AutoPlayToggle);
        steppingToggle.onValueChanged.AddListener(SteppingToggle);
        InitializeSettingsPage();    
        RedPlayerTypeChanged(redPlType.value);
        GreenPlayerTypeChanged(greenPlType.value);
        AutoPlayToggle(autoplayToggle.isOn);
        SteppingToggle(steppingToggle.isOn);
    }
    

    // reads from SettingsParams and initializes the SettingsUI accordingly
    public void InitializeSettingsPage()
    {
        redPlType.value = (int) settingsParams.redPlayerType;
        greenPlType.value = (int) settingsParams.greenPlayerType;
        redDifficulty.value = (int) settingsParams.redDifficultyLevel;
        greenDifficulty.value = (int) settingsParams.greenDifficultyLevel;
        redAgentType.value = (int) settingsParams.redAgentType;
        greenAgentType.value = (int) settingsParams.greenAgentType;
        autoplayToggle.isOn = gameManagerParams.autoPlay;
        steppingToggle.isOn = gameManagerParams.stepping;
    }

    public void SaveSettingsAndReset()
    {
        settingsParams.redPlayerType  = (PlayerType) redPlType.value;
        settingsParams.redAgentType = (AgentType) redAgentType.value;
        settingsParams.redDifficultyLevel = (DifficultyLevel) redDifficulty.value;
        
        settingsParams.greenPlayerType = (PlayerType) greenPlType.value;
        settingsParams.greenDifficultyLevel = (DifficultyLevel) greenDifficulty.value;
        settingsParams.greenAgentType = (AgentType) greenAgentType.value;

        settingsParams.redDifficultyInt = redDifficulty.value + 1;
        settingsParams.greenDifficultyInt = greenDifficulty.value + 1;

        gameManagerParams.stepping = steppingToggle.isOn;
        gameManagerParams.autoPlay = autoplayToggle.isOn;
            
        gameManager.uiManager.HideSettings();
        gameManager.HardRestart();
    }

    private void RedPlayerTypeChanged(int value)
    {
        switch (value)
        {
            case (int) PlayerType.Human:
                redDifficulty.gameObject.SetActive(false);
                redAgentType.gameObject.SetActive(false);
                break;
            case (int) PlayerType.MinMaxAI:
                redDifficulty.gameObject.SetActive(true);
                redAgentType.gameObject.SetActive(false);
                break;
            default:
                redDifficulty.gameObject.SetActive(true);
                redAgentType.gameObject.SetActive(true);
                break;
        }
    }
    
    private void GreenPlayerTypeChanged(int value)
    {
        switch (value)
        {
            case (int) PlayerType.Human:
                greenDifficulty.gameObject.SetActive(false);
                greenAgentType.gameObject.SetActive(false);
                break;
            case (int) PlayerType.MinMaxAI:
                greenDifficulty.gameObject.SetActive(true);
                greenAgentType.gameObject.SetActive(false);
                break;
            default:
                greenDifficulty.gameObject.SetActive(true);
                greenAgentType.gameObject.SetActive(true);
                break;
        }
    }
    
    private void AutoPlayToggle(bool isOn) => autoplayToggle.gameObject.GetComponentInChildren<Text>().text = isOn ? "ON" : "OFF";

    private void SteppingToggle(bool isOn) => steppingToggle.gameObject.GetComponentInChildren<Text>().text = isOn ? "ON" : "OFF";
}
