public class Scissors : HandShape
{
    public override HandShape GetOtherHand(RoundOutcome outcome)
    {
        return outcome switch
        {
            RoundOutcome.Lost => new Rock(),
            RoundOutcome.Draw => new Scissors(),
            RoundOutcome.Won => new Paper(),
            _ => throw new NotImplementedException()
        };
    }

    public override int GetScore() => 3;

    public override RoundOutcome Play(HandShape other)
    {
        if (other.GetType() == typeof(Rock))
            return RoundOutcome.Lost;

        if (other.GetType() == typeof(Paper))
            return RoundOutcome.Won;

        if (other.GetType() == typeof(Scissors))
            return RoundOutcome.Draw;

        throw new NotImplementedException();
    }
}
