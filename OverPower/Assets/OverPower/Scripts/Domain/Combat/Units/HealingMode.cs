namespace OverPower.Domain.Combat.Units
{
    /// <summary>
    /// Defines the healing capabilities supported by the unit stack runtime model.
    /// </summary>
    public enum HealingMode
    {
        /// <summary>
        /// Restores HP only inside the currently surviving members. Cannot resurrect lost members.
        /// </summary>
        SurvivorOnly,

        /// <summary>
        /// Restores HP and can resurrect lost members up to the stack's initial quantity.
        /// </summary>
        ReviveToInitial,

        /// <summary>
        /// Restores HP, resurrects, and can create temporary extra members above the stack's initial quantity.
        /// </summary>
        TemporaryOverflow
    }
}
