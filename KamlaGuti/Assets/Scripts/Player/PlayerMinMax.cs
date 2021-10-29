using Board.Guti;

namespace Player
{
    public class PlayerMinMax : BasePlayer
    {
        private readonly MinMaxAi _minMaxAi;
        private int _explorationDepth;
        public PlayerMinMax(GutiType gutiType, PlayerType tPlayerType, GameManager gameManager, MinMaxAi minMaxAi, int explorationDepth = -1, string name = "PlayerMinMax") : base(gameManager)
        {
            _explorationDepth = explorationDepth<=0? 1: explorationDepth;
            this.gutiType = gutiType;
            base.name = name;
            PlayerType = tPlayerType;
            CapturedGutiCount = 0;
            _minMaxAi = minMaxAi;
        }


        public override Move GetMove()
        {
            var _ = 0;
            var move = _minMaxAi.MinMax(gutiType, _explorationDepth, ref _);
            UpdateScore(move);
            return move;
        }

        public override void ReInit() => CapturedGutiCount = 0;

        public void ReInit(int expDepth) => _explorationDepth = expDepth<=0? 1: expDepth;

        public MinMaxAi GetMinMaxAi() => _minMaxAi;
    
        public override string ToString() => $"Type: {"MinMaxAI"}\nDepth: {_explorationDepth}\nColor: {gutiType}\nScore: {CapturedGutiCount}";
    }
}