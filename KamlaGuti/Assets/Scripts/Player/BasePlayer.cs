namespace Player
{
    public enum PlayerType
    {
        Human = 0,
        AI = 1,
        RLA = 2
    
    }

    public abstract class BasePlayer
    {
        protected GutiType gutiType;
        protected GameManager gameManager;

        public int CapturedGutiCount { get; set; }
        public PlayerType PlayerType { get; protected set; }
    
        protected BasePlayer() {}

        public abstract void ReInit();

        public abstract Move GetMove();

        public GutiType GetGutiType() => gutiType;

        public void UpdateScore(Move move)
        {
            if (RuleBook.CanCaptureGuti(move)) CapturedGutiCount++;
        }

        public float GetScore() => CapturedGutiCount * gameManager.scoreboard.ScoreUnit;

    }
}