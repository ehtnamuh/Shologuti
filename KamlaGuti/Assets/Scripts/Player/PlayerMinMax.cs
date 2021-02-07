namespace Player
{
    public class PlayerMinMax : BasePlayer
    {
        private readonly MinMaxAi _minMaxAi;
        private int _explorationDepth;
        public PlayerMinMax(GutiType gutiType, PlayerType tPlayerType, MinMaxAi minMaxAi, int explorationDepth = -1)
        {
            _explorationDepth = explorationDepth<=0? 1: explorationDepth;
            this.gutiType = gutiType;
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


        public override void ReInit()
        {
            ReInit(0);
        }
    
        public void ReInit(int explorationDepth = -1)
        {
            _explorationDepth = explorationDepth<=0? 1: explorationDepth;
            CapturedGutiCount = 0;
        }
    
        public MinMaxAi GetMinMaxAi() => _minMaxAi;
    
        public override string ToString() => $"Type: {PlayerType}\nDepth: {_explorationDepth}\nColor: {gutiType}\nScore: {CapturedGutiCount}";
    }
}