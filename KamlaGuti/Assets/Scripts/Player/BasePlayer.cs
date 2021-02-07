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

        public float GetScore() => CapturedGutiCount * GameManager.instance.scoreboard.ScoreUnit;

    }
}