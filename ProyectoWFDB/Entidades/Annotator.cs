/*_______________________________________________________________________________
 * wfdbcsharpwrapper:
 * ------------------
 * A .NET library that encapsulates the wfdb library.
 * Copyright Oualid BOUTEMINE, 2009-2016
 * Contact: boutemine.walid@hotmail.com
 * Project web page: https://github.com/oualidb/WfdbCsharpWrapper
 * Code Documentation : From WFDB Programmer's Guide BY George B. Moody
 * wfdb: 
 * -----
 * a library for reading and writing annotated waveforms (time series data)
 * Copyright (C) 1999 George B. Moody

 * This library is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Library General Public License as published by the Free
 * Software Foundation; either version 2 of the License, or (at your option) any
 * later version.

 * This library is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
 * PARTICULAR PURPOSE.  See the GNU Library General Public License for more
 * details.

 * You should have received a copy of the GNU Library General Public License along
 * with this library; if not, write to the Free Software Foundation, Inc., 59
 * Temple Place - Suite 330, Boston, MA 02111-1307, USA.

 * You may contact the author by e-mail (george@mit.edu) or postal mail
 * (MIT Room E25-505A, Cambridge, MA 02139 USA).  For updates to this software,
 * please visit PhysioNet (http://www.physionet.org/).
 * _______________________________________________________________________________
 */

