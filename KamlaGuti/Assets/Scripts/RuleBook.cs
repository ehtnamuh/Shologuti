using System;
using System.Linq;
using Board.Guti;

public static class RuleBook
{
    public static int maxStepCount;
    public static int winningScore;

    public static bool CanContinueTurn(Move move, GutiMap _gutiMap) => (_gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) &&  _gutiMap.CanCaptureGuti(move.targetAddress));

    public static bool CanCaptureGuti(Move move, GutiMap gutiMap)
    {
        return gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress);
    }

    public static bool IsMoveValid(Move move, GutiType gutiType, GutiMap gutiMap)
    {
        var walkableNodes = gutiMap.GetWalkableNodes(move.sourceAddress);
        return gutiMap.GetGutiType(move.sourceAddress) == gutiType && walkableNodes.Contains(move.targetAddress);
    }
    
    public static bool MaxStepExceeded() => throw new NotImplementedException();
}
