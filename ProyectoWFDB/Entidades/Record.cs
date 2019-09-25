using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;
using System.Globalization;

namespace ProyectoWFDB.Entidades
{
    /// <summary>
    /// 
    /// </summary>
    public class Record : IDisposable
    {
        public Record(string name)
        {
            this.Name = name;
        }

        #region Properties

        /// <summary>
        /// Gets the name of the record.
        /// </summary>
        /// <remarks>
        /// You may qualify the name with the full/relative path on the hard disk. 
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Número de segmentos obtenido del fichero .hea
        /// </summary>
        public int NumberOfSegments { get; set; } = 1;

        /// <summary>
        /// Número de señales obtenido del fichero .hea
        /// </summary>
        public int NumberOfSignals { get; set; }

        private double samplingFrequencyPerSignal;
        /// <summary>
        /// Number of samples per second
        /// Gets or sets the sampling frequency for this record.
        /// Samples/Seconds/Signals .
        /// For multi-frequency  is the max sampling frequency of the signals
        /// </summary>
        public double SamplingFrequencyPerSignal
        {
            get
            {
                if (samplingFrequencyPerSignal == 0 && Signals.Count > 0)
                    return Signals.Max(s => s.SamplingFrequency);                   
                else
                    return samplingFrequencyPerSignal;
            }
            set
            {
                samplingFrequencyPerSignal = value;
                if (samplingFrequencyPerSignal != 0)
                    samplingIntervalPerSignal = 1 / samplingFrequencyPerSignal;
            }

        }

        private double samplingIntervalPerSignal;
        /// <summary>
        /// Obtiene el intervalo de muestreo, la inversa de la frecuencia de muestreo
        /// </summary>
        public double SamplingIntervalPerSignal
        {
            get
            {
                if (samplingIntervalPerSignal == 0 && Signals.Count > 0)
                    return Signals.Min(s => s.SamplingInterval);
                else
                    return samplingIntervalPerSignal;
            }
            set
            {
                samplingIntervalPerSignal = value;
                if (samplingIntervalPerSignal != 0)
                    samplingFrequencyPerSignal = 1 / samplingIntervalPerSignal;
            }

        }

        /// <summary>
        /// Número de muestras por señal obtenido del .hea
        /// O para multifrecuencias número de frame(data record)
        /// </summary>
        public int NumberOfSamplesPerSignal { get; set; }

        /// <summary>
        /// The time of day that corresponds to sample 0 in a given record. 
        /// For MIT, AHA, and ESC DB records, 
        /// the base time was not recorded and is taken to be 0:0:0 (midnight).
        /// </summary>
        public DateTime? StartDateTime { get; set; }=null;

        private double durationInSeconds;
        /// <summary>
        /// Gets the duration in seconds (NumberOfSamples / SamplingFrequency).
        /// Or for MultiFrequency, is number of frames * duration of a frame
        /// </summary>
        public double DurationInSeconds
        {
            get
            {
                if (durationInSeconds == 0 )
                    return NumberOfFrames * FrameInterval;
                else
                    return durationInSeconds;
            }
            set
            {
                durationInSeconds = value;
            }
        }

        private double counterFrequency;
        /// <summary>
        /// The difference between counter values (q.v.) that are separated by an interval of one second.
        /// The counter frequency is constant throughout any given record. 
        /// It may be undefined, in which case it is treated as equivalent to the sampling frequency (q.v.)
        /// 
        /// Counter value:A number that serves as a time reference, in a record for which a counter frequency is defined.
        /// A counter value may be converted to the time in seconds from the beginning of the record 
        /// by subtracting the base counter value (q.v.) and dividing the remainder by the counter frequency.
        /// The units of ‘c’-prefixed strtim arguments are counter values. 
        /// </summary>
        public double CounterFrequency
        {
            get
            {
                if (counterFrequency == 0)
                    return FrameFrequency;
                else
                    return counterFrequency;
            }
            set
            {
                counterFrequency = value;
            }

        }

        /// <summary>
        /// The counter value (q.v.) that corresponds to sample 0. 
        /// If not defined explicitly, the base counter value is taken to be 0.
        /// </summary>
        public double BaseCounter { get; set; }

        private int numberOfFrames;
        /// <summary>
        /// The number of frames of the signal, or number of data record for EDF format
        /// Frame: A set of samples, containing all samples that occur within a given frame interval.
        /// For an ordinary record, a frame contains exactly 1 sample of each signal; 
        /// for a multi-frequency record, a frame contains at least one sample of each signal, 
        /// and more than one sample of each oversampled signal (q.v.).
        /// </summary>
        public int NumberOfFrames
        {
            get
            {
                if (numberOfFrames == 0)
                    return NumberOfSamplesPerSignal;
                else
                    return numberOfFrames;
            }
            set
            {
                numberOfFrames = value;
            }

        }
        private double frameFrequency;
        /// <summary>
        /// Number of frames per second.
        /// The basic sampling frequency defined for a multi-frequency record; 
        /// the reciprocal of the frame interval. 
        /// The frame rate is usually the lowest sampling frequency used for any signal included in the record.
        /// </summary>
        public double FrameFrequency
        { get
            {
                if (frameFrequency == 0)
                    return SamplingFrequencyPerSignal;
                else
                    return frameFrequency;
            }
          set
            {
                frameFrequency = value;
                if (frameFrequency != 0)
                    frameInterval = 1 / frameFrequency;
            }

        }