using ProyectoWFDB.Enumeraciones;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ProyectoWFDB.Entidades
{
    /// <summary>
    /// 
    /// </summary>
    public class Annotator : IEquatable<Annotator>, IDisposable
    {
        #region Properties
        private string name;
        /// <summary>
        /// Gets or sets the annotator's name.
        /// <remarks>
        /// The name ‘atr’ is reserved for a reference annotation
        /// file supplied by the creator of the database record to document its contents as
        /// accurately and thoroughly as possible. You may use other annotator names to
        /// identify annotation files that you create; unless there are compelling reasons not
        /// to do so, follow the convention that the annotator name is the name of the file’s
        /// creator (a program or a person). To avoid confusion, do not use ‘dat’, ‘datan’,
        /// ‘dn’, or ‘hea’ (all of which are commonly used as parts of WFDB file names) as
        /// annotator names. The special name ‘-’ refers to the standard input or output.
        /// Other annotator names may contain upper- or lower-case letters, digits, and
        /// underscores. Annotation files are normally created in the current directory and
        /// found in any of the directories in the database path.
        /// </remarks>
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private Stat stat;
        /// <summary>
        /// Gets or sets the file type/access code. 
        /// <remarks>
        /// Usually, Stat is either <see cref="Stat.Read"/> or 
        /// <see cref="Stat.Write"/> to specify standard (“WFDB format”) annotation files to be read by getann
        /// or to be written by putann. Both MIT DB and AHA DB annotation files can
        /// be (and generally are) stored in WFDB format. An AHA-format annotation file
        /// can be read by getann or written by putann if the Stat field is set to <see cref="Stat.AhaRead"/>
        /// or <see cref="Stat.AhaWrite"/> before calling annopen or wfdbinit.
        /// </remarks>
        /// </summary>
        public Stat Stat
        {
            get
            {
                return stat;
            }
            set
            {
                stat = value;
            }
        }
        /// <summary>
        /// Gets or sets an unsigned integer type used to represent annotator numbers.
        /// </summary>
        public int Number { get; set; }
      
  
        private List<Annotation> annotations= new List<Annotation>();
        public List<Annotation> Annotations
        {
            get { return annotations; }
            
        }

        #endregion

        #region Methods

        public static Annotator ReadAnnotatorFile(FileStream annotatorFile)
        {
            string annotatorName = Path.GetExtension(annotatorFile.Name);
            Annotator annotator = new Annotator { Name = annotatorName };

            annotatorFile.Position = 0;
            byte[] bytes = new byte[2];
            int numBytesToRead = annotatorFile.Read(bytes, 0, bytes.Length);
            annotatorFile.Position = 0;
            AnnotationCode a = AnnotationCode.MapAhaToMit((char)bytes[1]);
            Annotation annotation = new Annotation();
            bool isMIT = (a == AnnotationCode.NotQrs || bytes[0] != 0 || bytes[1] == '[' || bytes[1] == ']');

            if (isMIT)
            {
                //Los 6 bit más signifactivo del MSB especifican el codigo de la anotación
                annotator.Stat = Stat.Read;
                int tiempo = 0;

                while (true)
                {
                    bytes = new byte[2];
                    numBytesToRead = annotatorFile.Read(bytes, 0, bytes.Length);
                    if (numBytesToRead == 0)
                        break;

                    a = (AnnotationCode)(bytes[1] >> 2);

                    int i = ((bytes[1] & 0b0000_0011) << 8) | bytes[0];
                    switch (a.Value)
                    {
                        //SKIP
                        case 59:
                            if (i == 0)
                                tiempo = (int)(annotatorFile.Read4BytesPDP_11());
                            break;

                        //NUM
                        case 60:
                            annotation.AnnotatorNumber = (byte)(i);
                            break;

                        //SUB
                        case 61:
                            annotation.SubType = (byte)(i);
                            break;

                        //CHN
                        case 62:
                            annotation.ChannelNumber = (byte)(i);
                            break;

                        //AUX
                        case 63:
                            bytes = new byte[i + 1];
                            numBytesToRead = annotatorFile.Read(bytes, 0, bytes.Length);
                            annotation.Aux = Encoding.ASCII.GetString(bytes);
                            break;
                        default:
                            annotator.Annotations.Add(annotation);
                            annotation = new Annotation();
                            annotation.Type = a;
                            annotation.AnnotatorNumber = annotator.Annotations.LastOrDefault().AnnotatorNumber;
                            annotation.ChannelNumber = annotator.Annotations.LastOrDefault().ChannelNumber;
                            int timeLast = annotator.Annotations.LastOrDefault().Time;
                            annotation.Time = timeLast + i + tiempo;
                            tiempo = 0;
                            break;
                    }
                }

                annotator.Annotations.RemoveAt(0);
            }
            else
            {// AHA FORMAT
                //Bytes para leer la información auxiliar de la anotación AHA
                bytes = new byte[6];
                annotator.Stat = Stat.AhaRead;
                while (true)
                {
                    int y = annotatorFile.ReadByte();
                    if (y == -1) break; //1ºB ignorar
                    annotation = new Annotation();
                    char ahaCode = (char)annotatorFile.ReadByte();// 2º B
                    annotation.Type = AnnotationCode.MapAhaToMit(ahaCode);
                    annotation.Time = (int)annotatorFile.Read4BytesPDP_11(); // 3-6ºB
                    var numSerieAnotacion = annotatorFile.Read2BytesPDP_11();// 7,8ºB
                    annotation.AnnotatorNumber = (byte)numSerieAnotacion;
                    annotation.SubType = (AnnotationCode)annotatorFile.ReadByte(); // 9ºB

                    if (ahaCode == 'U' && annotation.SubType == 0)
                        annotation.SubType = -1; /* ilegible (ruido subtype -1) */

                    annotation.ChannelNumber = (byte)annotatorFile.ReadByte(); //10ºB                  
                    annotatorFile.Read(bytes, 0, bytes.Length);
                    annotation.Aux = Encoding.ASCII.GetString(bytes);// 11-16ºB
                    annotator.Annotations.Add(annotation);
                }
            }


            return annotator;
        }
     
        public bool Equals(Annotator other)
        {
            if (other == null)
                throw new ArgumentNullException("other", "Parameter should not be null.");
            return other.name.Equals(name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Annotator)) return false;
            return Equals((Annotator)obj);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public void Dispose()
        {
            annotations.Clear();
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion

        #region Operator overloads
        public static bool operator ==(Annotator left, Annotator right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Annotator left, Annotator right)
        {
            return !left.Equals(right);
        }

        public static implicit operator int(Annotator ann)
        {
            return ann.Number;
        }
        #endregion


      
    }
}
