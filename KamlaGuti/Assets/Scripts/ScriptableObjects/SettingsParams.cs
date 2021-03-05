using Player;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SettingsParams", menuName = "KamlaGuti/SettingsParams")]
    public class SettingsParams : ScriptableObject
    {
        public PlayerType playerOneType;
        public PlayerType playerTwoType;
        public int difficultyLevelOne;
        public int difficultyLevelTwo;
    }
}
