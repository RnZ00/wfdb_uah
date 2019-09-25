using ProyectoWFDB.Entidades;
using ProyectoWFDB.Enumeraciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace ProyectoWFDB
{
    public static class ManagerWFDB
    {
        ///<summary>
        ///
        ///Open record file of type.
        ///
        public static Stream Wfdb_Open(string recordName, string type, Stat mode)
        {
            //Comprobar si es e/s estandaR  
            if (type == "-" || (type == "hea" && recordName == "-"))
            {
                Stream stream = null;

                switch (mode)
                {
                    case Stat.Read:
                        stream = Console.OpenStandardInput(); ;
                        break;
                    case Stat.Write:
                        stream = Console.OpenStandardOutput();
                        break;
                    case Stat.AhaRead:
                        break;
                    case Stat.AhaWrite:
                        break;
                    default:
                        break;
                }
                return stream;
            }
            string recordPath = GetFileRecordPath(recordName, type);

            //Abrir el fichero del registro
            FileStream recordFile = null;
            switch (mode)
            {
                case Stat.Read:
                    recordFile = File.OpenRead(recordPath);
                    break;
                case Stat.Write:
                    recordFile = File.OpenWrite(recordPath);
                    break;
                case Stat.AhaRead:
                    break;
                case Stat.AhaWrite:
                    break;
                default:
                    break;
            }

            return recordFile;
        }

        ///<summary>
        ///
        ///Search a file record in WFDBPATH and return the full path
        ///
        private static string GetFileRecordPath(string recordName, string type)
        {

            //Limpiar el nombre de registro si viene con ruta y con extension
            recordName = Path.GetFileNameWithoutExtension(recordName);
            //Limpiar el tipo o extensión del fichero
            type = type.Replace(".", "");

            //Añadir al nombre del registro limpio la extensión
            string recordWithType = recordName + "." + type;

            string recordPath = string.Empty;
            //Buscar el registro en la rutas de la variable de entorno de WFDB
            foreach (var wPath in WfdbPath.Split(';'))
            {
                if (Directory.Exists(wPath))
                    recordPath = Directory.GetFiles(wPath, recordWithType, SearchOption.AllDirectories).FirstOrDefault();
                if (!string.IsNullOrEmpty(recordPath))
                    break;
            }

            if (string.IsNullOrEmpty(recordPath))
            {
                throw new Exception(String.Format("{0} was not found in {1}", recordWithType, WfdbPath));
            }

            return recordPath;
        }

        /// <summary>
        /// Lee un fichero en formato EDF de un registro determinado 
        /// </summary>
        /// <param name="recordName">Nombre del registro, que debe coincidir con el nombre del archivo edf con o sin extensión .EDF</param>
        /// <param name="readSignals">Por defecto está a true</param>
        /// <returns>Devuelve un Record con las propiedades establecidas y las muestra llenas si readSignals es true</returns>
        public static Record ReadEDF(string recordName, bool readSignals=true)
        {
            string fullFileName = GetFileRecordPath(recordName, "edf");

            var fs=(FileStream)Wfdb_Open(recordName, "edf", Stat.Read);

            InfoRecordEDF recordEDF = new InfoRecordEDF(fs);
            Record record=recordEDF.toRecord(readSignals);

            return record;
        }

        ///<summary>
        ///
        ///Leer fichero de cabecera de un registro.
        ///
        public static Record ReadHeader(string recordName, bool isEDF = false)
        {
            Record registro;

            if (isEDF)
                registro= ReadEDF(recordName, false);
            else
            {
                //Abre el fichero head
                var headerFile = Wfdb_Open(recordName, "hea", Stat.Read);
                registro = Record.ReadHeaderFile(recordName, headerFile);
            }

        
            return registro;

        }

        /// <summary>
        /// Lee las anotaciones de un fichero de anotación.
        /// </summary>
        /// <param name="recordName">Nombre del registro</param>
        /// <param name="annotatorName">Nombre del anotador o extensión del registro (atr)</param>
        /// se debe especificar en esta variable
        /// <returns>Devuelve una instancia de un Annotador con la lista de anotaciones leidas del fichero
        /// de anotaciones.</returns>
        public static Annotator ReadAnnotator(string recordName,string annotatorName)
        {          
            FileStream annotatorFile = (FileStream)Wfdb_Open(recordName, annotatorName,Stat.Read);
            Annotator annotator = Annotator.ReadAnnotatorFile(annotatorFile);
            return annotator;
        }

        ///<summary>
        ///Lee los ficheros de señales de un registro
        ///Y llena las muestras de cada señal del registro
        ///</summary>
        public static Record ReadSignals(string recordName)
        {
            Record registro = ReadHeader(recordName);
            List<string> filesSignal = registro.Signals.Select(s => s.FileName).Distinct().ToList();

            //Por cada fichero de señal del registro
            foreach (string file in filesSignal)
            {
                //Señales con el mismo nombre del fichero de datos
                var signalsGroup = registro.Signals.Where(s => s.FileName == file).ToList();
                
                //Abrir el fichero de datos
                FileStream fileSignal = (FileStream)Wfdb_Open(
                    Path.GetFileNameWithoutExtension(file),
                    Path.GetExtension(file),
                    Stat.Read);
                //Lee el fichero de señal y llena las muestras
                Signal.ReadSignalFile(fileSignal, signalsGroup);
            
            }

            return registro;
        }

        /// <summary>
        /// Exporta en un fichero CSV las muestras de las todas las señales del registro
        /// comprendida entre el segundo inicial y segundo final.
        /// Guarda el fichero CSV en la ruta del fichero .hea o .edf
        /// </summary>
        /// <param name="recordName"></param>
        /// <param name="secondInit"></param>
        /// <param name="secondEnd"></param>
        /// <param name="isEDF"> Por defecto falso</param>
        /// <returns></returns>
        public static string ExportSamplesCSV(string recordName,double secondInit=0, double secondEnd=-1,bool isEDF=false)
        {
            
            Record registro = (isEDF)? ReadEDF(recordName,true):ReadSignals(recordName);

            secondInit = (secondInit < 0 || secondInit > secondEnd) ? 0 : secondInit;
            secondEnd = (secondEnd < 0)? registro.DurationInSeconds: secondEnd;
            secondEnd = Math.Min(secondEnd, registro.DurationInSeconds);
            int idSampleInit = registro.SecondToIdSample(secondInit);
            int idSampleEnd = registro.SecondToIdSample(secondEnd);

            string extension = (isEDF) ? "edf" : "hea";
            string pathRecord = GetFileRecordPath(recordName, extension);
            string directoryRecord = Path.GetDirectoryName(pathRecord);
            string fileSample = Path.Combine(directoryRecord, recordName+".csv");

           
            List<string> AllLines = new List<string>();

            List<string> line = new List<string>();
            line.Add("Tiempo");          
            line.Add("Hora");
            line.Add("Fecha");

            foreach (var signal in registro.Signals)
            {
                string signalName= String.Format("{0}({1})",signal.Description,signal.Units);
                line.Add(signalName);
            }
            AllLines.Add(String.Join(";", line));
            line.Clear();
            DateTime startDateTime = (registro.StartDateTime.HasValue) ? registro.StartDateTime.Value : (new DateTime());
            //for (int i = idSampleInit; i < idSampleEnd; i++)
            //{
            //    double secondSample = registro.IdSampleToSecond(i);
            //    TimeSpan ts = TimeSpan.FromSeconds(secondSample);

            //    DateTime dt=startDateTime.AddSeconds(secondSample);

            //    line.Add(ts.ToString(@"hh\:mm\:ss\.fff"));
            //    line.Add(dt.ToString(@"hh\:mm\:ss"));
            //    line.Add(dt.ToString(@"dd/MM/yyyy"));
               
                 
            //    foreach (var signal in registro.Signals)
            //    {
            //        double aduToUnits=0;
            //        if (i < signal.NumberOfSamples)
            //        aduToUnits = signal.SampleToPhysicalUnits(i);
            //        line.Add(aduToUnits.ToString());
            //    }
            //    AllLines.Add(String.Join(";", line));
            //    line.Clear();

            //}
            for (double secondSample = secondInit;
                secondSample <= secondEnd;
                secondSample= secondSample+registro.SamplingIntervalPerSignal)
            {
                
                TimeSpan ts = TimeSpan.FromSeconds(secondSample);

                DateTime dt = startDateTime.AddSeconds(secondSample);

                line.Add(ts.ToString(@"hh\:mm\:ss\.fff"));
                line.Add(dt.ToString("HH:mm:ss"));
                line.Add(dt.ToString(@"dd/MM/yyyy"));


                foreach (var signal in registro.Signals)
                {
                    int numberSample = signal.SecondToIdSample(secondSample);
                    double aduToUnits = 0;                    
                    aduToUnits = signal.SampleToPhysicalUnits(numberSample);
                    line.Add(aduToUnits.ToString());
                }
                AllLines.Add(String.Join(";", line));
                line.Clear();

            }
            if (File.Exists(fileSample)) File.Delete(fileSample);
            File.WriteAllLines(fileSample, AllLines);
            return fileSample;
        }

        /// <summary>
        /// Exporta la clase Record en un JSON
        /// La información volcada se obtiene del fichero .hea o del .edf
        /// Guarda el JSON en la ruta del fichero de cabecera.
        /// </summary>
        /// <param name="recordName"></param>
        /// <param name="isEDF"></param>
        /// <returns></returns>
        public static string ExportRecordJSON(string recordName,bool isEDF=false)
        {
            Record registro = (isEDF)? ReadEDF(recordName,false):ReadHeader(recordName);
            string extension = (isEDF) ? "edf" : "hea";
            var pathRecord = GetFileRecordPath(recordName, extension);
            var jsonPath = Path.Combine(Path.GetDirectoryName(pathRecord), recordName + ".json");

            var json = JsonConvert.SerializeObject(registro, Formatting.Indented);
            File.WriteAllText(jsonPath, json);

            return jsonPath;
        }

        /// <summary>
        /// Exporta en un CSV las anotaciones de un registro.
        /// Guarda el CSV en la ruta  del fichero anotador
        /// </summary>
        /// <param name="recordName"></param>
        /// <param name="annotatorName"></param>
        /// <param name="readHeader"></param>
        /// <returns></returns>
        public static string ExportAnnotationsCSV(string recordName, string annotatorName,bool readHeader=false)
        {
            Record registro = (readHeader)?ReadHeader(recordName):(new Record(recordName));          
            string pathAnnotator = GetFileRecordPath(recordName, annotatorName);
            string directoryAnnotator= Path.GetDirectoryName(pathAnnotator);
            string fileAnnotation = Path.Combine(directoryAnnotator, recordName +"-" +annotatorName+".csv");

            Annotator annotator=ManagerWFDB.ReadAnnotator(recordName, annotatorName);

            List<string> AllLines = new List<string>();

            List<string> line = new List<string>();
            line.Add("Tiempo");
            line.Add("Hora");
            line.Add("Fecha");
            line.Add("Muestra");
            line.Add("Tipo");
            line.Add("Subtipo");
            line.Add("Canal");
            line.Add("Anotador");
            line.Add("Informacion");
            AllLines.Add(String.Join(";", line));
            line.Clear();
            DateTime startDateTime = (registro.StartDateTime.HasValue) ? registro.StartDateTime.Value : (new DateTime());
            for (int i = 0; i < annotator.Annotations.Count; i++)
            {
                Annotation ann = annotator.Annotations[i]; 
                double secondSample = readHeader? registro.IdSampleToSecond(ann.Time):0;
                TimeSpan ts = TimeSpan.FromSeconds(secondSample);

                DateTime dt = startDateTime.AddSeconds(secondSample);

                line.Add(ts.ToString(@"hh\:mm\:ss\.fff"));
                line.Add(dt.ToString("HH:mm:ss"));
                line.Add(dt.ToString(@"dd/MM/yyyy"));
                line.Add(ann.Time.ToString());
                line.Add(ann.Type.ToString());
                line.Add(ann.SubType.ToString());
                line.Add(ann.ChannelNumber.ToString());
                line.Add(ann.AnnotatorNumber.ToString());
                line.Add(ann.Aux.ToString());

                AllLines.Add(String.Join(";", line));
                line.Clear();

            }
            if (File.Exists(fileAnnotation)) File.Delete(fileAnnotation);
            File.WriteAllLines(fileAnnotation, AllLines);
            return fileAnnotation;
        }

        private static string wfdbPath;
        ///<summary>
        ///
        ///Return the path of WFDB from la EnviromentvVariable WFDB or DEFWFDB
        ///
        public static string WfdbPath
        {
            get
            {
               if(wfdbPath == null)
                {
                    wfdbPath = Environment.GetEnvironmentVariable("WFDB");
                    if(wfdbPath == null)
                    {
                        wfdbPath = ConfigurationManager.AppSettings["DEFWFDB"];

                    }
                    return wfdbPath;
                }
                else
                {
                    return wfdbPath;
                }
            }
            set
            {
                wfdbPath = value;
            }
        }

        /// <summary>
        /// Reinicia el valor por defecto de WFDBPATH
        /// </summary>
        public static void ResetWfdbPath()
        {
            wfdbPath = Environment.GetEnvironmentVariable("WFDB");
            if (wfdbPath == null)
            {
                wfdbPath = ConfigurationManager.AppSettings["DEFWFDB"];

            }
        }
    }
}
