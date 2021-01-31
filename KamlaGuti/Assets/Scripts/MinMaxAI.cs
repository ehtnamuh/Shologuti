using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class MinMaxAI
{
    public GutiMap _gutiMap;
    private GutiType playerGutiType;
    private int _captureUnitScore;
    private int _loseUnitScore;

    public MinMaxAI(GutiMap _gutiMap, GutiType playerGutiType, int captureUnitScore = 1, int loseUnitScore = 1)
    {
        this._gutiMap = _gutiMap;
        this.playerGutiType = playerGutiType;
        this._loseUnitScore = loseUnitScore == 0? 1: Math.Abs(loseUnitScore) ;
        this._captureUnitScore = captureUnitScore == 0? 1: Math.Abs(captureUnitScore);
    }
    
    // TODO: Need a way to check if game ended while exploring
    public Move MinMax(GutiType gutiType, int explorationDepth, ref int projectedScore)
    {
        if(explorationDepth <= 0) return null;
        var moveList = ExtractMoves(gutiType);
        var maxScore = -2;
        // MoveList.Count 0 indicates end of game
        if (moveList.Count <= 0) return null;
        var selectedMove = moveList[0];
        foreach (var move in moveList)
        {
            var tempExplorationDepth = explorationDepth;
            var score = 0;
            score += MoveGuti(move, gutiType);
            if (_gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) && _gutiMap.CanCaptureGuti(move.targetAddress))
            {
                var tempScore = 0;
                MinMax(gutiType, --tempExplorationDepth, ref tempScore);
                score += tempScore;
            }
            else
            {
                var tempScore = 0;
                var tempGutiType = ChangeGutiType(gutiType);
                MinMax(tempGutiType, --tempExplorationDepth, ref tempScore);
                score -= tempScore;
            }
            if (maxScore < score)
            {
                maxScore = score;
                selectedMove = move;
            } 
            else if (maxScore == score)
            {
                if (Random.value > 0.8)
                {
                    maxScore = score;
                    selectedMove = move;
                }
            }
            ReverseMove(gutiType, move);
        }
        projectedScore = maxScore;
        return selectedMove;
    }

    private int MoveGuti(Move move, GutiType gutiType)
    {
        if (move == null) return 0;
        var captureScore = playerGutiType == gutiType ? _captureUnitScore : _loseUnitScore;
        _gutiMap.CaptureGuti(move.sourceAddress, move.targetAddress);
        return _gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) ? captureScore : 0;
    }

    private int MoveGutiV2(Move move, GutiMap gutiMap)
    {
        if (move == null) return 0;
        gutiMap.CaptureGuti(move.sourceAddress, move.targetAddress);
        return gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) ? 1 : 0;
    }
    
    private void ReverseMove(GutiType gutiType, Move hooch)
    {
        if(hooch == null) return;
        // var move = _moveStack.Pop();
        var move = hooch;
        _gutiMap.MoveGuti(move.targetAddress, move.sourceAddress);
        if (_gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress))
        {
            var capturedGuti = _gutiMap.GetCapturedGutiAddress(move.sourceAddress, move.targetAddress);
            var tempGutiType = ChangeGutiType(gutiType);
            _gutiMap.RestoreGuti(capturedGuti, tempGutiType);
        }
    }

    private GutiType ChangeGutiType(GutiType gutiType) => gutiType == GutiType.GreenGuti ? GutiType.RedGuti : GutiType.GreenGuti;

    public List<Move> ExtractMoves(GutiType gutiType)
    {
        var playerGutiAddress = _gutiMap.GetGutisOfType(gutiType);
        var list = new List<Move>();
        foreach (var source in playerGutiAddress)
        {
            IEnumerable<Address> walkableAddress = _gutiMap.GetWalkableNodes(source);
            foreach (var target in walkableAddress) list.Add(new Move(source, target));
        }
        return list;
    }

    public List<List<float>> GetGutiTypeTree(GutiType gutiType, List<Move> moveList)
    {
        // var moveList = ExtractMoves(gutiType);
        var gutiTypeTree = new List<List<float>>();
        for (var index = 0; index < moveList.Count; index++)
        {
            var move = moveList[index];
            MoveGuti(move, gutiType);
            gutiTypeTree.Add(_gutiMap.GetGutiTypeList());
            ReverseMove(gutiType, move);
        }
        return gutiTypeTree;
    }
    
    
    public List<List<float>> ParallelGetGutiTypeTree(GutiType gutiType, List<Move> moveList, GutiMap meGutiMap)
    {
        // var moveList = ExtractMoves(gutiType);
        var gutiTypeTree = new List<List<float>>();
        object balanceLock = new object();
        Parallel.ForEach(moveList, move =>
        {
            GutiMap gutiMap = new GutiMap(meGutiMap);
            MoveGutiV2(move, gutiMap);
            var gg = gutiMap.GetGutiTypeList();
            lock (balanceLock)
            {
                gutiTypeTree.Add(gg);
            }
        });
        return gutiTypeTree;
    }
}