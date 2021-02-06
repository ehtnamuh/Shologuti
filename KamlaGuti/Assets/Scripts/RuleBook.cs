public static class RuleBook
{
    public static GutiMap gutiMap;
    public static int maxStepCount;
    public static int winningScore;

    public static bool CanContinueTurn(Move move) => (gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress) &&  gutiMap.CanCaptureGuti(move.targetAddress));
    public static bool CanCaptureGuti(Move move) => gutiMap.CanCaptureGuti(move.sourceAddress, move.targetAddress);
    public static bool MaxStepExceeded() => false;
}
