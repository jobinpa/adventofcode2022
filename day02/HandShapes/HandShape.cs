public abstract class HandShape
{
    public abstract HandShape GetOtherHand(RoundOutcome outcome);
    public abstract int GetScore();
    public abstract RoundOutcome Play(HandShape other);
}
