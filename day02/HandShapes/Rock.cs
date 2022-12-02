public class Rock : HandShape
{
    public override HandShape GetOtherHand(RoundOutcome outcome)
    {
        return outcome switch
        {
            RoundOutcome.Lost => new Paper(),
            RoundOutcome.Draw => new Rock(),
            RoundOutcome.Won => new Scissors(),
            _ => throw new NotImplementedException()
        };
    }

    public override int GetScore() => 1;

    public override RoundOutcome Play(HandShape other)
    {
        if (other.GetType() == typeof(Rock))
            return RoundOutcome.Draw;

        if (other.GetType() == typeof(Paper))
            return RoundOutcome.Lost;

        if (other.GetType() == typeof(Scissors))
            return RoundOutcome.Won;

        throw new NotImplementedException();
    }
}
