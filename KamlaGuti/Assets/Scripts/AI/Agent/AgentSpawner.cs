using System.Collections.Generic;
using Board.Guti;
using Unity.MLAgents;
using UnityEngine;

public enum DifficultyLevel
{
        Easy = 0,
        Hard = 1
}

public readonly struct AgentPrefabIdentifier
{
        public override bool Equals(object obj)
        {
                return obj is AgentPrefabIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
                unchecked
                {
                        return ((int) _agentType * 397) ^ (int) _difficultyLevel;
                }
        }

        private readonly AgentType _agentType;
        private readonly DifficultyLevel _difficultyLevel;

        public AgentPrefabIdentifier(AgentType agentType, DifficultyLevel difficultyLevel)
        {
                _agentType = agentType;
                _difficultyLevel = difficultyLevel;
        }

        private bool Equals(AgentPrefabIdentifier other) => (_agentType == other._agentType && _difficultyLevel == other._difficultyLevel);

        public static bool operator ==(AgentPrefabIdentifier a, AgentPrefabIdentifier b) => (a._agentType == b._agentType && a._difficultyLevel == b._difficultyLevel);

        public static bool operator !=(AgentPrefabIdentifier a, AgentPrefabIdentifier b) => !(a == b);
}

public class AgentSpawner : MonoBehaviour
{
        [SerializeField] private GutiAgent sacAgentPrefabEasy;
        [SerializeField] private GutiAgent sacAgentPrefabHard;
        [SerializeField] private GutiAgent ppoAgentPrefabEasy;
        [SerializeField] private GutiAgent ppoAgentPrefabHard;
        [SerializeField] private GutiAgent tdAgentPrefab;
        [SerializeField] private GameManager gameManager;
        
        private Dictionary<AgentPrefabIdentifier, GutiAgent> _agentDictionary;
        // has prefabs to all possible agents and their difficulties
        // for now there is only two difficulties difficulty

        public void Awake()
        {
                _agentDictionary = new Dictionary<AgentPrefabIdentifier, GutiAgent>();
                _agentDictionary[new AgentPrefabIdentifier(AgentType.AgentTD, DifficultyLevel.Easy)] = tdAgentPrefab;
                _agentDictionary[new AgentPrefabIdentifier(AgentType.AgentTD, DifficultyLevel.Hard)] = tdAgentPrefab;
                _agentDictionary[new AgentPrefabIdentifier(AgentType.AgentPPO, DifficultyLevel.Easy)] = ppoAgentPrefabEasy;
                _agentDictionary[new AgentPrefabIdentifier(AgentType.AgentPPO, DifficultyLevel.Hard)] = ppoAgentPrefabHard;
                _agentDictionary[new AgentPrefabIdentifier(AgentType.AgentSAC, DifficultyLevel.Easy)] = sacAgentPrefabEasy;
                _agentDictionary[new AgentPrefabIdentifier(AgentType.AgentSAC, DifficultyLevel.Hard)] = sacAgentPrefabHard;
        }

        public GutiAgent SpawnAgent(AgentType agentType, DifficultyLevel difficultyLevel, GutiType gutiType)
        {
                GutiAgent agentPrefab;
                var agentPrefabIdentifier = new AgentPrefabIdentifier(agentType, difficultyLevel);
                if (_agentDictionary.ContainsKey(agentPrefabIdentifier))
                {
                        agentPrefab = _agentDictionary[agentPrefabIdentifier];
                }
                else
                {
                        Debug.Log("Agent Type and Difficulty Combination does not exist");
                        agentPrefab = sacAgentPrefabHard;
                } 
                var agent  = Instantiate(agentPrefab, gameObject.transform, true);
                agent.gameManager = gameManager;
                agent.gutiType = gutiType;
                return agent;
        }
        
}
