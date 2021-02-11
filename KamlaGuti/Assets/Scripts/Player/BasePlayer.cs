using Board.Guti;
using UnityEngine;

namespace Player
{
    public enum PlayerType
    {
        Human = 0,
        AI = 1,
        RLA = 2
    
    }

    public abstract class BasePlayer
    {
        protected GutiType gutiType;
        private GameManager _gameManager;

        public int CapturedGutiCount { get; set; }
        public PlayerType PlayerType { get; protected set; }

        protected BasePlayer(GameManager gameManager) => _gameManager = gameManager;

        public abstract void ReInit();

        public abstract Move GetMove();

        public GutiType GetGutiType() => gutiType;

        public void UpdateScore(Move move)
        {
            if (RuleBook.CanCaptureGuti(move, _gameManager.GetBoard().GetGutiMapRef())) CapturedGutiCount++;
        }

        public float GetScore() => CapturedGutiCount * _gameManager.scoreboard.ScoreUnit;

    }
}

