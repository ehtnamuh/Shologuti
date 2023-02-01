using Board.Guti;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private Text redScore;
    [SerializeField] private Text greenScore;
    [SerializeField] private GameManager gameManager;

    public void UpdateScoreboard(GutiType gutiType, string details)
    {
        if (gutiType == GutiType.GreenGuti)
            greenScore.text = gameManager.GetPlayer(GutiType.GreenGuti).name + "\n" + details;
        else
            redScore.text = gameManager.GetPlayer(GutiType.RedGuti).name + "\n"+ details;
    }
    
    public void UpdateScoreboard(BasePlayer basePlayer) => UpdateScoreboard(basePlayer.GetGutiType(), basePlayer.ToString());
    
    public float GetScoreDifference(GutiType gutiType)
    {
        if (gutiType == GutiType.GreenGuti)
            return gameManager.GetPlayer(GutiType.GreenGuti).GetScore() - gameManager.GetPlayer(GutiType.RedGuti).GetScore();
        return gameManager.GetPlayer(GutiType.RedGuti).GetScore() - gameManager.GetPlayer(GutiType.GreenGuti).GetScore();
    }
    
}
