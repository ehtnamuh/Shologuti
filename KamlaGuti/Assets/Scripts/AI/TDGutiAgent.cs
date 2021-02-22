using System.Collections.Generic;
using Board.Guti;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TDGutiAgent : GutiAgent
{
    private enum StepStage
    {
        StepBegin = 0,
        StepRunning = -1,
        StepEnd = -2
    }
    
    private List<List<float>> _gutiTypeTree;
    private int _iterator;
    private int _maxIndex;
    private float _maxObsValue;
    private int _actionIndex;
    private List<Move> _moveList;
    
    public override void Initialize()
    {
        // if (!Academy.Instance.IsCommunicatorOn)
        //     this.MaxStep = 0;
        MaxStep = 0; // This is to prevent the agent being reset by MlAgents Academy 
        Init();
    }

    private void Awake() => Init();

    private void Init()
    {
        _actionIndex = gutiType == GutiType.GreenGuti ? 0 : 1;
        _iterator = -1;
        _maxIndex = -1;
        _maxObsValue = -1;
        _moveList = null;
        _gutiTypeTree = null;
    }
    
    public override void OnEpisodeBegin()
    {
        Init();
        if(gameManager.gameManagerParams.autoPlay)
            gameManager.Restart();
    }

    public override void MakeMove()
    {
        var simulator = gameManager.simulator;
        simulator.LoadMap();
        _moveList = simulator.ExtractMoves(gutiType);
        var gutiTypeTree = simulator.GetAllFutureBoardStatesAsList(gutiType, _moveList);
        PopulateGutiTypeTree(gutiTypeTree);
        // simulator.UnloadMap();
        RequestDecision();
    }

    private void PopulateGutiTypeTree(List<List<float>> gutiTypeTree)
    {
        _gutiTypeTree = gutiTypeTree;
        if (_gutiTypeTree.Count > 0) _iterator = 0;
        else
            gameManager.DeclareWinner();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        if (_iterator < 0)
        {
            // This is triggered when Academy calls a RequestDecision Cycle
            sensor.AddObservation(new List<float>(new float[39]));
            return;
        }
        // Adding Observations
        var gutiList = _gutiTypeTree[_iterator++];
        sensor.AddObservation(gutiList);
        sensor.AddObservation(gameManager.scoreboard.GetScoreDifference(gutiType));
        
        // Last bit used to Send signals to the Python Environment to indicate stage of the step observation
        if (_iterator <= 1)
            sensor.AddObservation((float) StepStage.StepBegin);
        else if (_iterator < _gutiTypeTree.Count)
            sensor.AddObservation((float) StepStage.StepRunning);
        else
            sensor.AddObservation((float) StepStage.StepEnd);
    }
    
    
    public override void OnActionReceived(float[] vectorAction)
    {
        if (_gutiTypeTree == null || _moveList.Count <= 0)
            return;
        if (_iterator < _gutiTypeTree.Count)
        {
            UpdateMaxState(vectorAction[_actionIndex]);
            SetReward(0);
            RequestDecision();
        }
        else if (_iterator >= _gutiTypeTree.Count)
        {
            UpdateMaxState(vectorAction[_actionIndex]);
            var move = AgentMove(_moveList[_maxIndex]);
            var reward =  gameManager.scoreboard.GetScoreDifference(gutiType);
            Debug.Log($"{_iterator}, {_gutiTypeTree.Count}");
            SetReward(reward);
            gameManager.EndStep(gutiType, move);
            Init();
            gameManager.UnlockStep();
        }
    }

    protected override Move AgentMove(Move move)
    {
        gameManager.GetBoard().MoveGuti(move);
        gameManager.GetPlayer(gutiType).UpdateScore(move);
        return move;
    }

    private void UpdateMaxState(float obsVal)
    {
        var tempIterator = _iterator;
        if (!(obsVal > _maxObsValue)) return;
        if (_iterator > _moveList.Count) tempIterator = _moveList.Count;
        _maxIndex =  tempIterator <= 0 ? 0 : tempIterator - 1;
        _maxObsValue = obsVal;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Random.Range(0.0f, 1.0f);
    }
}