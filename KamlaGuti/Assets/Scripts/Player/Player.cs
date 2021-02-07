using System;

public enum PlayerType
{
    Human = 0,
    AI = 1,
    RLA = 2
    
}

public abstract class Player
{
    protected GutiType gutiType;
    protected GameManager gameManager;

    public int CapturedGutiCount { get; set; }
    public PlayerType PlayerType { get; protected set; }
    
    protected Player() {}

    public abstract void ReInit();

    public abstract Move MakeMove();

    public GutiType GetGutiType() => gutiType;

    public void UpdateScore(Move move)
    {
        if (RuleBook.CanCaptureGuti(move)) CapturedGutiCount++;
    }

    public float GetScore() => CapturedGutiCount * gameManager.scoreboard.ScoreUnit;

}