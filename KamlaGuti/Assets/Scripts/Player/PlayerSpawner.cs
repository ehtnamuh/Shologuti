using System;
using Board.Guti;
using Player;
using UnityEngine;

public class PlayerSpawner: MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AgentSpawner agentSpawner;
    
    public BasePlayer SpawnPlayer(PlayerType playerType, GutiType gutiType)
    {
        if (playerType == PlayerType.Human)
            return new PlayerHuman(gutiType, PlayerType.Human, gameManager);
        return new PlayerMinMax(gutiType, PlayerType.AI, gameManager, new MinMaxAi(gameManager.simulator), 1);
    }

    public BasePlayer SpawnPlayer(PlayerType playerType, GutiType gutiType, int difficultyLevel)
    {
        if (playerType == PlayerType.AI)
            return new PlayerMinMax(gutiType, PlayerType.AI, gameManager, new MinMaxAi(gameManager.simulator), difficultyLevel);
        return new PlayerRla(gutiType, PlayerType.RLA, gameManager,
            agentSpawner.SpawnAgent(AgentType.AgentSAC, DifficultyLevel.Easy, gutiType));
    }

    public BasePlayer SpawnPlayer(PlayerType playerType, GutiType gutiType, AgentType agentType, DifficultyLevel difficultyLevel)
    {
        if (playerType != PlayerType.RLA) throw new ArgumentException();
        var agent = agentSpawner.SpawnAgent(agentType, difficultyLevel, gutiType);
        return new PlayerRla(gutiType, PlayerType.RLA, gameManager, agent);
    }
}
