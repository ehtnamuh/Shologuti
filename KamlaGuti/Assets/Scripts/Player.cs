using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Human = 0,
    AI = 1,
    RLA = 2
    
}

public class Player
{
    private  GutiType _gutiType;
    private  GameManager _gameManager;
    private  MinMaxAi _minMaxAi;
    private  List<Move> _moveList;
    
    public int CapturedGutiCount { get; private set; }
    public GutiAgent agent;
    public Move SelectedMove { get; set; }
    public PlayerType PlayerType { get; private set; }

    private int _explorationDepth;
    
    
    public Player(GutiType gutiType, PlayerType tPlayerType, GameManager gm, int explorationDepth = -1)
    {
        Init(gutiType, tPlayerType, gm, explorationDepth);
    }

    private void Init(GutiType gutiType, PlayerType tPlayerType, GameManager gameManager, int explorationDepth = -1)
    {
        _explorationDepth = explorationDepth<=0? 1: explorationDepth;
        _gameManager = gameManager;
        _gutiType = gutiType;
        PlayerType = tPlayerType;
        CapturedGutiCount = 0;
        SelectedMove = null;
        if (PlayerType != PlayerType.Human) _minMaxAi = new MinMaxAi(_gutiType, gameManager.simulator);
        if (PlayerType != PlayerType.RLA) return;
        agent = _gameManager.agent;
        agent.gutiType = _gutiType;
    }

    public void ReInit()
    {
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
                var simulator = _gameManager.simulator;
                simulator.gutiMap = _gameManager.GetBoard().GetGutiMap();
                _moveList = simulator.ExtractMoves(_gutiType);
                var gutiTypeTree = simulator.GetBoardMapAsList(_gutiType, _moveList);
                agent.PopulateGutiTypeTree(gutiTypeTree);
                agent.RequestDecision();
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
    
    public Move AgentMove(int maxIndex)
    {
        if(PlayerType != PlayerType.RLA) Debug.Log("not RLA Agent");
        try
        {
            var move = _moveList[maxIndex];
            _gameManager.GetBoard().MoveGuti(move);
            UpdateScore(move);
            return move;
        }           
        catch (Exception e)
        {
            Debug.Log("AgentMove in Player Broke at Index:" + maxIndex);
            Debug.Log(e);
        }
        return null;
    }

    public GutiType GetGutiType() => _gutiType;

    private void UpdateScore(Move move)
    {
        if (_gameManager.GetBoard().HasCapturedGuti(move)) CapturedGutiCount++;
    }

    public float GetScore() => CapturedGutiCount * _gameManager.scoreUnit;

    public MinMaxAi GetMinMaxAi() => _minMaxAi;

    public override string ToString()
    {
        if (PlayerType != PlayerType.AI)
            return $"Type: {PlayerType}\nColor: {_gutiType}\nScore: {CapturedGutiCount}";
        return $"Type: {PlayerType}\nDepth: {_explorationDepth}\nColor: {_gutiType}\nScore: {CapturedGutiCount}";
    }
    
}