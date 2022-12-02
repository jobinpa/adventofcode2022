public class Paper : HandShape
{
    public override HandShape GetOtherHand(RoundOutcome outcome)
    {
        return outcome switch
        {
            RoundOutcome.Lost => new Scissors(),
            RoundOutcome.Draw => new Paper(),
            RoundOutcome.Won => new Rock(),
            _ => throw new NotImplementedException()
        };
    }

    public override int GetScore() => 2;

    public override RoundOutcome Play(HandShape other)
    {
        if (other.GetType() == typeof(Rock))
            return RoundOutcome.Won;

        if (other.GetType() == typeof(Paper))
            return RoundOutcome.Draw;

        if (other.GetType() == typeof(Scissors))
            return RoundOutcome.Lost;

        throw new NotImplementedException();
    }
}
