using System.Collections.Generic;
using Board.Guti;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    [SerializeField] private Board.Board board;
    [SerializeField] private int minMaxReward = 1;
    [SerializeField] private int minMaxPenalty = 1;
    public GutiMap gutiMap;


    private void Start() => gutiMap = board.GetGutiMap();

    public void LoadMap() => gutiMap = board.GetGutiMap();

    public void UnloadMap() => gutiMap = null;

    public void MoveGuti(Move move) => gutiMap.CaptureGuti(move.sourceAddress, move.targetAddress);

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
        var tempGutiType = GutiNode.ChangeGutiType(gutiType);
        gutiMap.RestoreGuti(capturedGuti, tempGutiType);
    }
    
    public List<Move> ExtractMoves(GutiType gutiType)
    {
        var playerGutiAddress = gutiMap.GetGutiListOfType(gutiType);
        var list = new List<Move>();
        foreach (var source in playerGutiAddress)
        {
            IEnumerable<Address> walkableAddress = gutiMap.GetWalkableNodes(source);
            foreach (var target in walkableAddress) list.Add(new Move(source, target));
        }
        return list;
    }

    // Returns all future board states upto depth of 1 as Lists of floats
    public List<List<float>> GetAllFutureBoardStatesAsList(GutiType gutiType, List<Move> moveList)
    {
        var gutiTypeTree = new List<List<float>>();
        for (var index = 0; index < moveList.Count; index++)
        {
            var move = moveList[index];
            MoveGuti(move);
            gutiTypeTree.Add(gutiMap.GetBoardStateAsList());
            ReverseMove(gutiType, move);
        }
        return gutiTypeTree;
    }
    
    
    public List<float> GetCurrentBoardStateAsList() => gutiMap.GetBoardStateAsList();
}
