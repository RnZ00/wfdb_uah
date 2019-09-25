namespace ProyectoWFDB.Enumeraciones
{
    /// <summary>
    /// Holds the supported values for the <see cref="CalibrationType"/> Property
    /// <remarks>
    /// <see cref="AcCoupled"/> and <see cref="DcCoupled"/> are used in combination with the pulse
    /// shape definitions in this enumeration to characterize calibration pulses.
    /// </remarks>
    /// </summary>
    public enum CalibrationType
    {
        /// <summary>
        /// AC coupled signal
        /// </summary>
        AcCoupled = 0,

        /// <summary>
        /// DC coupled signal
        /// </summary>
        DcCoupled = 1,

        /// <summary>
        /// Square wave pulse
        /// </summary>
        CalSquare = 2,

        /// <summary>
        /// Sine wave pulse
        /// </summary>
        CalSine = 4,

        /// <summary>
        /// Sawtooth pulse
        /// </summary>
        CalSawtooth = 6,

        /// <summary>
        /// Undefined pulse shape
        /// </summary>
        CalUndef = 8
    }
}
