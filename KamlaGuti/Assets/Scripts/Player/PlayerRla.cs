using Board.Guti;

namespace Player
{
    public class PlayerRla : BasePlayer
    {
        public GutiAgent agent;
        private int _explorationDepth;
        public PlayerRla(GutiType gutiType, PlayerType tPlayerType, GameManager gameManager, GutiAgent agent, int explorationDepth = -1, string name = "PlayerRLA") : base(gameManager)
        {
            _explorationDepth = explorationDepth<=0? 1: explorationDepth;
            this.gutiType = gutiType;
            base.name = name;
            PlayerType = tPlayerType;
            CapturedGutiCount = 0;
            this.agent = agent;
            this.agent.gutiType = this.gutiType;
        }
    
        public override Move GetMove()
        {
            agent.MakeMove();
            return null;
        }
    
        public override void ReInit() => CapturedGutiCount = 0;

        public void ReInit(int expDepth) => _explorationDepth = expDepth<=0? 1: expDepth;
    
        public override string ToString() => $"Type: {PlayerType}\nDetail:{agent.name} \nDepth: {_explorationDepth}\nColor: {gutiType}\nScore: {CapturedGutiCount}";
    }
}