        private double frameInterval;
        /// <summary>
        /// Obtiene el intervalo de frame, la inversa de la frecuencia de frame
        /// Representa el número de segundos que transcurre entre frames consecutivos
        /// O duración de un frame en segundos.
        /// A time interval during which at least one sample exists for each signal. 
        /// For an ordinary record, the frame interval and the sampling interval are identical. 
        /// For a multi-frequency record, the frame interval is chosen to be an integer multiple of each sampling frequency used.
        /// </summary>
        public double FrameInterval
        {
            get
            {
                if (frameInterval == 0)
                    return SamplingIntervalPerSignal;
                else
                    return frameInterval;
            }
            set
            {
                frameInterval = value;
                if(frameInterval!=0)
                frameFrequency = 1 / frameInterval;
            }

        }

        /// <summary>
        /// Gets or sets the info associated with this record.
        /// </summary>
        public List<string> Info { get; set; } = new List<string>();

        /// <summary>
        /// Gets the signals available in this record.
        /// You should call <seealso cref="Open"/> before using this member.
        /// </summary>
        private List<Signal> signals = new List<Signal>();
        /// <summary>
        /// Gets the signals associated with this record.
        /// </summary>
        public List<Signal> Signals
        {
            get
            {
                return signals;
            }
        }

        #endregion

        #region Methods Public

        /// <summary>
        /// Add a signal to list of signals set the same  sampling frequency 
        /// and number of samples of the .hea
        /// </summary>
        public void AddSignal(Signal newSignal)
        {
            if (newSignal.NumberOfSamples == 0)
                newSignal.NumberOfSamples = NumberOfSamplesPerSignal;

            if (newSignal.SamplingFrequency == 0)
                newSignal.SamplingFrequency = SamplingFrequencyPerSignal;

            var signalsSameGroup = signals.Where(s => s.FileName == newSignal.FileName).ToList();
            newSignal.Number = signalsSameGroup.Count;

            //Si no existe ninguna señal con el mismo nombre de fileSignal
            if (newSignal.Number == 0)
            {
                if (signals.Count > 0)
                    newSignal.Group = signals.Max(s => s.Group) + 1;
            }
            else
            {
                newSignal.Group = signalsSameGroup[0].Group;
            }           
            signals.Add(newSignal);
        }

        /// <summary>
        /// Convierte un segundo en el identificador de la muestra
        /// que fue realizada en dicho segundo. 
        /// </summary>
        public int SecondToIdSample(double second)
        {
            if (second > DurationInSeconds)
                throw new ArgumentException(
                    String.Format("El segundo ({0}) es mayor que la duración del registro ({1} s)"
                    , second, DurationInSeconds));


            int idSample= (int)(second*SamplingFrequencyPerSignal);

            return idSample;

        }

        /// <summary>
        /// Convierte un identificador de muestra a segundo.
        /// El segundo en el cual fue capturado la muestra
        /// </summary>
        public double IdSampleToSecond(int idSample)
        {
            if (idSample >= NumberOfSamplesPerSignal)
                throw new ArgumentException(
                    String.Format("El # de muestra ({0}) es mayor o igual que el número de muestras del registro({1})"
                    , idSample, NumberOfSamplesPerSignal));

            double second = idSample * SamplingIntervalPerSignal;

            return second;
        }


        /// <summary>
        /// Diccionario de señales agrupada por el numero de grupo.
        /// </summary>
        public Dictionary<int, List<Signal>> GetGroupedSignals()
        {

            var group = signals.GroupBy(s => s.Group).
                ToDictionary(g1 => g1.Key, g2 => g2.
                   ToList()
                );
            return group;
        }
        #endregion

        #region Static and overriden Methods

