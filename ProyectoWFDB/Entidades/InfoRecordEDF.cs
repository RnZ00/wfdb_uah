using ProyectoWFDB.Enumeraciones;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoWFDB.Entidades
{
    /// <summary>
    ///  Clase que almacena la información en ASCII de registro de encabezado de un fichero EDF
    /// </summary>
    class InfoRecordEDF
    {
        private FileStream fileEDF;
        public InfoRecordEDF(FileStream fs)
        {
            fileEDF = fs;
            int offset = 0, count=256;
            byte[] bytes;

            bytes= new byte[count];
            //Leer los 256 primeros bytes
            fileEDF.Read(bytes,0,count);
            List<byte> listBytes = bytes.ToList();
            
            Version = Encoding.ASCII.GetString(listBytes.GetRange(0, 8).ToArray()).Trim();
            IdPatient = Encoding.ASCII.GetString(listBytes.GetRange(8, 80).ToArray()).Trim();
            IdRecording = Encoding.ASCII.GetString(listBytes.GetRange(88, 80).ToArray()).Trim();
            StartDateRecording = Encoding.ASCII.GetString(listBytes.GetRange(168, 8).ToArray()).Trim();
            StartTimeRecording = Encoding.ASCII.GetString(listBytes.GetRange(176, 8).ToArray()).Trim();
            BytesHeaderRecord = Encoding.ASCII.GetString(listBytes.GetRange(184, 8).ToArray()).Trim();
            Reserved = Encoding.ASCII.GetString(listBytes.GetRange(192, 44).ToArray()).Trim();
            NumberDataRecords = Encoding.ASCII.GetString(listBytes.GetRange(236, 8).ToArray()).Trim();
            DurationDataRecord = Encoding.ASCII.GetString(listBytes.GetRange(244, 8).ToArray()).Trim();
            NumberSignals = Encoding.ASCII.GetString(listBytes.GetRange(252, 4).ToArray()).Trim();
            
                  
            if (Version != "0" && Version != "BIOSEMI")
                throw new ArgumentException("Versión de EDF no valida, se esperba '0' o 'BIOSEMI': " + Version);
      

            if((numberSignals+1)*256!= bytesHeaderRecord)
                 throw new ArgumentException(String.Format("Se esperaba un tamaño del registro de encabezado de {0} Bytes " +
                     "y se leyo {1} bytes como tamaño del registro de encabezado:" , (numberSignals + 1) * 256, bytesHeaderRecord));


            List<int> serie;
            int lenghtField=0;
            string bytesASCII;
            string[] fieldX;

            offset = 256;
            //Llenar los 10 campos de la señal
            for (int fieldSignal = 0; fieldSignal <10 ; fieldSignal++)
            {
                switch (fieldSignal)
                {
                    case 0://Label and PhysicalDim
                        lenghtField = 16;
                        break;
                    case 1:
                    case 7:
                        //Transductor and Prefiltering
                        lenghtField = 80;
                        break;
                    case 9:
                        //ReservedSignal
                        lenghtField = 32;
                        break;
                    default:
                        //Other Fields
                        lenghtField = 8;
                        break;
                }

                count = lenghtField * numberSignals;
                serie = Enumerable.Repeat(0, numberSignals).Select((x, i) => x + i * lenghtField).ToList();
                bytes = new byte[count];
                fileEDF.Read(bytes, 0, count);
                bytesASCII = Encoding.ASCII.GetString(bytes);
                fieldX = serie.Select(stIndex => bytesASCII.Substring(stIndex, lenghtField).Trim()).ToArray();
                offset = offset + count;

                switch (fieldSignal)
                {
                    case 0: Label = fieldX; break;
                    case 1: Transducer = fieldX; break;
                    case 2: PhysicalDim = fieldX; break;
                    case 3: PhysicalMin = fieldX; break;
                    case 4: PhysicalMax = fieldX; break;
                    case 5: DigitallMin = fieldX; break;
                    case 6: DigitalMax = fieldX; break;
                    case 7: Prefiltering = fieldX; break;
                    case 8: NumberSamplesDataRecord = fieldX; break;
                    case 9: ReservedSignal = fieldX; break;
                }

            }
        }

        public string Version { get; private set; }

        public string IdPatient { get; private set; }

        public string IdRecording { get; private set; }

        public string StartDateRecording { get; private set; }

        public string StartTimeRecording { get; private set; }

        private int bytesHeaderRecord;
        public string BytesHeaderRecord
        {
            get
            {
                return bytesHeaderRecord.ToString();
            }
            private set
            {
                if (!int.TryParse(value, out bytesHeaderRecord))
                    throw new ArgumentException("Número de bytes del registro de encabezado invalido:" + value);
            }

        }

        public string Reserved { get; private set; }

        private int numberDataRecords;
        public string NumberDataRecords
        {
            get
            {
                return numberDataRecords.ToString();
            }
            private set
            {
                if (!int.TryParse(value, out numberDataRecords))
                    throw new ArgumentException("Número de registros de datos invalido:" + value);
            }

        }


        private double durationDataRecord;
        public string DurationDataRecord
        {
            get
            {
                return durationDataRecord.ToString();
            }
            private set
            {
                if (!double.TryParse(value,NumberStyles.None,CultureInfo.InvariantCulture ,out durationDataRecord))
                    throw new ArgumentException("Duración del registro de datos invalido:" + value);

                if (durationDataRecord <= 0.0)
                    durationDataRecord = 1;

            }

        }

        private int numberSignals;
        public string NumberSignals
        {
            get
            {
                return numberSignals.ToString();
            }
            private set
            {
                if (!int.TryParse(value, out numberSignals))
                    throw new ArgumentException("Número de señales invalido:" + value);
            }

        }



        public string[] Label { get; private set; }

        public string[] Transducer { get; private set; }
     
        public string[] PhysicalDim { get; private set; }

        private double[] physicalMin=new double[0];
        public string[] PhysicalMin
        {
            get
            {
                return physicalMin.Select(x=>x.ToString()).ToArray();
            }
            private set
            {
                physicalMin = value.Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            }

        }

        private double[] physicalMax=new double[0];
        public string[] PhysicalMax
        {
            get
            {
                return physicalMax.Select(x => x.ToString()).ToArray();
            }
            private set
            {
                physicalMax = value.Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            }

        }

        private int[] digitallMin = new int[0];
        public string[] DigitallMin
        {
            get
            {
                return digitallMin.Select(x => x.ToString()).ToArray();
            }
            private set
            {
                digitallMin = value.Select(s => int.Parse(s)).ToArray();
            }

        }

        private int[] digitalMax= new int[0];
        public string[] DigitalMax
        {
            get
            {
                return digitalMax.Select(x => x.ToString()).ToArray();
            }
            private set
            {
                digitalMax = value.Select(s => int.Parse(s)).ToArray();
            }

        }

        public string[] Prefiltering { get ; private set; }

        private int[] numberSamplesDataRecord= new int[0];
        /// <summary>
        /// Numero de muestra en un registro de datos
        /// para una señal determinada.
        /// El nummero total de muestras de una señal sería,
        /// Nº de muestras de un DataRecord * Nº DataRecord
        /// </summary>
        public string[] NumberSamplesDataRecord
        {
            get
            {
                return numberSamplesDataRecord.Select(x => x.ToString()).ToArray();
            }
            private set
            {
                numberSamplesDataRecord = value.Select(s => int.Parse(s)).ToArray();
            }

        }

        public string[] ReservedSignal { get; private set;}

        /// <summary>
        /// Convierte la informacion ASCII del registro de cabecera de
        /// fichero EDF en información necesaria para la Clase Record
        /// </summary>
        /// <returns></returns>
        public Record toRecord(bool readSamples=false)
        {
            string recordName = Path.GetFileNameWithoutExtension(fileEDF.Name);
            Record record = new Record(recordName);

            string s = StartDateRecording + StartTimeRecording;
            DateTime dt;
            if (!DateTime.TryParseExact(s, "dd.MM.yyHH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                if (!DateTime.TryParseExact(StartDateRecording, "dd.MM.yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    if (!DateTime.TryParseExact(StartTimeRecording, "HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        dt = new DateTime();

            if (dt != new DateTime())
                record.StartDateTime = dt;

            
          
            record.NumberOfSignals = numberSignals;
            record.NumberOfFrames = numberDataRecords;
           
            //Frecuencia de frame           
            record.FrameFrequency = 1 / durationDataRecord;
            record.FrameInterval = durationDataRecord;
            record.CounterFrequency = 1 / durationDataRecord;
            //La señal con el maxima cantidad de muestras señal    
            record.NumberOfSamplesPerSignal = numberSamplesDataRecord.Max()* numberDataRecords;            
            //Máxima frecuencia de muestreo de las señales    
            record.SamplingFrequencyPerSignal = numberSamplesDataRecord.Max() / durationDataRecord;
            record.DurationInSeconds = durationDataRecord * numberDataRecords;
            

            record.Info.Add("Version EDF:" + Version);
            record.Info.Add("IdPatient:" + IdPatient);
            record.Info.Add("IdRecording:" + IdRecording);
            record.Info.Add("Reserved:" + Reserved);
            record.Info.Add("Transducer:" + String.Join(";", Transducer));
            record.Info.Add("PhysicalMin:" + String.Join(";", PhysicalMin));
            record.Info.Add("PhysicalMax:" + String.Join(";", PhysicalMax));
            record.Info.Add("DigitallMin:" + String.Join(";", DigitallMin));
            record.Info.Add("DigitalMax:" + String.Join(";", DigitalMax));
            record.Info.Add("Prefiltering:" + String.Join(";", Prefiltering));
            record.Info.Add("ReservedSignal:" + String.Join(";", ReservedSignal));

            Signal signal = new Signal() ;
            for (int i = 0; i < record.NumberOfSignals; i++)
            {
                signal = new Signal();
                signal.FileName = Path.GetFileName(fileEDF.Name);
                signal.Number = i;
                signal.ByteOffset = bytesHeaderRecord;
                signal.AdcZero= (1 +digitalMax[i] + digitallMin[i]) / 2;
                signal.InitValue = signal.AdcZero;

                int adcRange = digitalMax[i] -digitallMin[i];

                int resolution;
                for (resolution = 1; adcRange > 1; resolution++)
                    adcRange = adcRange / 2;

                signal.AdcResolution = resolution;


                if(physicalMax[i]!= physicalMin[i])
                {
                    signal.Gain = (digitalMax[i] - digitallMin[i])/ (physicalMax[i] - physicalMin[i]);
                    var bLine =  digitalMax[i] - (signal.Gain * physicalMax[i]);
                    signal.Baseline =(int) ((bLine >= 0.0) ? bLine + 0.5 : bLine - 0.5);
                }
                             
                signal.Units = PhysicalDim[i];
                signal.Description = Label[i];
                
                signal.Format = (Version=="0")? SignalStorageFormat.Sf16Bit: SignalStorageFormat.Sf24Bit;
                signal.SamplesPerFrame = numberSamplesDataRecord[i];
                
                signal.NumberOfSamples = numberSamplesDataRecord[i] * numberDataRecords;
                signal.SamplingFrequency = numberSamplesDataRecord[i] / durationDataRecord;              
                record.Signals.Add(signal);
            }

            if (readSamples)
                ReadAllSamples(record);

            return record;
        }

        /// <summary>
        /// Lee todas las muestras del fichero .edf
        /// </summary>
        /// <param name="record"> Con las señales rellenadas de sus muestras.</param>
        private void ReadAllSamples(Record record)
        {
            fileEDF.Position = bytesHeaderRecord;
           
            for (int d = 0; d < numberDataRecords; d++)
            {
                for (int s = 0; s < numberSignals; s++)
                {
                    for(int m=0;m< numberSamplesDataRecord[s];m++)
                    {
                        int muestra = 0;
                        if (Version == "0")
                            muestra = fileEDF.ReadF16bits();
                        else
                            muestra = fileEDF.ReadF24bits();

                        record.Signals[s].AddSample(muestra);
                    }
                }
            }

            
        }

      
    }
}
