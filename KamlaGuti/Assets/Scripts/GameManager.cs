using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum GameState
{
    InPlay = 0,
    RedWin = 1,
    GreenWin = 2,
    Paused = 3,
    Draw = 4
}

public class GameManager : MonoBehaviour
{
    public GameObject boardGameObject;
    public Board board;
    public UIManager uiManager;
    public GutiAgent agent;
    
    public int scoreUnit;
    public int maxStepCount;
    public bool autoPlay;

    private int _currentStepCount = 0;
    private GameState _gameState = GameState.InPlay;
    private GutiType _currentTurnGutiType;
    private Dictionary<GutiType, Player> _playerMap = null;

    private bool _stepEnded;

    #region StartRestart

    private void Start()
    {
        LockStep();
        board = boardGameObject.GetComponent<Board>();
        uiManager = gameObject.GetComponent<UIManager>();
        maxStepCount = maxStepCount == 0 ? 200 : maxStepCount;
        Init();
        _gameState = GameState.InPlay; // Using SetGameState() can cause failure if UIManager fails to load in time
        UnlockStep();
    }

    public void Restart()
    {
        LockStep();
        board.Restart();
        Init();
        SetGameState(GameState.InPlay);
        UnlockStep();
    }


    #endregion

    #region Initialization

    private void Init()
    {
        _currentTurnGutiType = Random.value > 0.5? GutiType.GreenGuti : GutiType.RedGuti;
        _currentStepCount = 0;
        InitPlayers();
        InitScoreboard();
        Time.timeScale = 2.0f;
    }
    
    private void InitPlayers()
    {
        if (_playerMap == null)
        {
            _playerMap = new Dictionary<GutiType, Player>();
            // _playerMap[GutiType.GreenGuti] = new Player(GutiType.GreenGuti, PlayerType.Human, this);
            // _playerMap[GutiType.RedGuti] = new Player(GutiType.RedGuti, PlayerType.Human, this);
            _playerMap[GutiType.RedGuti] = new Player(GutiType.RedGuti, PlayerType.AI, this, 1);
            _playerMap[GutiType.GreenGuti] = new Player(GutiType.GreenGuti, PlayerType.AI, this, 3);
            // _playerMap[GutiType.GreenGuti] = new Player(GutiType.GreenGuti, PlayerType.RLA, this);
        }
        else
        {
            _playerMap[GutiType.GreenGuti].ReInit();
            _playerMap[GutiType.RedGuti].ReInit();
        }
      
    }
    
    private void InitScoreboard()
    {
        uiManager.UpdateScore(GutiType.RedGuti,  _playerMap[GutiType.RedGuti].ToString());
        uiManager.UpdateScore(GutiType.GreenGuti,  _playerMap[GutiType.GreenGuti].ToString());
    }

    #endregion

    #region Game Step 

    private void Update()
    {
        if (_gameState != GameState.InPlay)
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
        switch (player.playerType)
        {
            case PlayerType.RLA:
                LockStep();
                player.MakeMove();
                return;
            case PlayerType.Human when player.SelectedMove == null:
                return;
        }
        var canContinueTurn = player.CanContinueTurn(player.MakeMove());
        uiManager.UpdateScore(player.GetGutiType(), player.ToString());
        if(!canContinueTurn) ChangeTurn();     
        if(player.CapturedGutiCount >= 16) DeclareWinner();
        UnlockStep();
    }

    private void LockStep() => _stepEnded = false;

    public void UnlockStep() => _stepEnded = true;

    public void AgentMove(GutiType gutiType, int maxIndex)
    {
        var player = _playerMap[gutiType];
        var canContinueTurn = player.CanContinueTurn(player.AgentMove(maxIndex));
        uiManager.UpdateScore(player.GetGutiType(), _playerMap[_currentTurnGutiType].ToString());
        if(!canContinueTurn) ChangeTurn();
    }

    public void DeclareWinner()
    {
        GutiType winningGutiType;
        if (_playerMap[GutiType.GreenGuti].GetScore() == _playerMap[GutiType.RedGuti].GetScore())
            winningGutiType = GutiType.NoGuti;
        else
            winningGutiType = _playerMap[GutiType.GreenGuti].GetScore() > _playerMap[GutiType.RedGuti].GetScore()
                ? GutiType.GreenGuti
                : GutiType.RedGuti;
        SetGameEndState(winningGutiType);
        if (_playerMap[GutiType.GreenGuti].playerType != PlayerType.RLA &&
            _playerMap[GutiType.RedGuti].playerType != PlayerType.RLA) return;
        if(autoPlay) agent.EndEpisode();
    }

    #endregion

    #region GameState
    public void SetGameState(GameState gameState)
    {
        _gameState = gameState;
        uiManager.UpdateGameStatus(gameState);
    }

    public GameState GetGameState() => _gameState;

    private void SetGameEndState(GutiType gutiType)
    {
        var gameState = gutiType == GutiType.GreenGuti ? GameState.GreenWin : GameState.RedWin;
        SetGameState(gameState);
    }
    
    #endregion

    #region Human Input

    public void ProcessHumanInput(GameObject go)
    {
        var guti = go.GetComponent<Guti>();
        var player = _playerMap[_currentTurnGutiType];
        if (_playerMap[_currentTurnGutiType].playerType != PlayerType.Human)
        {
            // TODO: Make Button to HighLight Move that AI MinMax wants to take
            var ai = player.GetMinMaxAi();
            var projectedScore = 0;
            ai.gutiMap = board.GetGutiMap();
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
    
    public GutiType GetCurrentGutiType() => _currentTurnGutiType;

    public void ChangeTurn() => _currentTurnGutiType = _currentTurnGutiType == GutiType.GreenGuti ? GutiType.RedGuti : GutiType.GreenGuti;
}
