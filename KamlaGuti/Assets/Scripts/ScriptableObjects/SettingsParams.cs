using Player;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SettingsParams", menuName = "KamlaGuti/SettingsParams")]
    public class SettingsParams : ScriptableObject
    {
        public PlayerType redPlayerType;
        public PlayerType greenPlayerType;
        public int redDifficultyInt;
        public int greenDifficultyInt;
        public DifficultyLevel redDifficultyLevel;
        public DifficultyLevel greenDifficultyLevel;
        public AgentType redAgentType;
        public AgentType greenAgentType;
        public bool isSteping;
    }
}
