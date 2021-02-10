using System;
using System.Collections.Generic;
using System.Linq;
using Board.Guti;
using Random = UnityEngine.Random;

public class MinMaxAi
{
    private readonly Simulator _simulator;
    private readonly int _captureUnitScore;
    private readonly int _loseUnitScore;
    private GutiMap _gutiMap;
    
    public MinMaxAi( Simulator simulator,int captureUnitScore = 1, int loseUnitScore = 1)
    {
        _simulator = simulator;
        _loseUnitScore = loseUnitScore == 0? 1: Math.Abs(loseUnitScore) ;
        _captureUnitScore = captureUnitScore == 0? 1: Math.Abs(captureUnitScore);
    }

    public Move MinMax(GutiType rootGutiType, int explorationDepth, ref int projectedScore)
    {
        _simulator.LoadMap();
        return MinMax(rootGutiType, rootGutiType, explorationDepth, ref projectedScore);
        _simulator.UnloadMap();
    }

    private Move MinMax(GutiType rootGutiType, GutiType gutiType, int explorationDepth, ref int projectedScore)
    {
        if(explorationDepth <= 0) return null;
        var moveList = _simulator.ExtractMoves(gutiType);
        var maxValueMoveList = new List<Move>();
        var maxScore = -2;
        // MoveList.Count 0 indicates end of game
        if (moveList.Count <= 0) return null;
        foreach (var move in moveList)
        {
            var tempExplorationDepth = explorationDepth;
            var score = 0;
            score += _simulator.PredictMoveValue(move, rootGutiType, gutiType);
            _simulator.MoveGuti(move);
            if (RuleBook.CanContinueTurn(move))
            {
                var tempScore = 0;
                MinMax(gutiType, gutiType, --tempExplorationDepth, ref tempScore);
                score += tempScore;
            }
            else
            {
                var tempScore = 0;
                var tempGutiType = GutiNode.ChangeGutiType(gutiType);
                MinMax(gutiType , tempGutiType, --tempExplorationDepth, ref tempScore);
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