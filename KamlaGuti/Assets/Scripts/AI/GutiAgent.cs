using Board.Guti;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public abstract class GutiAgent : Agent
{
    public GutiType gutiType;
    public GameManager gameManager;
    public string name = "BaseAgent";
    
    public override void Initialize()
    {
        // if (!Academy.Instance.IsCommunicatorOn)
        //     this.MaxStep = 0;
        this.MaxStep = 0; // This is to prevent the agent being reset by MlAgents Academy 
    }

    public abstract void MakeMove();
    
    public abstract override void CollectObservations(VectorSensor sensor);

    public abstract override void OnActionReceived(float[] vectorAction);

    protected abstract Move AgentMove(Move move);
    
}