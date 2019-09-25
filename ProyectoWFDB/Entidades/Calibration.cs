using ProyectoWFDB.Enumeraciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoWFDB.Entidades
{
    ///<summary>
    /// Class Calibration information = WFDB_calinfo
    /// Holds calibration specifications for signals of a given type.
    ///</summary>
    public class Calibration
    {
        /// <summary>
        /// Gets or sets the value (in physical units) corresponding to the low level of a calibration
        /// pulse. 
        /// <remarks>
        /// If the signal is AC-coupled, low is zero, and high is the pulse amplitude.
        /// </remarks>
        /// </summary>
        public double Low { get; set; }

        ///<summary>
        ///High level of calibration pulse in physical units
        ///</summary>
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the customary plotting scale, in physical units per centimeter. 
        /// <remarks>
        /// WFDB applications that produce graphical output may use scale as a default. Except
        /// in unusual circumstances, signals of different types should be plotted at equal
        /// multiples of their respective scales.
        /// </remarks>
        /// </summary>
        public double Scale { get; set; }

        /// <summary>
        /// Gets or sets a string (without embedded tabs or newlines) that describes the type(s) of signals to which the calibration specifications apply.
        /// <remarks>
        /// Usually, <see cref="SignalType"/> is an exact match to (or a prefix of) the <see cref="Signal.Description"/> field of the
        /// <see cref="Signal"/> object that describes a matching signal.
        /// </remarks>
        /// </summary>
        public string SignalType { get; set; }

        /// <summary>
        /// Gets or sets a string without embedded whitespace that specifies the physical units 
        /// of signals to which the calibration specifications apply. 
        /// <remarks>
        /// Usually, the units field of a 
        /// <see cref="Calibration"/> structure must exactly match
        /// the <see cref="Signal.Units"/> field of the <see cref="Signal"/> structure that describes a matching signal.
        /// </remarks>
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the shape of the calibration pulse.
        /// <remarks>
        /// Type is even if signals of the corresponding <see cref="SignalType"/> 
        /// are AC-coupled, and odd if they are DC-coupled.
        /// </remarks>
        /// </summary>
        public CalibrationType CalibrationType { get; set; }




    }
}
