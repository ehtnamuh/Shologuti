using UnityEngine;

public class RuleBook : MonoBehaviour 
{
    public Board board;
    public int maxStepCount;
    public int winningScore;
    private void Start() => board = GetComponent<Board>();

    public  bool CanContinueTurn(Move move) => (board.HasCapturedGuti(move) &&  board.CanCaptureGuti(move.targetAddress));
}
