namespace Player
{
    public class PlayerRla : BasePlayer
    {
        private readonly GutiAgent _agent;
        private int _explorationDepth;
        public PlayerRla(GutiType gutiType, PlayerType tPlayerType, GutiAgent agent, int explorationDepth = -1)
        {
            _explorationDepth = explorationDepth<=0? 1: explorationDepth;
            this.gutiType = gutiType;
            PlayerType = tPlayerType;
            CapturedGutiCount = 0;
            _agent = agent;
            _agent.gutiType = this.gutiType;
        }
    
        public override Move GetMove()
        {
            _agent.MakeMove();
            return null;
        }
    
        public override void ReInit()
        {
            ReInit(0);
        }
    
        public void ReInit(int explorationDepth = -1)
        {
            _explorationDepth = explorationDepth<=0? 1: explorationDepth;
            CapturedGutiCount = 0;
        }
    
        public override string ToString() => $"Type: {PlayerType}\nDepth: {_explorationDepth}\nColor: {gutiType}\nScore: {CapturedGutiCount}";
    }
}