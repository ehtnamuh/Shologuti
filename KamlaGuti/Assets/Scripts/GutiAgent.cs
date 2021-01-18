using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class GutiAgent : Agent
{
    public GameObject _gameManagetGO;
    public GameManager _gameManager;
    private List<List<float>> _gutiTypeTree;
    public GutiType gutiType;
    
    private int iterator;
    private int maxIndex;
    private float maxValue;
    
    public override void Initialize()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _gameManager = _gameManagetGO.GetComponent<GameManager>();
        iterator = -1;
        maxIndex = -1;
        maxValue = -1;
        _gutiTypeTree = null;
    }
    
    public override void OnEpisodeBegin()
    {
        Init();
    }

    public void PopulateGutiTypeTree(List<List<float>> gutiTypeTree)
    {
        _gutiTypeTree = gutiTypeTree;
        if (_gutiTypeTree.Count > 0) iterator = 0;
        else
        {
            Debug.Log("Should Set the game to restart here");
            _gameManager.DeclareWinner();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (iterator < 0)
        {
               var noObservation = new List<float>(new float[37]);
               Debug.Log("This should not happen");
               sensor.AddObservation(noObservation);
               return;
        }
        var gutiList = _gutiTypeTree[iterator++];
        sensor.AddObservation(gutiList);
        // Debug.Log(sensor.GetName() + " || " + sensor.GetObservationShape());
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // ML-Agent calls this function after end and begining of episode
        // this check is to prevent actions from being taken then
        if (_gutiTypeTree == null && iterator < 0)
        {
            return;
        }
        if (iterator < _gutiTypeTree.Count)
        {
            // Debug.Log("we must request more actions");
            var val = vectorAction[0];
            if (val > maxValue)
            {
                maxIndex = iterator <= 0? 0: iterator - 1;
                maxValue = val;
            }
            SetReward(-666);
            RequestDecision();
        }
        else
        {
            Debug.Log("We should have the best state now");
            _gutiTypeTree = null;
            // If no possible moves, (indicating end of game) iterator will be unset
            if (iterator == -1)
            {
                _gameManager.DeclareWinner();
                return;
            }
            // If only one move was available, maxIndex will be unset
            if (maxIndex == -1) maxIndex = 0;
            _gameManager.AgentMove(gutiType, maxIndex);
            iterator = -1;
            maxIndex = -1;
            maxValue = -1;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        // base.OnDisable();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Random.value;
        // Debug.Log($"heuristic: {actionsOut[0]}");
    }
}
