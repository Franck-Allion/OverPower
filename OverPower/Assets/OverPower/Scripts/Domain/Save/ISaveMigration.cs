namespace OverPower.Domain.Save
{
    /// <summary>
    /// Lightweight abstraction and seam for future save schema migrations.
    /// Actual concrete migrations will be implemented when newer schema versions are defined.
    /// </summary>
    public interface ISaveMigration
    {
        /// <summary>
        /// Gets the source schema version this migration starts from.
        /// </summary>
        int SourceVersion { get; }

        /// <summary>
        /// Gets the target schema version this migration upgrades to.
        /// </summary>
        int TargetVersion { get; }
    }
}
