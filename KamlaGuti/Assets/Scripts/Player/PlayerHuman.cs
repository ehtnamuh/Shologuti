﻿public class PlayerHuman: Player
{
    public Move SelectedMove { get; set; }
    
    public PlayerHuman(GutiType gutiType, PlayerType tPlayerType, GameManager gameManager)
    {
        this.gameManager = gameManager;
        this.gutiType = gutiType;
        PlayerType = tPlayerType;
        CapturedGutiCount = 0;
        SelectedMove = null;
    }
    
    public override Move MakeMove()
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