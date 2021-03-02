using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public IEnumerable<float> GetCurrentBoardStateAsList(GutiType gutiType)
    {
        var boardStateAsList = _simulator.gutiMap.GetBoardStateAsList();
        if (gutiType == GutiType.RedGuti) boardStateAsList = AgentFilter.ObservationFilter(boardStateAsList, gutiType);
        return boardStateAsList;
    }

    // first column = source Address, second column = target Address
    public List<List<int>> GetMoveIndexes(GutiType gutiType)
    {
        var moves = _simulator.ExtractMoves(gutiType);
        var moveIndices = new List<List<int>>(2){new List<int>(), new List<int>()};
        foreach (var move in moves)
        {
            moveIndices[0].Add(AgentFilter.IndexFilter( AddressIndexTranslator.GetIndexFromAddress(move.sourceAddress), gutiType));
            moveIndices[1].Add(AgentFilter.IndexFilter( AddressIndexTranslator.GetIndexFromAddress(move.targetAddress), gutiType));
        }
        return moveIndices;
    }
    
    public Move GetMoveFromIndexes(int sourceIndex, int targetIndex, GutiType gutiType)
    {
        sourceIndex = AgentFilter.IndexFilter(sourceIndex, gutiType);
        targetIndex = AgentFilter.IndexFilter(targetIndex, gutiType);
        return new Move(
            AddressIndexTranslator.GetAddressFromIndex(sourceIndex),
            AddressIndexTranslator.GetAddressFromIndex(targetIndex));
    }

}

public static class AgentFilter
{
    public static int IndexFilter(int index,  GutiType gutiType) => gutiType == GutiType.GreenGuti ? index : 36 - index;

    public static List<float> ObservationFilter(List<float> observationList, GutiType gutiType)
    {
        if (gutiType == GutiType.GreenGuti) return observationList;
        const float greenVal = (float) GutiType.GreenGuti;
        const float redVal = (float) GutiType.RedGuti;
        observationList.Reverse();
        for (var index = 0; index < observationList.Count() ; index++)
        {
            var f = observationList[index];
            if (Math.Abs(f - greenVal) <= 0.001)
            {
                observationList[index] = redVal;
            }
            else if (Math.Abs(f - redVal) <= 0.001)
            {
                observationList[index] = greenVal;
            }
        } 
        return observationList;
    }
}