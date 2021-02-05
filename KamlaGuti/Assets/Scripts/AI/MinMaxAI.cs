using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class MinMaxAi
{
    private readonly Simulator _simulator;
    private readonly GutiType _playerGutiType;
    private readonly int _captureUnitScore;
    private readonly int _loseUnitScore;

    public MinMaxAi(GutiType playerGutiType, Simulator simulator,int captureUnitScore = 1, int loseUnitScore = 1)
    {
        _simulator = simulator;
        _playerGutiType = playerGutiType;
        _loseUnitScore = loseUnitScore == 0? 1: Math.Abs(loseUnitScore) ;
        _captureUnitScore = captureUnitScore == 0? 1: Math.Abs(captureUnitScore);
    }
    
    public Move MinMax(GutiType gutiType, int explorationDepth, ref int projectedScore)
    {
        if(explorationDepth <= 0) return null;
        _simulator.MakeReady();
        var moveList = _simulator.ExtractMoves(gutiType);
        var maxValueMoveList = new List<Move>();
        var maxScore = -2;
        // MoveList.Count 0 indicates end of game
        if (moveList.Count <= 0) return null;
        foreach (var move in moveList)
        {
            var tempExplorationDepth = explorationDepth;
            var score = 0;
            score += _simulator.PredictMoveValue(move,_playerGutiType, gutiType);
            _simulator.MoveGuti(move, gutiType);
            if (_simulator.CanContinueTurn(move))
            {
                var tempScore = 0;
                MinMax(gutiType, --tempExplorationDepth, ref tempScore);
                score += tempScore;
            }
            else
            {
                var tempScore = 0;
                var tempGutiType = Simulator.ChangeGutiType(gutiType);
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
            else if (maxScore == score) maxValueMoveList.Add(move);

            _simulator.ReverseMove(gutiType, move);
        }
        projectedScore = maxScore;
        return maxValueMoveList[Random.Range(0, maxValueMoveList.Count())];
    }
}