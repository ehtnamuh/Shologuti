﻿using System;
using System.Collections.Generic;
using Board.Guti;
using Board.View;
using Player;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Board.Board))]
[RequireComponent(typeof(Simulator))]
[RequireComponent(typeof(GameStateManager))]
[RequireComponent(typeof(Scoreboard))]
public class GameManager : MonoBehaviour
{
    // public static GameManager instance;

    // [SerializeField] public GameManagerParams settingsManager.gameManagerParams;
    [SerializeField] private Board.Board board;
    [SerializeField] public Simulator simulator;
    [SerializeField] public GameStateManager gameStateManager;
    [SerializeField] public Scoreboard scoreboard;
    [SerializeField] public PlayerSpawner playerSpawner;
    [SerializeField] public SettingsManager settingsManager;
    public UIManager uiManager;
    
    
    private int _currentStepCount = 0;
    private Dictionary<GutiType, BasePlayer> _playerMap = null;

    private bool _stepEnded;

    #region StartRestart

    // public void Awake() => MakeSingleton();

    // private void MakeSingleton()
    // {
    //     if (instance == null)
    //     {
    //         instance = this;
    //         DontDestroyOnLoad(instance.gameObject);
    //     }
    //     else
    //     {
    //         Destroy(instance.gameObject);
    //     }
    // }

    private void Start()
    {
        LockStep();
        settingsManager.gameManagerParams.maxStepCount = settingsManager.gameManagerParams.maxStepCount == 0 ? 100 : settingsManager.gameManagerParams.maxStepCount;
        Init();
        gameStateManager.SetGameState(GameState.InPlay); // Using SetGameState() can cause failure if UIManager fails to load in time
        UnlockStep();
    }

    public void Restart()
    {
        LockStep();
        board.Restart();
        Init();
        gameStateManager.SetGameState(GameState.InPlay);
        UnlockStep();
    }

    public void HardRestart()
    {
        _playerMap = null;
        GC.Collect();
        Restart();
    }

    #endregion

    #region Initialization

    private void Init()
    {
        _currentStepCount = 0;
        uiManager.Init();
        InitPlayers();
        InitScoreboard();
        gameStateManager.CurrentGutiType = Random.value > 0.5? GutiType.GreenGuti : GutiType.RedGuti;
        gameStateManager.SetPlayerTurn(gameStateManager.CurrentGutiType, _playerMap[gameStateManager.CurrentGutiType].PlayerType);
        Time.timeScale = settingsManager.gameManagerParams.timeScale;
    }
    
    private void InitPlayers()
    {
        if (_playerMap == null)
        {
            _playerMap = new Dictionary<GutiType, BasePlayer>();
            _playerMap[GutiType.RedGuti] = playerSpawner.SpawnPlayer(GutiType.RedGuti);
            _playerMap[GutiType.GreenGuti] = playerSpawner.SpawnPlayer(GutiType.GreenGuti);
        }
        else
        {
            _playerMap[GutiType.GreenGuti].ReInit();
            _playerMap[GutiType.RedGuti].ReInit();
        }
      
    }
    
    private void InitScoreboard()
    {
        scoreboard.UpdateScoreboard(GutiType.RedGuti,  _playerMap[GutiType.RedGuti].ToString());
        scoreboard.UpdateScoreboard(GutiType.GreenGuti,  _playerMap[GutiType.GreenGuti].ToString());
    }

    #endregion

    #region MainGameLoop

    private void FixedUpdate()
    {
        if (gameStateManager.GameState != GameState.InPlay)
            return;
        if (_stepEnded) NextStep();
    }
    
    public void NextStep()
    {
        if(!_stepEnded) return;
        LockStep();
        // Checking Maximum Step per Episode
        if(_currentStepCount > settingsManager.gameManagerParams.maxStepCount){ DeclareWinner(); return;}
        // Taking appropriate Actions According to Player type
        var player = _playerMap[gameStateManager.CurrentGutiType];
        var move = player.GetMove();
        if(move == null) return;
        board.MoveGuti(move);
        EndStep(gameStateManager.CurrentGutiType, move);
        if ((gameStateManager.GameState == GameState.Paused || gameStateManager.GameState == GameState.InPlay) && settingsManager.gameManagerParams.stepping)
            gameStateManager.SetGameState(GameState.Paused);
        UnlockStep();
    }

