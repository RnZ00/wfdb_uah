using ProyectoWFDB.Entidades;
using ProyectoWFDB.Enumeraciones;
//using ProyectoWFDB.TiposDatosSimples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ProyectoWFDB.Entidades
{
    /// <summary>
    /// Holds the name and global attributes of a given signal.
    /// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    public class Signal : IDisposable   
    {
        public Signal() { }

        #region Properties



        /// <summary>
        /// Name of the file in which samples of the associated signal are stored.
        /// <remarks>
        /// Input signal files are found by prefixing FileName with
        /// each of the components of the database path in turn.
        /// FileName may include relative or absolute path specifications
        /// if necessary; the use of an absolute pathname, combined with an initial null
        /// component in WFDB, reduces the time needed to find the signal file to a minimum.
        /// If FileName is ‘-’, it refers to the standard input or output.
        /// </remarks>
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The signal group number. All signals in a given group are stored in the same file.
        /// If there are two or more signals in a group, the file is called a multiplexed signal
        /// file. Group numbers begin at 0; arrays of Signal are always
        /// kept ordered with respect to the group number, so that signals belonging to the
        /// same group are described by consecutive entries in the array.
        /// </summary>
        public int Group { get; set; }

        /// <summary>
        /// Gets the signal number.
        /// Function findsig finds an open input signal with the name specified by its
        /// (string) argument, and returns the associated signal number.If the argument
        ///is a decimal numeral and is less than the number of open input signals, it is
        ///assumed to represent a signal number, which is returned.Otherwise, findsig
        ///looks for a signal with a description matching the string, and returns the
        ///first match if any, or -1 if not.
        /// </summary>
        /// <remarks>
        /// The current wrapper implementation uses the signal's description to recover the underlying number since it's the only available method to find it (<see cref="PInvoke.findsig"/>).
        /// This implies that the property getter would potentially fail in case two or more signals share the same description.
        /// </remarks>
        public int Number { get; set; }

        /// <summary>
        /// Signal's description text.
        /// <remarks>
        /// This is a string without embedded newlines (e.g., ‘ECG lead V1’ or ‘trans-thoracic impedance’). 
        /// </remarks>
        /// </summary>
        public string Description { get; set; }

        private int initValue;
        /// <summary>
        /// The initial value of the associated signal (i.e., the value of sample number 0).
        /// </summary>
        public int InitValue
        {
            get
            {
                if (initValue == 0)
                    return AdcZero;
                return initValue;
            }
            set
            {
                initValue = value;             
            }

        }

        /// <summary>
        /// The number of analog-to-digital converter units (adus) per physical unit (<see cref="Units"/>)
        /// relative to the original analog signal; for an ECG, this is roughly
        /// equal to the amplitude of a normal QRS complex. If gain is zero, no amplitude
        /// calibration is available; in this case, a gain of 200
        /// may be assumed.
        /// </summary>
        public double Gain { get; set; } = 200;

        /// <summary>
        /// The value of ADC output that would map to 0 physical units input.
        /// The sample value that corresponds to the baseline (isoelectric level or physical zero level) in the signal.
        /// <remarks>
        /// The value of AdcZero is not synonymous with that of Baseline; the Baseline is a characteristic of the signal, while
        /// AdcZero is a characteristic of the digitizer. The value of baseline need not
        /// necessarily lie within the output range of the ADC; for example, if the units
        /// are ‘degrees_Kelvin’, and the ADC range is 200–300 degrees Kelvin, baseline
        /// corresponds to absolute zero, and lies well outside the range of values actually
        /// produced by the ADC.
        /// </remarks>
        /// </summary>
        public int Baseline { get; set; }

        /// <summary>
        /// The signal storage format. 
        /// <remarks>
        /// The most commonly-used formats are format 8 (8-bit
        /// first differences), format 16 (16-bit amplitudes), and format 2
        /// (pairs of 12-bit amplitudes bit-packed into byte triplets).
        /// See <see cref="SignalStorageFormat"/> enumeration for a complete
        /// list of supported formats.
        /// All signals belonging to the same group must be
        /// stored in the same format.
        /// </remarks>
        /// </summary>
        public SignalStorageFormat Format{ get; set;}

        /// <summary>
        /// Specifies the physical units of the signal; if null, the units are assumed to
        /// be millivolts (mV unless otherwise specified).
        /// </summary>
        public string Units { get; set; } = "mV";

        /// <summary>
        /// The number of samples per frame. This is 1, for all except oversampled signals
        /// in multi-frequency records, for which spf may be any positive integer.
        /// <remarks>
        /// Note that non-integer values are not permitted (thus the frame rate must be chosen
        /// such that all sampling frequencies used in the record are integer multiples of
        /// the frame rate).
        /// </remarks>
        /// </summary>
        public int SamplesPerFrame { get; set; } = 1;

        /// <summary>
        /// The block size, in bytes.
        /// <remarks>
        /// For signal files that reside on Unix character device
        /// special files (or their equivalents), the BlockSize field indicates how many bytes
        /// must be read or written at a time. For ordinary disk files, BlockSize is zero. 
        /// All signals belonging to a given group have the same BlockSize.
        /// </remarks>
        /// </summary>
        public int BlockSize { get; set; }

        /// <summary>
        /// The ADC(Analog-to-digital converter) resolution in bits.
        /// The number of significant bits per sample. 
        /// <remarks>
        /// Typical ADCs have resolutions between 8 and 16
        /// bits inclusive.
        /// </remarks>
        /// </summary>
        public int AdcResolution { get; set; } = 12;

        /// <summary>
        /// The value produced by the ADC(Analog-to-digital converter) given a 0 volt input.
        /// The ADC output given an input that falls exactly at the center of the ADC
        /// range (normally 0 VDC)
        /// The value produced by the ADC given a 0 volt input..
        /// <remarks>
        /// Bipolar ADCs produce two’s complement output; for
        /// these, AdcZero is usually zero. For the MIT DB, however, an offset binary
        /// ADC was used, and AdcZero was 1024.
        /// </remarks>
        /// </summary>
        public int AdcZero { get; set; }

        /// <summary>
        /// The number of samples in the signal.
        /// <remarks>
        /// All signals in a given record must have the same NumberOfSamples. If NumberOfSamples is
        /// zero, the number of samples is unspecified, and the cksum 
        /// is not used; this is useful for specifying signals that are obtained from pipes,
        /// for which the length may not be known.
        /// </remarks>
        /// </summary>
        public int NumberOfSamples { get; set; }

        /// <summary>
        /// A 16-bit checksum of all samples. This field is not usually accessed by application
        /// programs; newheader records checksums calculated by putvec when it creates a
        /// new ‘hea’ file, and getvec compares checksums that it calculates against cksum
        /// at the end of the record, provided that the entire record was read through
        /// without skipping samples.
        /// </summary>
        public int CheckSum { get; set; }

        /// <summary>
        /// Gets or sets the intersignal's skew.
        /// Skew: The time difference between samples having the same sample number but belonging to different signals.
        /// Ideally the skew is zero (or less than one sample interval), 
        /// but in some cases this is not so. 
        /// For example, if the signals were originally recorded on multitrack analog tape, 
        /// very small differences in the azimuth of the recording and playback heads may result in measurable skew among signals. 
        /// If the skew can be measured (for example, by reference to features of two signals with a known time difference),
        /// it can be recorded in the header file for a record; once this has been done
        /// Prospectively, if you anticipate that skew may be a problem, 
        /// it is a good idea to apply an easily identifiable synchronization pulse to all your inputs simultaneously
        /// while recording; you can then locate this pulse in each digitized signal and use these measurements to correct for skew.
        /// </summary>
        public int Skew { get; set; }
       
        /// <summary>
        /// Desplazamiento en byte de la primera muestra de la señal.
        /// O el tamaño del registro de encabezado de un fichero edf.
        /// </summary>
        public int ByteOffset { get; set; }

        private double samplingFrequency;
        /// <summary>
        /// Frecuencia de muestreo de la señal.
        /// FrameFrequency:The basic sampling frequency defined for a multi-frequency record; 
        /// the reciprocal of the frame interval. 
        /// The frame rate is usually the lowest sampling frequency used for any signal included in the record.
        /// </summary>
        public double SamplingFrequency
        {
            get
            {
                return samplingFrequency;
            }
            set
            {
                samplingFrequency = value;
                if (samplingFrequency != 0)
                    samplingInterval = 1 / samplingFrequency;
            }

        }

        private double samplingInterval;
        /// <summary>
        /// Obtiene el intervalo de muestreo, la inversa de la frecuencia de muestreo
        /// Representa el número de segundos que transcurre entre muestras consecutivas
        /// A time interval during which at least one sample exists for each signal. 
        /// For an ordinary record, the frame interval and the sampling interval are identical. 
        /// For a multi-frequency record, the frame interval is chosen to be an integer multiple of each sampling frequency used.
        /// </summary>
        public double SamplingInterval
        {
            get
            {
                    return samplingInterval;
            }
            set
            {
                samplingInterval = value;
                if (samplingInterval != 0)
                    samplingFrequency = 1 / samplingInterval;
            }

        }

        #endregion

        private List<int> samples = new List<int>();
        /// <summary>
        /// Lista de todas las muestra de la señal.
        /// The unit of amplitude for samples is ADU.
        /// </summary>
        public List<int> Samples { get { return samples; } }

        public void AddSample(int adu)
        {
            Samples.Add(adu);
        }

        /// <summary>
        /// Convierte un valor ADU de una muestra seleccionada (idSample) de ADU a Unidades Fisicas (mV)
        /// ((adu - Baseline) / Gain)
        /// </summary>
        public double SampleToPhysicalUnits(int idSample)
        {
            if(idSample>= samples.Count)
            {
                throw new ArgumentException(String.Format(
                    "Id de la muestra {0} es mayor que el número total de muestras de la señal {1}",
                    idSample.ToString(), samples.Count.ToString()));
            }
            double pUnits= Math.Round((samples[idSample] - Baseline) / Gain, 3);
            return pUnits;
        }

        /// <summary>
        /// Convierte un identificador de muestra a segundo.
        /// El segundo en el cual fue capturado la muestra
        /// </summary>
        public double IdSampleToSecond(int idSample)
        {
            if (idSample >= NumberOfSamples)
                throw new ArgumentException(
                    String.Format("El # de muestra ({0}) es mayor o igual que el número de muestras de la señal({1})"
                    , idSample, NumberOfSamples));

            double second = idSample * SamplingInterval;
            return second;
        }

        /// <summary>
        /// Convierte un segundo en el identificador de la muestra
        /// que fue realizada en dicho segundo. 
        /// </summary>
        public int SecondToIdSample(double second)
        {
            double durationTotal = NumberOfSamples / SamplingFrequency;
            if (second > durationTotal)
                throw new ArgumentException(
                    String.Format("El segundo ({0}) es mayor que la duración del registro ({1} s)"
                    , second, durationTotal));

            int idSample = (int)(second * SamplingFrequency);
            return idSample;

        }



        /// <summary>
        /// Read a signal specification line and return a signal
        /// </summary>
        /// <param name="signalLine"></param>
        /// <returns></returns>
        public static Signal SignalLineToSignal(string signalLine)
        {
            Signal newSignal = new Signal();

            string[] tokens = signalLine.Trim().Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            for (int i = 0; i < tokens.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        //Obtener el nombre de fichero de señal
                        newSignal.FileName = tokens[i];
                        break;
                    case 1:
                        //Obtener formato de la señal, muestras por frame, skew y desplazamiento en byte
                        newSignal.Format = (SignalStorageFormat)int.Parse(tokens[i].ToLower().Split('x', ':', '+').First());

                        newSignal.SamplesPerFrame = (tokens[i].ToLower().Contains("x")) ?
                             int.Parse(tokens[i].ToLower().Split('x', ':', '+')[1]) : 1;

                        newSignal.Skew = (tokens[i].Contains(":")) ?
                             int.Parse(tokens[i].Split(':', '+')[1]) : 0;

                        newSignal.ByteOffset = (tokens[i].Contains("+")) ?
                             int.Parse(tokens[i].Split('+')[1]) : 0;

                        switch (newSignal.Format)
                        {
                            case SignalStorageFormat.Sf80Bit:
                                newSignal.AdcResolution = 8;
                                break;
                            case SignalStorageFormat.Sf160Bit:
                                newSignal.AdcResolution = 16;
                                break;
                            case SignalStorageFormat.Sf212Bit:
                                newSignal.AdcResolution = 12;
                                break;
                            case SignalStorageFormat.Sf310Bit:
                                newSignal.AdcResolution = 10;
                                break;
                            case SignalStorageFormat.Sf311Bit:
                                newSignal.AdcResolution = 10;
                                break;
                            default:
                                newSignal.AdcResolution = 12;
                                break;
                        }
                        break;
                    case 2:
                        //Obtener ganancia, linea base y las unidades fisica
                        newSignal.Gain = double.Parse(tokens[i].Split('(', '/')[0], CultureInfo.InvariantCulture);
                        newSignal.Baseline = (tokens[i].Contains("(")) ? int.Parse(tokens[i].Split('(', ')')[1]) : 0;
                        newSignal.Units = (tokens[i].Contains('/')) ? tokens[i].Split('/')[1] : "mV";
                        break;
                    case 3:
                        //Obtener resolucion ADC
                        newSignal.AdcResolution = int.Parse(tokens[i]);
                        break;
                    case 4:
                        //Obtener ADC Cero
                        newSignal.AdcZero = int.Parse(tokens[i]);
                        break;
                    case 5:
                        //Valor Inicial
                        newSignal.InitValue = int.Parse(tokens[i]);
                        break;
                    case 6:
                        //Suma de comprobación
                        newSignal.CheckSum = int.Parse(tokens[i]);
                        break;
                    case 7:
                        //Tamaño del bloque
                        newSignal.BlockSize = int.Parse(tokens[i]);
                        break;

                    default:
                        //Descripcion
                        newSignal.Description = (newSignal.Description + " " + tokens[i]).Trim();
                        break;
                }
            }

            return newSignal;
        }



        ///<summary>
        ///Lee un fichero de señal de un registro
        ///Y llena las muestras de cada señal perteneciente al mismo grupo
        ///</summary>
        public static void ReadSignalFile(FileStream fileSignal, List<Signal> signalsGroup)
        {       
                fileSignal.Position = signalsGroup[0].ByteOffset;

                switch (signalsGroup[0].Format)
                {
                    case SignalStorageFormat.Sf8Bit:
                    throw new NotImplementedException("Format SignalStorageFormat.Sf8Bit not implemented");
                       

                    case SignalStorageFormat.Sf16Bit:
                        ReadSignalF16(fileSignal, signalsGroup); break;

                    case SignalStorageFormat.Sf24Bit:
                        ReadSignalF24(fileSignal, signalsGroup);
                        break;

                    case SignalStorageFormat.Sf32Bit:
                        ReadSignalF32(fileSignal, signalsGroup); break;

                    case SignalStorageFormat.Sf61Bit:
                        ReadSignalF61(fileSignal, signalsGroup);
                        break;

                    case SignalStorageFormat.Sf80Bit:
                        ReadSignalF80(fileSignal, signalsGroup);
                        break;

                    case SignalStorageFormat.Sf160Bit:
                        ReadSignalF160(fileSignal, signalsGroup);
                        break;

                    case SignalStorageFormat.Sf212Bit:
                        ReadSignalF212(fileSignal, signalsGroup); break;

                    case SignalStorageFormat.Sf310Bit:
                    throw new NotImplementedException("Format SignalStorageFormat.Sf310Bit not Implemented");

                  
                    case SignalStorageFormat.Sf311Bit:
                    throw new NotImplementedException("Format SignalStorageFormat.Sf310Bit not Implemented");
                    
                    default:
                    throw new ArgumentException("Format not valid:"+ signalsGroup[0].Format);
            }                    
        }

        ///<summary>
        ///Lee el fichero de señales de formato Sf212Bit
        ///</summary>
        private static void ReadSignalF212(FileStream fileSignal, List<Signal> signals)
        {
            List<int> samples = new List<int>();
            int[] bytes = new int[3];
            int i, lastByteRead = 0;
            int numberSignal = 0, muestrasPorIteracion = 2;
            int muestra = 0, lsb = 0, msb = 0;
            while (true)
            {
                for (i = 0; i < bytes.Length; i++)
                {
                    lastByteRead = fileSignal.ReadByte();
                    if (lastByteRead == -1) break;
                    bytes[i] = lastByteRead;
                }

                if (lastByteRead == -1) break;

                for (int s = 0; s < muestrasPorIteracion; s++)
                {
                    muestra = 0;
                    switch (s)
                    {
                        case 0:
                            lsb = bytes[0];
                            // 0000 xxxx 0000 0000                           
                            msb = ((bytes[1] & 0b0000_1111) << 8);
                            break;
                        case 1:
                            lsb = bytes[2];
                            // 0000 xxxx 0000 0000
                            msb = ((bytes[1] >> 4) << 8);
                            break;
                    }
                    //Si tiene que extender el bit de signo
                    // 1111 xxxx xxxx xxxx
                    if (msb >= 2048)
                        muestra = (0b1111 << 12);

                    muestra = (muestra | msb | lsb);

                    numberSignal = numberSignal % signals.Count;
                    signals[numberSignal].AddSample((short)muestra);
                    numberSignal++;
                }


            }
        }

        ///<summary>
        ///Lee el fichero de señales de formato Sf16Bits
        ///</summary>
        private static void ReadSignalF16(FileStream fileSignal, List<Signal> signals)
        {
            List<int> samples = new List<int>();
            int LSByte;// Byte menos significativo
            int MSByte;//Byte mas significativo
            int numberSignal = 0;
            while (true)
            {
                LSByte = fileSignal.ReadByte();
                if (LSByte == -1) break;
                MSByte = fileSignal.ReadByte();

                // Desplazar 1 byte a la derecha: abcd efgh 0000 0000
                ushort wordMSB = (ushort)(MSByte << 8);
                ushort wordLSB = (ushort)(LSByte);
                short muestra = (short)(wordMSB | wordLSB);
                numberSignal = numberSignal % signals.Count;
                signals[numberSignal].AddSample(muestra);
                numberSignal++;
            }

        }

        ///<summary>
        ///Lee el fichero de señales de formato Sf24Bits
        ///</summary>
        private static void ReadSignalF24(FileStream fileSignal, List<Signal> signals)
        {
            List<int> samples = new List<int>();
            int[] bytes = new int[3];
            int i, lastByteRead = 0;
            int numberSignal = 0, muestra = 0;
            while (true)
            {
                for (i = 0; i < bytes.Length; i++)
                {
                    lastByteRead = fileSignal.ReadByte();
                    if (lastByteRead == -1) break;
                    bytes[i] = lastByteRead << (i * 8);
                }

                if (lastByteRead == -1) break;

                //Si es negativo el byte mas significativo, extender el signo
                //[1111 1111] [xxxx xxxx] [xxxx xxxx] [xxxx xxxx]
                if (lastByteRead > 127)
                    muestra = 0b1111_1111 << 24;
                else
                    muestra = 0;

                for (i = bytes.Length - 1; i >= 0; i--)
                {
                    muestra = muestra | bytes[i];
                }
                numberSignal = numberSignal % signals.Count;
                signals[numberSignal].AddSample(muestra);
                numberSignal++;
            }
        }

        ///<summary>
        ///Lee el fichero de señales de formato Sf32Bits
        ///</summary>
        private static void ReadSignalF32(FileStream fileSignal, List<Signal> signals)
        {
            List<int> samples = new List<int>();
            int[] bytes = new int[4];
            int i, lastByteRead = 0;
            int numberSignal = 0, muestra = 0;
            while (true)
            {
                for (i = 0; i < bytes.Length; i++)
                {
                    lastByteRead = fileSignal.ReadByte();
                    if (lastByteRead == -1) break;
                    bytes[i] = lastByteRead << (i * 8);
                }

                if (lastByteRead == -1) break;
                muestra = 0;
                for (i = bytes.Length - 1; i >= 0; i--)
                {
                    muestra = muestra | bytes[i];
                }
                numberSignal = numberSignal % signals.Count;
                signals[numberSignal].AddSample(muestra);
                numberSignal++;
            }
        }

        ///<summary>
        ///Lee el fichero de señales de formato Sf61Bits
        ///</summary>
        private static void ReadSignalF61(FileStream fileSignal, List<Signal> signals)
        {
            List<int> samples = new List<int>();
            int LSByte;// Byte menos significativo
            int MSByte;//Byte mas significativo
            int numberSignal = 0;
            while (true)
            {
                MSByte = fileSignal.ReadByte();
                if (MSByte == -1) break;
                LSByte = fileSignal.ReadByte();

                // Desplazar 1 byte a la derecha: abcd efgh 0000 0000
                ushort wordMSB = (ushort)(MSByte << 8);
                ushort wordLSB = (ushort)(LSByte);
                short muestra = (short)(wordMSB | wordLSB);
                numberSignal = numberSignal % signals.Count;
                signals[numberSignal].AddSample(muestra);
                numberSignal++;
            }
        }

        ///<summary>
        ///Lee el fichero de señales de formato Sf80Bit
        ///</summary>
        private static void ReadSignalF80(FileStream fileSignal, List<Signal> signals)
        {
            List<int> samples = new List<int>();
            int byteLeido, exceso = 128;
            int numberSignal = 0;
            while (true)
            {
                byteLeido = fileSignal.ReadByte();
                if (byteLeido == -1) break;

                int muestra = byteLeido - exceso;
                numberSignal = numberSignal % signals.Count;
                signals[numberSignal].AddSample(muestra);
                numberSignal++;
            }

        }

        ///<summary>
        ///Lee el fichero de señales de formato Sf160Bit
        ///</summary>
        private static void ReadSignalF160(FileStream fileSignal, List<Signal> signals)
        {
            List<int> samples = new List<int>();
            int msb, lsb, exceso = 32768;
            int numberSignal = 0;
            while (true)
            {
                lsb = fileSignal.ReadByte();
                if (lsb == -1) break;
                msb = fileSignal.ReadByte() << 8;

                int muestra = ((msb | lsb) - exceso);
                numberSignal = numberSignal % signals.Count;
                signals[numberSignal].AddSample(muestra);
                numberSignal++;
            }

        }


        #region Overriden Methods
        public override int GetHashCode()
        {
            unchecked
            {
                int result = FileName.GetHashCode();
                result = (result * 397) ^ Description.GetHashCode();
                result = (result * 397) ^ SamplesPerFrame;
                result = (result * 397) ^ NumberOfSamples.GetHashCode();
                return result;
            }
        }


        public override string ToString()
        {
            var result = string.Format("Signal {0}, {1}, {2} samples, {3}", this.Number, this.Description, this.NumberOfSamples, this.Format);
            return result;
        }

        public void Dispose()
        {
            samples.Clear();
            
        }


        #endregion



    }
}