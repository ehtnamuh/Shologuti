using Player;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private Text redScore;
    [SerializeField] private Text greenScore;
    [SerializeField] private GameManager gameManager;
    public int ScoreUnit = 1;
    
    public void UpdateScoreboard(GutiType gutiType, string details)
    {
        if (gutiType == GutiType.GreenGuti)
            greenScore.text = "Player 2\n" + details;
        else
            redScore.text = "Player 1\n"+ details;
    }
    
    public void UpdateScoreboard(BasePlayer basePlayer) => UpdateScoreboard(basePlayer.GetGutiType(), basePlayer.ToString());
    
    public float GetScoreDifference(GutiType gutiType)
    {
        if (gutiType == GutiType.GreenGuti)
            return gameManager.GetPlayer(GutiType.GreenGuti).GetScore() - gameManager.GetPlayer(GutiType.RedGuti).GetScore();
        return gameManager.GetPlayer(GutiType.RedGuti).GetScore() - gameManager.GetPlayer(GutiType.GreenGuti).GetScore();
    }
    
}
