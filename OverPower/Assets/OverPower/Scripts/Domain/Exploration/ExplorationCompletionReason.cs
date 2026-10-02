namespace OverPower.Domain.Exploration
{
    /// <summary>
    /// Identifies why an exploration room session completed.
    /// </summary>
    public enum ExplorationCompletionReason
    {
        None = 0,
        ActionPointsExhausted,
        EndedExplicitly
    }
}
