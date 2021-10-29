using System;
using Board.Guti;
using Player;
using UnityEngine;

public class PlayerSpawner: MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AgentSpawner agentSpawner;
    [SerializeField] private SettingsManager settingsManager;
    // [SerializeField] private SettingsParams settingsManager.settingsParams;
    
    // Spawn Human Player
    public BasePlayer SpawnPlayer(PlayerType playerType, GutiType gutiType)
    {
        if (playerType == PlayerType.Human)
            return new PlayerHuman(gutiType, PlayerType.Human, gameManager);
        return new PlayerMinMax(gutiType, PlayerType.MinMaxAI, gameManager, new MinMaxAi(gameManager.simulator), 1);
    }

    // Spawn Adversarial Player
    public BasePlayer SpawnPlayer(PlayerType playerType, GutiType gutiType, int difficultyLevel)
    {
        if (playerType == PlayerType.MinMaxAI)
            return new PlayerMinMax(gutiType, PlayerType.MinMaxAI, gameManager, new MinMaxAi(gameManager.simulator), difficultyLevel);
        return new PlayerRla(gutiType, PlayerType.RLAgent, gameManager,
            agentSpawner.SpawnAgent(AgentType.AgentSAC, DifficultyLevel.Easy, gutiType));
    }

    // Spawn RLA Player 
    public BasePlayer SpawnPlayer(PlayerType playerType, GutiType gutiType, AgentType agentType, DifficultyLevel difficultyLevel)
    {
        if (playerType != PlayerType.RLAgent) throw new ArgumentException();
        var agent = agentSpawner.SpawnAgent(agentType, difficultyLevel, gutiType);
        return new PlayerRla(gutiType, PlayerType.RLAgent, gameManager, agent);
    }

    public BasePlayer SpawnPlayer(GutiType gutiType)
    {
        var guti = GutiType.RedGuti;
        if (gutiType == guti)
        {
            switch (settingsManager.settingsParams.redPlayerType)
            {
                case PlayerType.Human:
                    return SpawnPlayer(PlayerType.Human, guti);
                case PlayerType.MinMaxAI:
                    return SpawnPlayer(PlayerType.MinMaxAI, guti, settingsManager.settingsParams.redDifficultyInt);
                case PlayerType.RLAgent when settingsManager.settingsParams.redAgentType == AgentType.AgentTD:
                    return SpawnPlayer(PlayerType.RLAgent, guti, AgentType.AgentTD, settingsManager.settingsParams.redDifficultyLevel);
                case PlayerType.RLAgent when settingsManager.settingsParams.redAgentType == AgentType.AgentPPO:
                    return SpawnPlayer(PlayerType.RLAgent, guti, AgentType.AgentPPO, settingsManager.settingsParams.redDifficultyLevel);
                case PlayerType.RLAgent when settingsManager.settingsParams.redAgentType == AgentType.AgentSAC:
                    return SpawnPlayer(PlayerType.RLAgent, guti, AgentType.AgentSAC, settingsManager.settingsParams.redDifficultyLevel);
                default:
                    return SpawnPlayer(PlayerType.Human, guti);
            }
        }
        
        guti = GutiType.GreenGuti;
        switch (settingsManager.settingsParams.greenPlayerType)
        {
            case PlayerType.Human:
                return SpawnPlayer(PlayerType.Human, guti);
            case PlayerType.MinMaxAI:
                return SpawnPlayer(PlayerType.MinMaxAI, guti, settingsManager.settingsParams.greenDifficultyInt);
            case PlayerType.RLAgent when settingsManager.settingsParams.greenAgentType == AgentType.AgentTD:
                return SpawnPlayer(PlayerType.RLAgent, guti, AgentType.AgentTD, settingsManager.settingsParams.greenDifficultyLevel);
            case PlayerType.RLAgent when settingsManager.settingsParams.greenAgentType == AgentType.AgentPPO:
                return SpawnPlayer(PlayerType.RLAgent, guti, AgentType.AgentPPO, settingsManager.settingsParams.greenDifficultyLevel);
            case PlayerType.RLAgent when settingsManager.settingsParams.greenAgentType == AgentType.AgentSAC:
                return SpawnPlayer(PlayerType.RLAgent, guti, AgentType.AgentSAC, settingsManager.settingsParams.greenDifficultyLevel);
            default:
                return SpawnPlayer(PlayerType.Human, guti);
        }

    }
    //settings params 
}
