using Board.Guti;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public abstract class GutiAgent : Agent
{
    public GutiType gutiType;

    protected abstract void Init();

    public override void OnEpisodeBegin() => Init();

    public abstract void MakeMove();
    
    public abstract override void CollectObservations(VectorSensor sensor);

    public abstract override void OnActionReceived(float[] vectorAction);

    protected abstract Move AgentMove(int moveIndex);
    
}