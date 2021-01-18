using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.MLAgents;
using UnityEngine;

public class MinMaxAI
{
    public GutiMap _gutiMap;
    private int scoreUnit;

    public MinMaxAI(GutiMap _gutiMap, int scoreUnit)
    {
        this._gutiMap = _gutiMap;
        this.scoreUnit = scoreUnit;
    }
    
    // TODO: Need a way to check if game ended while exploring
    public Move MinMax(GutiType gutiType, int explorationDepth, ref int projectedScore)
    {
        if(explorationDepth <= 0) return null;
        var moveList = ExtractMoves(gutiType);
        int maxScore = -2;
        var selectedMove = moveList.Count>0?moveList[0]: new Move();
        foreach (var move in moveList)
        {
            var tempExplorationDepth = explorationDepth;
            var score = 0;
            score += MoveGuti(move);
            if (_gutiMap.HasCapturableGuti(move.sourceAddress, move.targetAddress) && _gutiMap.HasCapturableGuti(move.targetAddress))
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
                if (Random.value > 0.5)
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

    private int MoveGuti(Move move)
    {
        if (move == null) return 0;
        _gutiMap.CaptureGuti(move.sourceAddress, move.targetAddress);
        // _moveStack.Push(move);
        return _gutiMap.HasCapturableGuti(move.sourceAddress, move.targetAddress) ? scoreUnit : 0;
    }

    private int MoveGutiV2(Move move, GutiMap gutiMap)
    {
        if (move == null) return 0;
        gutiMap.CaptureGuti(move.sourceAddress, move.targetAddress);
        return gutiMap.HasCapturableGuti(move.sourceAddress, move.targetAddress) ? scoreUnit : 0;
    }
    
    private void ReverseMove(GutiType gutiType, Move hooch)
    {
        if(hooch == null) return;
        // var move = _moveStack.Pop();
        var move = hooch;
        _gutiMap.MoveGuti(move.targetAddress, move.sourceAddress);
        if (_gutiMap.HasCapturableGuti(move.sourceAddress, move.targetAddress))
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
            IEnumerable<Address> walkableAddress = _gutiMap.GetWalkableNeighbours(source);
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
            MoveGuti(move);
            // gutiTypeTree.Add(_gutiMap.GetGutiTypeList());
            gutiTypeTree.Add(_gutiMap.GetGutiTypeList());
            ReverseMove(gutiType, move);
        }
        return gutiTypeTree;
    }
    
    // public 
    
    public List<List<float>> ParallelGetGutiTypeTree(GutiType gutiType, List<Move> moveList)
    {
        // var moveList = ExtractMoves(gutiType);
        var gutiTypeTree = new List<List<float>>();
        object balanceLock = new object();
        Parallel.ForEach(moveList, move =>
        {
            GutiMap gutiMap = new GutiMap(_gutiMap);
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