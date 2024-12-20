﻿using Board.Guti;

namespace Player
{
    public class PlayerHuman: BasePlayer
    {
        public Move SelectedMove { get; set; }

        public PlayerHuman(GutiType gutiType, PlayerType tPlayerType, GameManager gameManager, string name = "PlayerHuman") : base(gameManager)
        {
            this.gutiType = gutiType;
            base.name = name;
            PlayerType = tPlayerType;
            CapturedGutiCount = 0;
            SelectedMove = null;
        }
    
        public override Move GetMove()
        {
            if(SelectedMove == null) return null;
            var move = SelectedMove;
            SelectedMove = null;
            UpdateScore(move);
            return move;
        }
    
        public override void ReInit()
        {
            CapturedGutiCount = 0;
            SelectedMove = null;
        }
    
        public override string ToString() => $"Type: {PlayerType}\nColor: {gutiType}\nScore: {CapturedGutiCount}";
    }
}