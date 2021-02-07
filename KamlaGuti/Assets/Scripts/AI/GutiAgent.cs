using System;
using System.Collections.Generic;
using Board.Guti;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class GutiAgent : Agent
{
    private List<List<float>> _gutiTypeTree;
    public GutiType gutiType;

    private int _iterator;
    private int _maxIndex;
    private float _maxValue;
    private int _actionIndex;
    private int explorationDepth;
    private List<Move> _moveList;
    
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
        _actionIndex = gutiType == GutiType.GreenGuti ? 0 : 1;
        _iterator = -1;
        _maxIndex = -1;
        _maxValue = -1;
        _gutiTypeTree = null;
    }
    
    public override void OnEpisodeBegin()
    {
        Init();
        var gameState = GameManager.instance.gameStateManager.GameState;
        if (gameState == GameState.GreenWin || gameState == GameState.RedWin || gameState == GameState.Draw)
            GameManager.instance.Restart();
    }
    
    public void MakeMove()
    {
        var simulator = GameManager.instance.simulator;
        simulator.MakeReady();
        _moveList = simulator.ExtractMoves(gutiType);
        var gutiTypeTree = simulator.GetAllBoardStatesAsList(gutiType, _moveList);
        PopulateGutiTypeTree(gutiTypeTree);
        RequestDecision();
    }

    private void PopulateGutiTypeTree(List<List<float>> gutiTypeTree)
    {
        _gutiTypeTree = gutiTypeTree;
        if (_gutiTypeTree.Count > 0) _iterator = 0;
        else
            GameManager.instance.DeclareWinner();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        if (_iterator < 0)
        {
            sensor.AddObservation(new List<float>(new float[38]));
            return;
        }
        var gutiList = _gutiTypeTree[_iterator++];
        sensor.AddObservation(gutiList);
        if (_iterator < _gutiTypeTree.Count)
            sensor.AddObservation(1.0f);
        else
            sensor.AddObservation(-2.0f);
    }
    
    
    public override void OnActionReceived(float[] vectorAction)
    {
        if (_gutiTypeTree == null)
            return;
        if (_iterator < _gutiTypeTree.Count)
        {
            UpdateMaxState(vectorAction[0]);
            SetReward(0);
            RequestDecision();
        }
        else
        {
            if (_iterator < 0)
            {
                GameManager.instance.DeclareWinner(); // If no possible moves, (indicating end of game) iterator will be unset    
                return;
            }
            // If only one move was available, maxIndex will be unset
            if (_maxIndex == -1) _maxIndex = 0;
            var move = AgentMove(_maxIndex);
            var reward =  GameManager.instance.scoreboard.GetScoreDifference(gutiType) / 16.0f;
            SetReward(reward);
            GameManager.instance.EndStep(gutiType, move);
            GameManager.instance.UnlockStep();
            Init();
        }
    }


    private Move AgentMove(int moveIndex)
    {
        var move = _moveList[moveIndex];
        GameManager.instance.GetBoard().MoveGuti(move);
        GameManager.instance.GetPlayer(gutiType).UpdateScore(move);
        return move;
    }

    private void UpdateMaxState(float val)
    {
        if (!(val > _maxValue)) return;
        _maxIndex = _iterator <= 0 ? 0 : _iterator - 1;
        _maxValue = val;
    }


    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Random.value;
        actionsOut[1] = 1.0f - actionsOut[0];
    }
}