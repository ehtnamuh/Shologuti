using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class GutiAgent : Agent
{
    [SerializeField] private GameManager gameManager;
    private List<List<float>> _gutiTypeTree;
    public GutiType gutiType;
    
    private int iterator;
    private int maxIndex;
    private float maxValue;
    
    public override void Initialize()
    {
        // if (!Academy.Instance.IsCommunicatorOn)
        // {
        //     this.MaxStep = 0;
        // }
        // This is to prevent the agent being reset by MlAgents Academy 
        this.MaxStep = 0;
        Init();
    }

    private void Awake() => Init();

    private void Init()
    {
        iterator = -1;
        maxIndex = -1;
        maxValue = -1;
        _gutiTypeTree = null;
    }
    
    public override void OnEpisodeBegin()
    {
        Init();
        if (gameManager.GetGameState() == GameState.GreenWin || gameManager.GetGameState() == GameState.RedWin || gameManager.GetGameState() == GameState.Draw)
            gameManager.Restart();
    }
    

    public void PopulateGutiTypeTree(List<List<float>> gutiTypeTree)
    {
        _gutiTypeTree = gutiTypeTree;
        if (_gutiTypeTree.Count > 0) iterator = 0;
        else
            gameManager.DeclareWinner();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (iterator < 0)
        {
               // Debug.Log("Collect Observation Called by Unity Housekeeping.. appending empty observation");
               sensor.AddObservation(new List<float>(new float[38]));
               return;
        }
        var gutiList = _gutiTypeTree[iterator++];
        sensor.AddObservation(gutiList);
        if (iterator < _gutiTypeTree.Count)
            sensor.AddObservation(1.0f);
        else
            sensor.AddObservation(-2.0f);
    }
    
    
    public override void OnActionReceived(float[] vectorAction)
    {
        if (_gutiTypeTree == null)
            return;
        if (iterator < _gutiTypeTree.Count)
        {
            UpdateMaxState(vectorAction[0]);
            SetReward(0);
            RequestDecision();
        }
        else
        {
            if (iterator < 0)
            {
                gameManager.DeclareWinner(); // If no possible moves, (indicating end of game) iterator will be unset    
                return;
            }
            // If only one move was available, maxIndex will be unset
            if (maxIndex == -1) maxIndex = 0;
            gameManager.AgentMove(gutiType, maxIndex);
            var reward = gameManager.GetScoreDifference(gutiType) / 16.0f;
            SetReward(reward);
            gameManager.UnlockStep();
            Init();
        }
    }

    private void UpdateMaxState(float val)
    {
        if (!(val > maxValue)) return;
        maxIndex = iterator <= 0 ? 0 : iterator - 1;
        maxValue = val;
    }


    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Random.value;
    }
}
