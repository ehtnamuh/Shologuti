using Board.Guti;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class PpoGutiAgent : GutiAgent
{
    protected override void Init()
    {
        // throw new System.NotImplementedException();
    }

    public override void MakeMove() => RequestDecision();

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(GameManager.instance.simulator.GetCurrentBoardStateAsList());
        sensor.AddObservation((float)gutiType);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var source = (int)vectorAction[0];
        var target =  (int)vectorAction[1];
        var move = GameManager.instance.simulator.GetMoveFromIndexes(source, target);
        if (RuleBook.IsMoveValid(move, gutiType))
        {
            AgentMove(move);
            var reward =  GameManager.instance.scoreboard.GetScoreDifference(gutiType) / 16.0f;
            SetReward(reward);
            GameManager.instance.EndStep(gutiType, move);
            Init();
            GameManager.instance.UnlockStep();
        }
        else
        {
            SetReward(-1000);
            RequestDecision();
        }
    }

    protected override  Move AgentMove(Move move)
    {
        GameManager.instance.GetBoard().MoveGuti(move);
        GameManager.instance.GetPlayer(gutiType).UpdateScore(move);
        return move;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Random.Range(0, 37);
        actionsOut[1] = Random.Range(0, 37);
    }
}