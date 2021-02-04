using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private int minMaxReward = 1;
    [SerializeField] private int minMaxPenalty = 1;
    public GutiMap gutiMap;


    private void Start() => gutiMap = board.GetGutiMap();

    public void MakeReady() => gutiMap = board.GetGutiMap();

    public void MoveGuti(Move move, GutiType gutiType)
    {
        if (move == null) return;
        gutiMap.CaptureGuti(move.sourceAddress, move.targetAddress);
    }
    
    public int PredictMoveValue(Move move, GutiType playerGutiType ,GutiType gutiType)
    {
        var captureScore = playerGutiType == gutiType ? minMaxReward : minMaxPenalty;
        return gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) ? captureScore : 0;
    }
    
    public void ReverseMove(GutiType gutiType, Move move)
    {
        if(move == null) return;
        gutiMap.MoveGuti(move.targetAddress, move.sourceAddress);
        if (!gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress)) return;
        var capturedGuti = gutiMap.GetCapturedGutiAddress(move.sourceAddress, move.targetAddress);
        var tempGutiType = ChangeGutiType(gutiType);
        gutiMap.RestoreGuti(capturedGuti, tempGutiType);
    }
    
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

    public List<List<float>> GetBoardMapAsList(GutiType gutiType, List<Move> moveList)
    {
        var gutiTypeTree = new List<List<float>>();
        for (var index = 0; index < moveList.Count; index++)
        {
            var move = moveList[index];
            MoveGuti(move, gutiType);
            gutiTypeTree.Add(gutiMap.GetBoardStateList());
            ReverseMove(gutiType, move);
        }
        return gutiTypeTree;
    }

    public static GutiType ChangeGutiType(GutiType gutiType) => gutiType == GutiType.GreenGuti ? GutiType.RedGuti : GutiType.GreenGuti;
    
    public bool CanContinueTurn(Move move) => (board.HasCapturedGuti(move) &&  board.CanCaptureGuti(move.targetAddress));

}