        /// <summary>
        /// Read a header file (.hea) and return the record
        /// </summary>
        /// <param name="recordName"></param>
        /// <param name="headerFile"></param>
        /// <returns></returns>
        public static Record ReadHeaderFile(string recordName, Stream headerFile)
        {
            Record registro;
            using (StreamReader streamReader = new StreamReader(headerFile))
            {
                string line = String.Empty;

                if ((line = streamReader.ReadLine()) == null)
                    throw new Exception(String.Format("ReadHeaderFile: record {0} header is empty", recordName));

                //Ignora las lineas vacias y con comentario
                while (line != null && (line.Trim().StartsWith("#") || String.IsNullOrWhiteSpace(line.Trim())))
                    line = streamReader.ReadLine();

                if (line == null)
                    throw new Exception(String.Format("ReadHeaderFile: can't find record name in record {0} header", recordName));

                //Leer linea de especificación del registro
                registro = Record.RecordLineToRecord(recordName, line);


                if (registro.NumberOfSegments == 1)
                {
                    // Por cada señal,  leer la linea de espeficiacion de señal
                    for (int signalIndex = 0; signalIndex < registro.NumberOfSignals; signalIndex++)
                    {
                        line = streamReader.ReadLine();
                        //Ignora las lineas vacias y con comentario
                        while (line != null && (line.Trim().StartsWith("#") || String.IsNullOrWhiteSpace(line.Trim())))
                            line = streamReader.ReadLine();

                        if (line == null)
                            throw new Exception(String.Format("ReadHeaderFile: unexpected EOF in header file for record {0}", recordName));

                        Signal signal = Signal.SignalLineToSignal(line);
                        registro.AddSignal(signal);
                    }

                    //Leer lineas de información del registro
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        registro.Info.Add(line.Replace("#", "").Trim());
                    }

                }
                else
                {
                    throw new NotImplementedException("Not implemented record multi-segment");
                }


            }

            return registro;
        }

        /// <summary>
        /// Read a record line from a header file (.hea)
        /// and return a record
        /// </summary>
        /// <param name="recordName"></param>
        /// <param name="recordLine"></param>
        /// <returns>Record with the filled propeties </returns>
        public static Record RecordLineToRecord(string recordName,string recordLine)
        {
            Record registro = new Record(recordName);

            //Comprueba que la  linea de registro comience con el nombre del registro
            if (!recordLine.StartsWith(recordName))
                throw new Exception(String.Format("RecordLineToRecord: record name in record {0} header is incorrect", recordName));

            string[] tokens = recordLine.Trim().Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            //Comprueba si es un formato antiguo
            if (tokens.Length < 2)
                throw new Exception(String.Format("RecordLineToRecord:obsolete format in record {0} header", recordName));

            //Divide la linea de registro en tokens y la recorre
            for (int i = 0; i < tokens.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        //Comprueba si tiene numero de segmentos en el primer token

                        if (tokens[i].Contains('/'))
                            registro.NumberOfSegments = int.Parse(tokens[i].Split('/').Last());
                        else
                            registro.NumberOfSegments = 1;
                        break;
                    case 1:
                        //Obtiene el numero de señales en el segundo token
                        registro.NumberOfSignals = int.Parse(tokens[i]);
                        break;
                    case 2: //Obtiene la velocidad de frame, frecuencia de contardor y contador base                            

                        registro.SamplingFrequencyPerSignal = double.Parse(tokens[i].Split('/').First());
                        registro.CounterFrequency = registro.SamplingFrequencyPerSignal;
                       
                        if (tokens[i].Contains('/'))
                        {
                            registro.CounterFrequency = double.Parse(tokens[i].Split('/', '(')[1]);

                            if (tokens[i].Contains('('))
                            {

                                registro.BaseCounter = double.Parse(tokens[i].Split('(', ')')[1]);
                            }

                        }
                        break;
                    case 3:
                        //Obtiene el número de muestras por señal o duración (calculando la frecuencia de muestreo)
                        registro.NumberOfSamplesPerSignal = int.Parse(tokens[i]);
                        break;
                    case 4:
                        //Obtiene la hora de inicio del registro
                        DateTime dt;
                        if (!DateTime.TryParseExact(tokens[i], "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                            if (!DateTime.TryParseExact(tokens[i], "hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                if (!DateTime.TryParseExact(tokens[i], "H:m:s", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                    if (!DateTime.TryParseExact(tokens[i], "h:m:s", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                        dt = new DateTime();

                        registro.StartDateTime = new DateTime(
                              1,
                              1,
                              1,
                              dt.Hour,
                              dt.Minute,
                              dt.Second
                              );
                        break;
                    case 5:// Obtiene la fecha de inicio del registro
                        if (!DateTime.TryParseExact(tokens[i], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                            if (!DateTime.TryParseExact(tokens[i], "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                if (!DateTime.TryParseExact(tokens[i], "d/M/y", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                    dt = new DateTime();
                        registro.StartDateTime = new DateTime(
                            dt.Year,
                            dt.Month,
                            dt.Day,
                            registro.StartDateTime.Value.Hour,
                            registro.StartDateTime.Value.Minute,
                            registro.StartDateTime.Value.Second
                            );
                        break;
                    default: break;
                }
            }


            return registro;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Dispose()
        {
           
            foreach (var signal in Signals)
            {
                signal.Dispose();
            }

            signals.Clear();
            Info.Clear();                    
        }



        #endregion

    }
}