    public void EndStep(GutiType gutiType, Move move)
    {
        _currentStepCount++;
        var player = _playerMap[gutiType];
        var canContinueTurn = RuleBook.CanContinueTurn(move, board.GetGutiMapRef());
        scoreboard.UpdateScoreboard(player);
        if(!canContinueTurn) ChangeTurn();     
        if(player.CapturedGutiCount*settingsManager.gameManagerParams.scoreUnit >= settingsManager.gameManagerParams.ScoreToWin) DeclareWinner();
    }

    public void DeclareWinner()
    {
        GutiType winningGutiType;
        if (Math.Abs(_playerMap[GutiType.GreenGuti].GetScore() - _playerMap[GutiType.RedGuti].GetScore()) < 0.01)
            winningGutiType = GutiType.NoGuti;
        else
            winningGutiType = _playerMap[GutiType.GreenGuti].GetScore() > _playerMap[GutiType.RedGuti].GetScore()
                ? GutiType.GreenGuti
                : GutiType.RedGuti;
        gameStateManager.SetGameEndState(winningGutiType);
        
        // End Episodes for RLA agents *important for training
        if (_playerMap[GutiType.GreenGuti].PlayerType != PlayerType.RLAgent &&
            _playerMap[GutiType.RedGuti].PlayerType != PlayerType.RLAgent) return;
        if (_playerMap[GutiType.GreenGuti].PlayerType == PlayerType.RLAgent)
        {
            var temp = _playerMap[GutiType.GreenGuti] as PlayerRla;
            temp?.agent.EndEpisode();
        }
        if (_playerMap[GutiType.RedGuti].PlayerType == PlayerType.RLAgent)
        {
            var temp = _playerMap[GutiType.RedGuti] as PlayerRla;
            temp?.agent.EndEpisode();
        }

        if(settingsManager.gameManagerParams.autoPlay)
            Restart();
    }

    private void LockStep() => _stepEnded = false;

    public void UnlockStep() => _stepEnded = true;
    
    #endregion
    
    #region Human Input

    public void ProcessHumanInput(GameObject go)
    {
        var guti = go.GetComponent<Guti>();
        if (_playerMap[gameStateManager.CurrentGutiType].PlayerType == PlayerType.MinMaxAI)
        {
            var player = _playerMap[gameStateManager.CurrentGutiType] as PlayerMinMax;
            if (player == null) return;
            var ai = player.GetMinMaxAi();
            var projectedScore = 0;
            var move = ai.MinMax(guti.gutiType, 1, ref projectedScore);
            board.boardGui.HighlightMove(move);
            return;
        }
        if (guti.gutiType == GutiType.Highlight)
        {
            if (!(_playerMap[gameStateManager.CurrentGutiType] is PlayerHuman player)) return;
            if(player.SelectedMove == null) return;
            if(player.SelectedMove.sourceAddress == guti.address) return;
            player.SelectedMove.targetAddress = guti.address;
            UnlockStep();
        }
        else if (gameStateManager.CurrentGutiType == guti.gutiType)
        {
            var selectedAddress = guti.address;
            if (_playerMap[gameStateManager.CurrentGutiType] is PlayerHuman player) 
                player.SelectedMove = new Move {sourceAddress = selectedAddress};
            board.HighlightWalkableNodes(selectedAddress);
        }
        else
            Debug.Log("Not your turn");
    }

    public void ClearHighlights() => board.boardGui.ClearHighlightedNodes();

    #endregion
    
    #region Utilities

    public Board.Board GetBoard() => board;
    
    public BasePlayer GetPlayer(GutiType gutiType) => _playerMap[gutiType];

    private void ChangeTurn()
    {
        gameStateManager.CurrentGutiType = GutiNode.ChangeGutiType(gameStateManager.CurrentGutiType);
        gameStateManager.SetPlayerTurn(gameStateManager.CurrentGutiType, _playerMap[gameStateManager.CurrentGutiType].PlayerType);
    }

    #endregion
    
    
}
