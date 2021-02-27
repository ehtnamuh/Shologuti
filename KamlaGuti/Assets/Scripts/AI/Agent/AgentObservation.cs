using System.Collections.Generic;
using Board;
using Board.Guti;
using UnityEngine;

public class AgentObservation
{
    // private readonly GutiMap _gutiMap;
    private readonly Simulator _simulator;


    public AgentObservation( Simulator simulator)
    {
        _simulator = simulator;
    }
    
    // Returns all future board states upto depth of 1 as Lists of floats
    public List<List<float>> GetAllFutureBoardStatesAsList(GutiType gutiType, List<Move> moveList)
    {
        var gutiTypeTree = new List<List<float>>();
        gutiTypeTree.Add(_simulator.gutiMap.GetBoardStateAsList());
        for (var index = 0; index < moveList.Count; index++)
        {
            var move = moveList[index];
            _simulator.MoveGuti(move);
            gutiTypeTree.Add(_simulator.gutiMap.GetBoardStateAsList());
            _simulator.ReverseMove(gutiType, move);
        }
        return gutiTypeTree;
    }
    
    public IEnumerable<float> GetCurrentBoardStateAsList() => _simulator.gutiMap.GetBoardStateAsList();

    // first column = source Address, second column = target Address
    public List<List<int>> GetMoveIndexes(GutiType gutiType)
    {
        var moves = _simulator.ExtractMoves(gutiType);
        var moveIndices = new List<List<int>>(2){new List<int>(), new List<int>()};
        foreach (var move in moves)
        {
            moveIndices[0].Add(AddressIndexTranslator.GetIndexFromAddress(move.sourceAddress));
            moveIndices[1].Add(AddressIndexTranslator.GetIndexFromAddress(move.targetAddress));
        }
        return moveIndices;
    }
    
    public Move GetMoveFromIndexes(int sourceIndex, int targetIndex) => new Move(
        AddressIndexTranslator.GetAddressFromIndex(sourceIndex),
        AddressIndexTranslator.GetAddressFromIndex(targetIndex));
}
