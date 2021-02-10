using System;
using System.Linq;
using Board.Guti;

public static class RuleBook
{
    // RuleBook Class has reference of the source GutiMap
    private static GutiMap _gutiMap;
    public static int maxStepCount;
    public static int winningScore;

    public static void Init(GutiMap tGutiMap) => _gutiMap = tGutiMap;

    public static bool CanContinueTurn(Move move) => (_gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) &&  _gutiMap.CanCaptureGuti(move.targetAddress));
    public static bool CanCaptureGuti(Move move) => _gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress);

    public static bool IsMoveValid(Move move, GutiType gutiType)
    {
        var walkableNodes = _gutiMap.GetWalkableNodes(move.sourceAddress);
        return _gutiMap.GetGutiType(move.sourceAddress) == gutiType && walkableNodes.Contains(move.targetAddress);
    }
    
    public static bool MaxStepExceeded() => throw new NotImplementedException();
}
