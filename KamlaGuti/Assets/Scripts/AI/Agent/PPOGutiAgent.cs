using System.Collections.Generic;
using Board.Guti;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class PPOGutiAgent : GutiAgent
{
    public override void Initialize()
    {
        MaxStep = 0;
        agentObservation = new AgentObservation(gameManager.simulator);
    }

    public override void OnEpisodeBegin()
    {
        if(gameManager.gameManagerParams.autoPlay)
            gameManager.Restart();
    }

    public override void MakeMove()
    {
        gameManager.simulator.CopyBoardMap();
        RequestDecision();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(agentObservation.GetCurrentBoardStateAsList(gutiType));
        sensor.AddObservation((float) GutiType.GreenGuti);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var source = (int) vectorAction[0];
        var target = (int) vectorAction[1];
        var move = agentObservation.GetMoveFromIndexes(source, target, gutiType);
        if (RuleBook.IsMoveValid(move, gutiType, gameManager.simulator.gutiMap))
        {
            AgentMove(move);
            var reward = gameManager.scoreboard.GetScoreDifference(gutiType);
            SetReward(reward);
            gameManager.EndStep(gutiType, move);
            gameManager.UnlockStep();
        }
        else
        {
            SetReward(-16);
            RequestDecision();
        }
    }

    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        var mask = CreateMask();
        actionMasker.SetMask(0, mask[0]);
        actionMasker.SetMask(1, mask[1]);
    }
    
    private List<List<int>> CreateMask()
    {
        var moveIndexes = agentObservation.GetMoveIndexes(gutiType);
        var sourceMask = new List<int>();
        var targetMask = new List<int>();
    
        for (var i = 0; i < 37; i++)
        {
            if(!moveIndexes[0].Contains(i))sourceMask.Add(i);
            if(!moveIndexes[1].Contains(i))targetMask.Add(i);
        }
        var mask = new List<List<int>>() {sourceMask, targetMask};
        return mask;
    }

    protected override Move AgentMove(Move move)
    {
        gameManager.GetBoard().MoveGuti(move);
        gameManager.GetPlayer(gutiType).UpdateScore(move);
        return move;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Random.Range(0, 37);
        actionsOut[1] = Random.Range(0, 37);
    }
}