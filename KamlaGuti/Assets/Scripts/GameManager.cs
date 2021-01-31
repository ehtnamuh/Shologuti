using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum GameState
{
    InPlay = 0,
    RedWin = 1,
    GreenWin = 2,
    Paused = 3
}

public class GameManager : MonoBehaviour
{
    public GameObject boardGameObject;
    public Board board;
    public UIManager uiManager;
    public GutiAgent agent;
    
    public int scoreUnit;
    public int maxStepCount;
    public bool isTraining;

    private int _currentStepCount = 0;
    private GameState _gameState = GameState.InPlay;
    private GutiType _currentTurnGutiType;
    private Dictionary<GutiType, Player> _playerMap = null;

    private bool _stepEnded;

    #region StartUpdateRestart

    private void Start()
    {
        _stepEnded = false;
        board = boardGameObject.GetComponent<Board>();
        uiManager = gameObject.GetComponent<UIManager>();
        maxStepCount = maxStepCount == 0 ? 500 : maxStepCount;
        Init();
        agent.gutiType = GutiType.GreenGuti;
        _gameState = GameState.InPlay; // Using SetGameState() can cause failure if UIManager fails to load in time
        _stepEnded = true;
    }

    public void Restart()
    {
        _stepEnded = false;
        board.Restart();
        Init();
        agent.EndEpisode();
        SetGameState(GameState.InPlay);
        _stepEnded = true;
    }
    
    private void Update()
    {
        if (_gameState != GameState.InPlay)
            return;
        if (_stepEnded) NextStep();
    }


    #endregion

    #region Initialization

    private void Init()
    {
        _currentTurnGutiType = Random.value > 0.5? GutiType.GreenGuti : GutiType.RedGuti;
        _currentStepCount = 0;
        InitPlayers();
        InitScoreboard();
        _playerMap[GutiType.GreenGuti].agent = agent;
        Time.timeScale = 2.0f;
    }
    
    private void InitPlayers()
    {
        if (_playerMap == null)
        {
            _playerMap = new Dictionary<GutiType, Player>();
            // _playerMap[GutiType.GreenGuti] = new Player(GutiType.GreenGuti, PlayerType.Human, this);
            // _playerMap[GutiType.RedGuti] = new Player(GutiType.RedGuti, PlayerType.Human, this);
            _playerMap[GutiType.RedGuti] = new Player(GutiType.RedGuti, PlayerType.Ai, this, 1);
            // _playerMap[GutiType.GreenGuti] = new Player(GutiType.GreenGuti, PlayerType.Ai, this, 3);
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
        uiManager.UpdateScore(GutiType.RedGuti,  _playerMap[GutiType.RedGuti].ToString());
        uiManager.UpdateScore(GutiType.GreenGuti,  _playerMap[GutiType.GreenGuti].ToString());
    }

    #endregion

    #region Game Step

    public void NextStep()
    {
        if(!_stepEnded) return;
        _stepEnded = false;
        // Checking Maximum Step per Episode
        // Debug.Log("Step");
        if(_currentStepCount > maxStepCount){ DeclareWinner(); return;}
        _currentStepCount++;
        // Taking appropriate Actions According to Player type
        var player = _playerMap[_currentTurnGutiType];
        switch (player.playerType)
        {
            case PlayerType.RLA:
                _stepEnded = false;
                player.MakeMove();
                return;
            case PlayerType.Human when player.SelectedMove == null:
                return;
        }
        var continueTurn = player.MakeMove();
        
        if(player.CapturedGutiCount >= 16) DeclareWinner();
        // Update Score Board Immediately
        uiManager.UpdateScore(_currentTurnGutiType, player.ToString());
        // Check if Player should switch
        if(!continueTurn) ChangeTurn();
        _stepEnded = true;
    }
    
    public void AgentMove(GutiType gutiType, int maxIndex)
    {
        var continueTurn = _playerMap[gutiType].AgentMove(maxIndex);
        uiManager.UpdateScore(_currentTurnGutiType, _playerMap[_currentTurnGutiType].ToString());
        if(!continueTurn) ChangeTurn();
        _stepEnded = true;
    }

    public void DeclareWinner()
    {
        _stepEnded = false;
        var winningGutiType = _playerMap[GutiType.GreenGuti].GetScore() > _playerMap[GutiType.RedGuti].GetScore() ? GutiType.GreenGuti : GutiType.RedGuti;
        SetGameEndState(winningGutiType);
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

    public void ProcessInput(GameObject go)
    {
        var guti = go.GetComponent<Guti>();
        var player = _playerMap[_currentTurnGutiType];
        if (_playerMap[_currentTurnGutiType].playerType != PlayerType.Human)
        {
            var gu = go.GetComponent<Guti>();
            // TODO: Make Button to HighLight Move that AI MinMax wants to take
            // Debug.Log(board.getGutiType(gu.address));
            // board.HighlightWalkableNodes(gu.address);
            // player = _playerMap[guti.gutiType];
            var AI = player.GetMinMaxAi();
            AI._gutiMap = board.GetGutiMap();
            int tempScore = 0;
            var move = AI.MinMax(guti.gutiType, 1, ref tempScore);
            Debug.Log(move);
            ClearHighlights();
            board.SpawnHighlightNode(move.sourceAddress, Color.white);
            board.SpawnHighlightNode(move.targetAddress, Color.blue);
            return;
        }
        
        if (guti.gutiType == GutiType.Highlight)
        {
            if(player.SelectedMove == null) return;
            player.SelectedMove.targetAddress = guti.address;
            _stepEnded = true;
        }
        else if (_currentTurnGutiType == guti.gutiType)
        {
            var selectedAddress = guti.address;
            player.SelectedMove = new Move();
            player.SelectedMove.sourceAddress = selectedAddress;
            board.HighlightWalkableNodes(selectedAddress);
            board.SpawnHighlightNode(player.SelectedMove.sourceAddress, Color.white);
        }
        else
            Debug.Log("Not your turn mate!");
    }

    public void ClearHighlights() => board.ClearHighlightedNodes();
    
    public GutiType GetCurrentGutiType() => _currentTurnGutiType;

    public void ChangeTurn() => _currentTurnGutiType = _currentTurnGutiType == GutiType.GreenGuti ? GutiType.RedGuti : GutiType.GreenGuti;
}
