namespace ProyectoWFDB.Enumeraciones
{
    /// <summary>
    /// Represents valid modes for <see cref="PInvoke.getvec"/> when reading a multi-frequency record.
    /// </summary>
    public enum GVMode
    {
        /// <summary>
        /// Low Resolution.
        /// </summary>
        LowRes = 0,

        /// <summary>
        /// High Resolution.
        /// </summary>
        HighRes = 1,
    }
}
