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
        if (!Academy.Instance.IsCommunicatorOn)
        {
            this.MaxStep = 0;
        }
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
               var noObservation = new List<float>(new float[38]);
               Debug.Log("Collect Observation Called by Unity Housekeeping.. appending empty observation");
               sensor.AddObservation(noObservation);
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
        // ML-Agent calls this function after end and beginning of episode
        // this check is to prevent actions from being taken then
        if (_gutiTypeTree == null && iterator < 0)
        {
            return;
        }
        if (_gutiTypeTree != null && iterator < _gutiTypeTree.Count)
        {
            var val = vectorAction[0];
            if (val > maxValue)
            {
                maxIndex = iterator <= 0? 0: iterator - 1;
                maxValue = val;
            }
            SetReward(0);
            RequestDecision();
        }
        else
        {
            _gutiTypeTree = null;
            if (iterator == -1)
            {
                // If no possible moves, (indicating end of game) iterator will be unset    
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


    protected override void OnDisable()
    {
        // base.OnDisable();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Random.value;
    }
}
