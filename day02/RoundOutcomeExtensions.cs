public static class RoundOutcomeExtensions
{
    public static RoundOutcome Invert(this RoundOutcome outcome)
    {
        return outcome switch
        {
            RoundOutcome.Won => RoundOutcome.Lost,
            RoundOutcome.Lost => RoundOutcome.Won,
            RoundOutcome.Draw => RoundOutcome.Draw,
            _ => throw new NotImplementedException()
        };
    }

    public static int GetScore(this RoundOutcome outcome)
    {
        return outcome switch
        {
            RoundOutcome.Lost => 0,
            RoundOutcome.Draw => 3,
            RoundOutcome.Won => 6,
            _ => throw new InvalidOperationException()
        };
    }
}