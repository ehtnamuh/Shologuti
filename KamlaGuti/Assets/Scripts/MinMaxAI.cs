using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class MinMaxAI
{
    public GutiMap gutiMap;
    private readonly GutiType _playerGutiType;
    private readonly int _captureUnitScore;
    private readonly int _loseUnitScore;

    public MinMaxAI(GutiMap gutiMap, GutiType playerGutiType, int captureUnitScore = 1, int loseUnitScore = 1)
    {
        this.gutiMap = gutiMap;
        this._playerGutiType = playerGutiType;
        this._loseUnitScore = loseUnitScore == 0? 1: Math.Abs(loseUnitScore) ;
        this._captureUnitScore = captureUnitScore == 0? 1: Math.Abs(captureUnitScore);
    }
    
    public Move MinMax(GutiType gutiType, int explorationDepth, ref int projectedScore)
    {
        if(explorationDepth <= 0) return null;
        var moveList = ExtractMoves(gutiType);
        var maxValueMoveList = new List<Move>();
        var maxScore = -2;
        // MoveList.Count 0 indicates end of game
        if (moveList.Count <= 0) return null;
        foreach (var move in moveList)
        {
            var tempExplorationDepth = explorationDepth;
            var score = 0;
            score += PredictMoveValue(move, gutiType);
            MoveGuti(move, gutiType);
            if (gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) && gutiMap.CanCaptureGuti(move.targetAddress))
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
                if(maxValueMoveList.Any())
                    maxValueMoveList.Clear();
                maxValueMoveList.Add(move);
            } 
            else if (maxScore == score)
            {
                maxValueMoveList.Add(move);
            }
            ReverseMove(gutiType, move);
        }
        projectedScore = maxScore;
        return maxValueMoveList[Random.Range(0, maxValueMoveList.Count())];
    }

    private void MoveGuti(Move move, GutiType gutiType)
    {
        if (move == null) return;
        gutiMap.CaptureGuti(move.sourceAddress, move.targetAddress);
    }

    private int PredictMoveValue(Move move, GutiType gutiType)
    {
        var captureScore = _playerGutiType == gutiType ? _captureUnitScore : _loseUnitScore;
        return gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) ? captureScore : 0;
    }
    
    private void ReverseMove(GutiType gutiType, Move hooch)
    {
        if(hooch == null) return;
        // var move = _moveStack.Pop();
        var move = hooch;
        gutiMap.MoveGuti(move.targetAddress, move.sourceAddress);
        if (gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress))
        {
            var capturedGuti = gutiMap.GetCapturedGutiAddress(move.sourceAddress, move.targetAddress);
            var tempGutiType = ChangeGutiType(gutiType);
            gutiMap.RestoreGuti(capturedGuti, tempGutiType);
        }
    }

    private GutiType ChangeGutiType(GutiType gutiType) => gutiType == GutiType.GreenGuti ? GutiType.RedGuti : GutiType.GreenGuti;

    public List<Move> ExtractMoves(GutiType gutiType)
    {
        var playerGutiAddress = gutiMap.GetGutisOfType(gutiType);
        var list = new List<Move>();
        foreach (var source in playerGutiAddress)
        {
            IEnumerable<Address> walkableAddress = gutiMap.GetWalkableNodes(source);
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
            gutiTypeTree.Add(gutiMap.GetGutiTypeList());
            ReverseMove(gutiType, move);
        }
        return gutiTypeTree;
    }

    #region Experimental and Unused
    private int MoveGutiV2(Move move, GutiMap gutiMap)
    {
        if (move == null) return 0;
        gutiMap.CaptureGuti(move.sourceAddress, move.targetAddress);
        return gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) ? 1 : 0;
    }
    
    public List<List<float>> ParallelGetGutiTypeTree(List<Move> moveList, GutiMap meGutiMap)
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
    #endregion
   
}