using System;
using System.Collections.Generic;

public enum PlayerType
{
    Human = 0,
    AI = 1,
    RLA = 2
    
}

public class Player
{
    private readonly GutiType _gutiType;
    private readonly GameManager _gameManager;
    private readonly MinMaxAi _minMaxAi;

    private readonly GutiAgent _agent;
    public int CapturedGutiCount { get; private set; }
    public Move SelectedMove { get; set; }
    public PlayerType PlayerType { get; private set; }
    private int _explorationDepth;

    
    
    public Player(GutiType gutiType, PlayerType tPlayerType, GameManager gameManager, int explorationDepth = -1)
    {
        _explorationDepth = explorationDepth<=0? 1: explorationDepth;
        _gameManager = gameManager;
        _gutiType = gutiType;
        PlayerType = tPlayerType;
        CapturedGutiCount = 0;
        SelectedMove = null;
        if (PlayerType != PlayerType.Human) _minMaxAi = new MinMaxAi(_gutiType, gameManager.simulator);
        if (PlayerType != PlayerType.RLA) return;
        _agent = _gameManager.agent;
        _agent.gutiType = _gutiType;
    }

    public void ReInit(int explorationDepth = -1)
    {
        _explorationDepth = explorationDepth<=0? 1: explorationDepth;
        CapturedGutiCount = 0;
        SelectedMove = null;
    }
    
    public Move MakeMove()
    {
        Move move;
        switch (PlayerType)
        {
            case PlayerType.AI:
            {
                _gameManager.GetBoard().GetGutiMap();
                var _ = 0;
                move = _minMaxAi.MinMax(_gutiType, _explorationDepth, ref _);
                break;
            }
            case PlayerType.Human when SelectedMove == null:
                return null;
            case PlayerType.Human:
                move = SelectedMove;
                break;
            case PlayerType.RLA:
            {
                _agent.MakeMove();
                return null;
            }
            default:
                throw new Exception("No PlayerType Set");
        }
        _gameManager.GetBoard().MoveGuti(move);
        SelectedMove = null;
        UpdateScore(move);
        return move;
    }

    public GutiType GetGutiType() => _gutiType;

    public void UpdateScore(Move move)
    {
        if (_gameManager.GetBoard().HasCapturedGuti(move)) CapturedGutiCount++;
    }

    public float GetScore() => CapturedGutiCount * _gameManager.scoreboard.ScoreUnit;

    public MinMaxAi GetMinMaxAi() => _minMaxAi;

    public override string ToString()
    {
        if (PlayerType != PlayerType.AI)
            return $"Type: {PlayerType}\nColor: {_gutiType}\nScore: {CapturedGutiCount}";
        return $"Type: {PlayerType}\nDepth: {_explorationDepth}\nColor: {_gutiType}\nScore: {CapturedGutiCount}";
    }
    
}