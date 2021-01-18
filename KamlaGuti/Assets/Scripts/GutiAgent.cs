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
        _gutiTypeTree = new List<List<float>>();
    }
    
    public override void OnEpisodeBegin()
    {
        Init();
        // base.OnEpisodeBegin();
    }

    public void PopulateGutiTypeTree(List<List<float>> gutiTypeTree)
    {
        _gutiTypeTree = gutiTypeTree;
        if (_gutiTypeTree.Count > 0) iterator = 0;
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
        // Debug.Log($"VectorAction: {vectorAction[0]}");
        if (iterator < 0)
        {
            // _gameManager.ChangeTurn();
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
        // Debug.Log($"heuristic: {actionsOut[0]}");
    }
}
