using System;
using Board.Guti;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class PpoGutiAgent : GutiAgent
{
    public override void Initialize() => MaxStep = 0;

    public override void MakeMove() => RequestDecision();

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameManager.simulator.GetCurrentBoardStateAsList());
        sensor.AddObservation((float)gutiType);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var source = (int)vectorAction[0];
        var target =  (int)vectorAction[1];
        var move = gameManager.simulator.GetMoveFromIndexes(source, target);
        if (RuleBook.IsMoveValid(move, gutiType))
        {
            AgentMove(move);
            var reward =  gameManager.scoreboard.GetScoreDifference(gutiType) / 16.0f;
            SetReward(reward);
            gameManager.EndStep(gutiType, move);
            gameManager.UnlockStep();
        }
        else
        {
            SetReward(-1000);
            RequestDecision();
        }
    }

    protected override  Move AgentMove(Move move)
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