namespace OverPower.Domain.Save
{
    /// <summary>
    /// Constants and validation policies for the persistent save schema version.
    /// </summary>
    public static class SaveSchema
    {
        /// <summary>
        /// The current authoritative save schema version (independent of application bundleVersion).
        /// </summary>
        public const int CurrentVersion = 1;

        /// <summary>
        /// Gets whether a save schema version is structurally supported.
        /// </summary>
        public static bool IsSupported(int version)
        {
            return version > 0 && version <= CurrentVersion;
        }
    }
}
