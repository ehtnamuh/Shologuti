using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public enum PlayerType
{
    Human = 0,
    Ai = 1,
    RLA = 2
    
}

public class Move
{
    public Address sourceAddress;
    public Address targetAddress;
    public GutiType capturedGutiType;
    public GutiType sourceGutiType;

    public Move(Address sourceAddress, Address targetAddress)
    {
        this.sourceAddress = sourceAddress;
        this.targetAddress = targetAddress;
    }

    public override string ToString()
    {
        return $"Source: {sourceAddress} || Target {targetAddress}";
    }

    public Move()
    {
    }
}


public class Player
{
    private  GutiType _gutiType;
    private  GameManager _gameManager;
    private  MinMaxAI _minMaxAi;
    private  List<Move> _moveList;
    
    public int CapturedGutiCount { get; set; }
    public GutiAgent agent;
    public Move SelectedMove { get; set; }
    public  PlayerType playerType;
    private int _explorationDepth;
    
    
    public Player(GutiType gutiType, PlayerType tPlayerType, GameManager gm, int explorationDepth = 1)
    {
        Init(gutiType, tPlayerType, gm, explorationDepth);
    }

    public void Init(GutiType gutiType, PlayerType tPlayerType, GameManager gm, int explorationDepth = 1)
    {
        _explorationDepth = explorationDepth;
        _gameManager = gm;
        CapturedGutiCount = 0;
        _gutiType = gutiType;
        playerType = tPlayerType;
        SelectedMove = null;
        if (playerType != PlayerType.Human) _minMaxAi = new MinMaxAI(gm.board.GetGutiMap(), _gutiType, gm.scoreUnit);
    }

    public void ReInit()
    {
        CapturedGutiCount = 0;
        SelectedMove = null;
    }
    
    public bool MakeMove()
    {
        Move move;
        switch (playerType)
        {
            case PlayerType.Ai:
            {
                _minMaxAi._gutiMap = _gameManager.board.GetGutiMap();
                var _ = 0;
                move = _minMaxAi.MinMax(_gutiType, _explorationDepth, ref _);
                break;
            }
            case PlayerType.Human when SelectedMove == null:
                return true;
            case PlayerType.Human:
                move = SelectedMove;
                break;
            case PlayerType.RLA:
            {
                _minMaxAi._gutiMap = _gameManager.board.GetGutiMap();
                _moveList = _minMaxAi.ExtractMoves(_gutiType);
                // var gutiTypeTree = _minMaxAi.GetGutiTypeTree(_gutiType, _moveList);
                // agent.PopulateGutiTypeTree(gutiTypeTree);
                var gutiTypeTree = _minMaxAi.ParallelGetGutiTypeTree(_gutiType, _moveList);
                agent.PopulateGutiTypeTree(gutiTypeTree);
                agent.RequestDecision();
                return true;
            }
            default:
                throw new Exception("No PlayerType Set");
        }
        _gameManager.board.MoveGuti(move);
        SelectedMove = null;
        if (_gameManager.board.HasCapturedGuti(move)) this.CapturedGutiCount++;
        return CanContinueTurn(move);
    }

    public bool AgentMove(int maxIndex)
    {
        if(playerType != PlayerType.RLA) Debug.Log("not RLA Agent");
        try
        {
            var move = _moveList[maxIndex];
            _gameManager.board.MoveGuti(move);
            if (_gameManager.board.HasCapturedGuti(move)) this.CapturedGutiCount++;
            return CanContinueTurn(move);
        }
        catch (Exception e)
        {
            Debug.Log("AgentMove in Player Broke at Index:" + maxIndex);
            Debug.Log(e);
        }
        return true;
    }

    public bool CanContinueTurn(Move move) => (_gameManager.board.HasCapturedGuti(move) && _gameManager.board.HasCapturableGuti(move.targetAddress));

    public int GetScore() => CapturedGutiCount * _gameManager.scoreUnit;

    public MinMaxAI GetMinMaxAI() => _minMaxAi;

    public override string ToString()
    {
        if (playerType != PlayerType.Ai)
            return $"Type: {playerType}\nColor: {_gutiType}\nScore: {CapturedGutiCount}";
        else
            return $"Type: {playerType}\nDepth: {_explorationDepth}\nColor: {_gutiType}\nScore: {CapturedGutiCount}";
    }
    
}