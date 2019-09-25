using System;
using ProyectoWFDB.Entidades;

namespace ProyectoWFDB.Enumeraciones
{
    /// <summary>
    /// Legal values for the <see cref="Signal.Format"/> member.
    /// </summary>
    [Flags]
    public enum SignalStorageFormat
    {

        /// <summary>
        /// Null signal (nothing read or written)
        /// </summary>
        NullSignal = 0,

        /// <summary>
        /// 8-bit first differences
        /// </summary>
        Sf8Bit = 8,

        /// <summary>
        /// 16-bit 2's complement amplitudes, low byte first
        /// </summary>
        Sf16Bit = 16,

        /// <summary>
        /// 24-bit 2's complement amplitudes, low byte first
        /// </summary>
        Sf24Bit = 24,

        /// <summary>
        /// 32-bit 2's complement amplitudes, low byte first
        /// </summary>
        Sf32Bit = 32,

        /// <summary>
        /// 16-bit 2's complement amplitudes, high byte first
        /// </summary>
        Sf61Bit = 61,

        /// <summary>
        /// 8-bit offset binary amplitudes
        /// </summary>
        Sf80Bit = 80,

        /// <summary>
        /// 16-bit offset binary amplitudes
        /// </summary>
        Sf160Bit = 160,

        /// <summary>
        /// 2 12-bit amplitudes bit-packed in 3 bytes
        /// </summary>
        Sf212Bit = 212,

        /// <summary>
        /// 3 10-bit amplitudes bit-packed in 4 bytes
        /// </summary>
        Sf310Bit = 310,

        /// <summary>
        /// 3 10-bit amplitudes bit-packed in 4 bytes
        /// </summary>
        Sf311Bit = 311

    }
}
