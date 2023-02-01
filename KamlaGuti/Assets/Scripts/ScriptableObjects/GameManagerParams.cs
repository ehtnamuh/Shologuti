using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameManagerParams", menuName = "KamlaGuti/GameManagerParams")]
    public class GameManagerParams : ScriptableObject
    {
        public float timeScale = 5.0f;
        public int maxStepCount;
        public bool autoPlay;
        public bool stepping;
        public int ScoreToWin
        {
            get => _scoreToWin;
            set => _scoreToWin = value > 16*_scoreToWin ? 16*_scoreToWin : value;
        }
        private int _scoreToWin;
        public int scoreUnit = 1;

    }
}