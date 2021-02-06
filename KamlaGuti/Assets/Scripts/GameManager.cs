using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Board))]
[RequireComponent(typeof(Simulator))]
[RequireComponent(typeof(GameStateManager))]
[RequireComponent(typeof(Scoreboard))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private float timeScale = 5.0f;
    [SerializeField] private Board board;
    [SerializeField] public Simulator simulator;
    [SerializeField] public GameStateManager gameStateManager;
    [SerializeField] public Scoreboard scoreboard;
    public UIManager uiManager;
    public GutiAgent agent;
    
    public int maxStepCount;
    public bool autoPlay;

    private int _currentStepCount = 0;
    private GutiType _currentTurnGutiType;
    private Dictionary<GutiType, Player> _playerMap = null;

    private bool _stepEnded;

    #region StartRestart

    private void Start()
    {
        LockStep();
        uiManager = gameObject.GetComponent<UIManager>();
        maxStepCount = maxStepCount == 0 ? 200 : maxStepCount;
        Init();
        gameStateManager.GameState = GameState.InPlay; // Using SetGameState() can cause failure if UIManager fails to load in time
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
            _playerMap = new Dictionary<GutiType, Player>();
            // _playerMap[GutiType.GreenGuti] = new Player(GutiType.GreenGuti, PlayerType.Human, this);
            // _playerMap[GutiType.RedGuti] = new Player(GutiType.RedGuti, PlayerType.Human, this);
            _playerMap[GutiType.RedGuti] = new Player(GutiType.RedGuti, PlayerType.AI, this, 1);
            // _playerMap[GutiType.GreenGuti] = new Player(GutiType.GreenGuti, PlayerType.AI, this, 3);
            _playerMap[GutiType.GreenGuti] = new Player(GutiType.GreenGuti, PlayerType.RLA, this);
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
        Move move;
        switch (player.PlayerType)
        {
            case PlayerType.RLA:
                LockStep();
                player.MakeMove();
                return;
            case PlayerType.Human when player.SelectedMove == null:
                return;
            case PlayerType.AI:
                move = player.MakeMove();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
        if (_playerMap[GutiType.GreenGuti].PlayerType != PlayerType.RLA &&
            _playerMap[GutiType.RedGuti].PlayerType != PlayerType.RLA) return;
        if (autoPlay) agent.EndEpisode();
    }

    private void LockStep() => _stepEnded = false;

    public void UnlockStep() => _stepEnded = true;
    
    #endregion
    
    #region Human Input

    public void ProcessHumanInput(GameObject go)
    {
        var guti = go.GetComponent<Guti>();
        var player = _playerMap[_currentTurnGutiType];
        if (_playerMap[_currentTurnGutiType].PlayerType != PlayerType.Human)
        {
            // TODO: Make Button to HighLight Move that AI MinMax wants to take
            var ai = player.GetMinMaxAi();
            var projectedScore = 0;
            var move = ai.MinMax(guti.gutiType, 1, ref projectedScore);
            board.HighlightMove(move);
            return;
        }
        if (guti.gutiType == GutiType.Highlight)
        {
            if(player.SelectedMove == null) return;
            player.SelectedMove.targetAddress = guti.address;
            UnlockStep();
        }
        else if (_currentTurnGutiType == guti.gutiType)
        {
            var selectedAddress = guti.address;
            player.SelectedMove = new Move();
            player.SelectedMove.sourceAddress = selectedAddress;
            SpawnHighlights(selectedAddress);
        }
        else
            Debug.Log("Not your turn");
    }

    private void SpawnHighlights(Address selectedAddress)
    {
        board.SpawnHighlightNode(selectedAddress, Color.white);
        board.HighlightWalkableNodes(selectedAddress);
    }
    
    public void ClearHighlights() => board.ClearHighlightedNodes();

    #endregion
    
    #region Utilities

    public Board GetBoard() => board;
    
    public Player GetPlayer(GutiType gutiType) => _playerMap[gutiType];

    private void ChangeTurn() => _currentTurnGutiType = GutiNode.ChangeGutiType(_currentTurnGutiType);
    #endregion
    
    
}
