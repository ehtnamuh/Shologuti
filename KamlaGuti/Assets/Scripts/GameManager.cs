using System;
using System.Collections.Generic;
using Board.Guti;
using Board.View;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Board.Board))]
[RequireComponent(typeof(Simulator))]
[RequireComponent(typeof(GameStateManager))]
[RequireComponent(typeof(Scoreboard))]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] private float timeScale = 5.0f;
    [SerializeField] private Board.Board board;
    [SerializeField] public Simulator simulator;
    [SerializeField] public GameStateManager gameStateManager;
    [SerializeField] public Scoreboard scoreboard;
    public UIManager uiManager;
    public GutiAgent agent;
    
    public int maxStepCount;
    public bool autoPlay;

    private int _currentStepCount = 0;
    private GutiType _currentTurnGutiType;
    private Dictionary<GutiType, BasePlayer> _playerMap = null;

    private bool _stepEnded;

    #region StartRestart

    public void Awake() => MakeSingleton();

    private void MakeSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            Destroy(instance.gameObject);
        }
    }

    private void Start()
    {
        LockStep();
        maxStepCount = maxStepCount == 0 ? 200 : maxStepCount;
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


    #endregion

    #region Initialization

    private void Init()
    {
        _currentTurnGutiType = Random.value > 0.5? GutiType.GreenGuti : GutiType.RedGuti;
        _currentStepCount = 0;
        uiManager.Init();
        InitPlayers();
        InitScoreboard();
        Time.timeScale = timeScale;
    }
    
    private void InitPlayers()
    {
        if (_playerMap == null)
        {
            _playerMap = new Dictionary<GutiType, BasePlayer>();
            // _playerMap[GutiType.GreenGuti] = new PlayerHuman(GutiType.GreenGuti, PlayerType.Human);
            // _playerMap[GutiType.RedGuti] = new PlayerHuman(GutiType.RedGuti, PlayerType.Human);
            _playerMap[GutiType.RedGuti] = new PlayerMinMax(GutiType.RedGuti, PlayerType.AI, new MinMaxAi(simulator), 1);
            // _playerMap[GutiType.GreenGuti] = new PlayerMinMax(GutiType.GreenGuti, PlayerType.AI, new MinMaxAi(simulator), 3);
            _playerMap[GutiType.GreenGuti] = new PlayerRla(GutiType.GreenGuti, PlayerType.RLA, agent);
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

    private void Update()
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
        if(_currentStepCount > maxStepCount){ DeclareWinner(); return;}
        _currentStepCount++;
        // Taking appropriate Actions According to Player type
        var player = _playerMap[_currentTurnGutiType];
        var move = player.GetMove();
        if(move == null) return;
        board.MoveGuti(move);
        EndStep(_currentTurnGutiType, move);
        UnlockStep();
    }

    public void EndStep(GutiType gutiType, Move move)
    {
        var player = _playerMap[gutiType];
        var canContinueTurn = RuleBook.CanContinueTurn(move);
        scoreboard.UpdateScoreboard(player);
        if(!canContinueTurn) ChangeTurn();     
        if(player.CapturedGutiCount >= 16) DeclareWinner();
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
        if (_playerMap[GutiType.GreenGuti].PlayerType == PlayerType.RLA ||
            _playerMap[GutiType.RedGuti].PlayerType == PlayerType.RLA)
            agent.EndEpisode();
        else
            return;
        if (autoPlay) Restart();
    }

    private void LockStep() => _stepEnded = false;

    public void UnlockStep() => _stepEnded = true;
    
    #endregion
    
    #region Human Input

    public void ProcessHumanInput(GameObject go)
    {
        var guti = go.GetComponent<Guti>();
        if (_playerMap[_currentTurnGutiType].PlayerType == PlayerType.AI)
        {
            var player = _playerMap[_currentTurnGutiType] as PlayerMinMax;
            if (player == null) return;
            var ai = player.GetMinMaxAi();
            var projectedScore = 0;
            var move = ai.MinMax(guti.gutiType, 1, ref projectedScore);
            board.boardGui.HighlightMove(move);
            return;
        }
        if (guti.gutiType == GutiType.Highlight)
        {
            if (!(_playerMap[_currentTurnGutiType] is PlayerHuman player)) return;
            if(player.SelectedMove == null) return;
            if(player.SelectedMove.sourceAddress == guti.address) return;
            player.SelectedMove.targetAddress = guti.address;
            UnlockStep();
        }
        else if (_currentTurnGutiType == guti.gutiType)
        {
            var selectedAddress = guti.address;
            if (_playerMap[_currentTurnGutiType] is PlayerHuman player) 
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

    private void ChangeTurn() => _currentTurnGutiType = GutiNode.ChangeGutiType(_currentTurnGutiType);
    #endregion
    
    
}
