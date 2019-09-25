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
using System.Linq;
using System.Runtime.InteropServices;

namespace ProyectoWFDB.Entidades
{
    /// <summary>
    /// Annotation Codes.
    /// <remarks>
    /// The annotation codes are the predefined values of the <see cref="Annotation.Type"/>
    /// field. Other values in the range of 1 to <see cref="AnnotationCode.ACMax"/> are legal but do not have preassigned meanings. The constant
    /// <see cref="AnnotationCode.NotQrs"/>, is not a legal value for <see cref="Annotation.Type"/>, but is a
    /// possible output of macros implemented in this class.
    /// </remarks>
    /// </summary>
    public class AnnotationCode : IComparable<AnnotationCode>, IEquatable<AnnotationCode>
    {
        #region ctor
        /// <summary>
        /// Creates a new instance from <see cref="AnnotationCode"/>
        /// </summary>
        /// <param name="value">Annotation code's value.</param>
        public AnnotationCode(byte value)
        {
            this.value = value;
        }

        static AnnotationCode()
        {
            annotationCodes = new List<AnnotationCode>();
            annotationCodes.Add(AnnotationCode.Aberr);
            annotationCodes.Add(AnnotationCode.ACMax);
            annotationCodes.Add(AnnotationCode.Aesc);
            annotationCodes.Add(AnnotationCode.Apc);
            annotationCodes.Add(AnnotationCode.Arfct);
            annotationCodes.Add(AnnotationCode.Bbb);
            annotationCodes.Add(AnnotationCode.Diastole);
            annotationCodes.Add(AnnotationCode.FLWav);
            annotationCodes.Add(AnnotationCode.Fusion);
#pragma warning disable CS0612 // El tipo o el miembro están obsoletos
            annotationCodes.Add(AnnotationCode.JPt);
#pragma warning restore CS0612 // El tipo o el miembro están obsoletos
            annotationCodes.Add(AnnotationCode.Lbbb);
            annotationCodes.Add(AnnotationCode.Learn);
            annotationCodes.Add(AnnotationCode.Link);
            annotationCodes.Add(AnnotationCode.Measure);
            annotationCodes.Add(AnnotationCode.NApc);
            annotationCodes.Add(AnnotationCode.Nesc);
            annotationCodes.Add(AnnotationCode.Noise);
            annotationCodes.Add(AnnotationCode.Normal);
            annotationCodes.Add(AnnotationCode.Note);
            annotationCodes.Add(AnnotationCode.NotQrs);
            annotationCodes.Add(AnnotationCode.Npc);
            annotationCodes.Add(AnnotationCode.Pace);
            annotationCodes.Add(AnnotationCode.PaceSP);
            annotationCodes.Add(AnnotationCode.Pfus);
#pragma warning disable CS0612 // El tipo o el miembro están obsoletos
            annotationCodes.Add(AnnotationCode.PQ);
#pragma warning restore CS0612 // El tipo o el miembro están obsoletos
            annotationCodes.Add(AnnotationCode.Pvc);
            annotationCodes.Add(AnnotationCode.PWave);
            annotationCodes.Add(AnnotationCode.Rbbb);
            annotationCodes.Add(AnnotationCode.Reserved42);
            annotationCodes.Add(AnnotationCode.Reserved43);
            annotationCodes.Add(AnnotationCode.Reserved44);
            annotationCodes.Add(AnnotationCode.Reserved45);
            annotationCodes.Add(AnnotationCode.Reserved46);
            annotationCodes.Add(AnnotationCode.Reserved47);
            annotationCodes.Add(AnnotationCode.Reserved48);
            annotationCodes.Add(AnnotationCode.Rhythm);
            annotationCodes.Add(AnnotationCode.ROnT);
            annotationCodes.Add(AnnotationCode.STCh);
            annotationCodes.Add(AnnotationCode.Svesc);
            annotationCodes.Add(AnnotationCode.Svpb);
            annotationCodes.Add(AnnotationCode.Systole);
            annotationCodes.Add(AnnotationCode.TCh);
            annotationCodes.Add(AnnotationCode.TWave);
            annotationCodes.Add(AnnotationCode.Unknown);
            annotationCodes.Add(AnnotationCode.UWave);
            annotationCodes.Add(AnnotationCode.Vesc);
            annotationCodes.Add(AnnotationCode.VfOff);
            annotationCodes.Add(AnnotationCode.VfOn);
            annotationCodes.Add(AnnotationCode.WFOff);
            annotationCodes.Add(AnnotationCode.WFOn);

            annotationCodes.Sort();
        }

        #endregion

        #region Methods
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        private static readonly AnnotationCode[] mapAha2Mit = {
        Vesc,   Fusion, NotQrs, NotQrs, NotQrs,		/* 'E' - 'I' */
	    NotQrs, NotQrs, NotQrs, NotQrs, Normal,		/* 'J' - 'N' */
	    Note,   Pace,   Unknown,ROnT,   NotQrs,		/* 'O' - 'S' */
	    NotQrs, Noise,  Pvc,    NotQrs, NotQrs,		/* 'T' - 'X' */
	    NotQrs, NotQrs, VfOn,   NotQrs, VfOff		/* 'Y' - ']' */
        };
        /// <summary>
        /// Maps an AHA annotation code into an MIT annotation code (one of the
        /// set {<see cref="AnnotationCode.Normal"/>, <see cref="AnnotationCode.Pvc"/>, <see cref="AnnotationCode.Fusion"/>, <see cref="AnnotationCode.ROnT"/>, <see cref="AnnotationCode.Vesc"/>, 
        /// <see cref="AnnotationCode.Pace"/>, <see cref="AnnotationCode.Unknown"/>, <see cref="AnnotationCode.VfOn"/>, <see cref="AnnotationCode.VfOff"/>, <see cref="AnnotationCode.Noise"/>,
        /// <see cref="AnnotationCode.Note"/>}), or <see cref="AnnotationCode.NotQrs"/>
        /// </summary>
        /// <param name="ahaCode">AHA Annotation code.</param>
        /// <returns>Corresponding MIT Code</returns>
        public static AnnotationCode MapAhaToMit(char ahaCode)
        {
            //#define ammap(A)   (('D' < (wfdb_mt = (A)) && wfdb_mt <= ']') ? \
            //wfdb_ammp[wfdb_mt - 'E'] : NOTQRS)
            AnnotationCode mitCode = NotQrs;
            if ('D' < ahaCode && ahaCode <= ']')
                mitCode = mapAha2Mit[ahaCode - 'E'];

            return mitCode;
            //return PInvoke.wfdb_ammap(ahaCode);
        }




        private static readonly char[] mapMit2Aha = {
            'O',    'N',    'N',    'N',    'N',		/* 0 - 4 */
	        'V',    'F',    'N',    'N',    'N',		/* 5 - 9 */
	        'E',    'N',    'P',    'Q',    'U',		/* 10 - 14 */
	        'O',    'O',    'O',    'O',    'O',		/* 15 - 19 */
	        'O',    'O',    'O',    'O',    'O',		/* 20 - 24 */
	        'N',    'O',    'O',    'O',    'O',		/* 25 - 29 */
	        'Q',    'O',    '[',    ']',    'N',		/* 30 - 34 */
	        'N',    'O',    'O',    'N',    'O',		/* 35 - 39 */
	        'O',    'R',    'O',    'O',    'O',		/* 40 - 44 */
	        'O',    'O',    'O',    'O',    'O'			/* 45 - 49 */
        };

        /// <summary>
        ///  Maps this MIT annotation code into an AHA annotation code.
        /// </summary>
        /// <param name="mitSubCode">
        /// MIT annotation sub code
        /// <remarks>
        /// This parameter is significant only if mitCode is <see cref="AnnotationCode.Noise"/>)
        /// This overloaded version is used when mitSubCode is -1.
        /// </remarks>
        /// </param>
        /// <returns>The corresponding AHA annotation code</returns>
        public char ToAha(int subCode)
        {
            char ahaCode = 'O';
            if (IsAnnotation)
            {
                ahaCode = mapMit2Aha[value];
                if (ahaCode == 'U' && subCode != -1)
                    ahaCode = 'O';
            }

            return ahaCode;      
        }

        /* mnemonic strings for each code */
        private static string[] mnemonicStrings = {
            " ",    "N",    "L",    "R",    "a",		/* 0 - 4 */
	        "V",    "F",    "J",    "A",    "S",		/* 5 - 9 */
	        "E",    "j",    "/",    "Q",    "~",		/* 10 - 14 */
	        "[15]", "|",    "[17]", "s",    "T",		/* 15 - 19 */
	        "*",    "D",    "\"",   "=",    "p",		/* 20 - 24 */
	        "B",    "^",    "t",    "+",    "u",		/* 25 - 29 */
	        "?",    "!",    "[",    "]",    "e",		/* 30 - 34 */
	        "n",    "@",    "x",    "f",    "(",		/* 35 - 39 */
	        ")",    "r",    "[42]", "[43]", "[44]",		/* 40 - 44 */
	        "[45]", "[46]", "[47]", "[48]", "[49]"		/* 45 - 49 */
        };
        /// <summary>
        /// Converts a string into a valid annotation code if possible.
        /// </summary>
        /// <param name="codeString">Annotation code's string.</param>
        /// <returns>Annotation Code.</returns>
        /// <remarks>
        /// Illegal strings are translated into <see cref="AnnotationCode.NotQrs"/>. Input strings
        /// for Parse and ParseEcgString should match those returned by <see cref="String"/> and <see cref="EcgString"/> respectively.
        /// </remarks>
        public static AnnotationCode Parse(string codeString)
        {
            if (codeString == null)codeString ="";
            int index=mnemonicStrings.ToList().IndexOf(codeString);          
            return index==-1? NotQrs : new AnnotationCode((byte)index);
        }

        /// <summary>
        /// Converts a string into a valid annotation code if possible.
        /// </summary>
        /// <param name="ecgString">Annotation code's string.</param>
        /// <returns>Annotation Code.</returns>
        /// <remarks>
        /// Illegal strings are translated into <see cref="AnnotationCode.NotQrs"/>. Input strings
        /// for Parse and ParseEcgString should match those returned by <see cref="AnnotationCode.String"/> and <see cref="EcgString"/> respectively.
        /// </remarks>
        public static AnnotationCode ParseEcgString(string ecgString)
        {
            if (ecgString == null) ecgString = "";
            int index = ecgMnemonicStrings.ToList().IndexOf(ecgString);
            return index == -1 ? NotQrs : new AnnotationCode((byte)index);
        }

        public override string ToString()
        {
            return String;
        }

        public bool Equals(AnnotationCode obj)
        {
            return Value.Equals(obj.Value);
        }

        public override bool Equals(object obj)
        {
            var annotationCode = obj as AnnotationCode;
            if (annotationCode == null)
                return false;

            return Equals(annotationCode);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether or not a given code is a legal annotation code.
        /// </summary>
        /// <returns>True if code is a legal annotation code, false otherwise.</returns>
        public bool IsAnnotation
        {
            get
            {
                return 0 < value && value <= ACMax;
                //return PInvoke.wfdb_isann(this);
            }
        }

        private static List<AnnotationCode> annotationCodes;
        /// <summary>
        /// Gets the list of supported annotation codes.
        /// </summary>
        public static List<AnnotationCode> AnnotationCodes
        {
            get { return annotationCodes; }
        }

        private byte value;
        /// <summary>
        /// Gets the integer value of this annotation code.
        /// </summary>
        public byte Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Converts the specified annotation code into a string.
        /// </summary>
        public string String
        {
            get
            {
                if(0 <= Value && Value <=ACMax)
                {
                    return mnemonicStrings[Value];
                }
                return "[" + Value.ToString() + "]";
            }
            set
            {
                if (0 <= Value && Value <= ACMax)
                    mnemonicStrings[Value] = value;
                else
                    throw new ArgumentException("Illegal annotation code:" + Value);
            }
        }


        /* ECG mnemonic strings for each code */
        private static string[] ecgMnemonicStrings = {
            " ",    "N",    "L",    "R",    "a",		/* 0 - 4 */
	        "V",    "F",    "J",    "A",    "S",		/* 5 - 9 */
	        "E",    "j",    "/",    "Q",    "~",		/* 10 - 14 */
	        "[15]", "|",    "[17]", "s",    "T",		/* 15 - 19 */
	        "*",    "D",    "\"",   "=",    "p",		/* 20 - 24 */
	        "B",    "^",    "t",    "+",    "u",		/* 25 - 29 */
	        "?",    "!",    "[",    "]",    "e",		/* 30 - 34 */
	        "n",    "@",    "x",    "f",    "(",		/* 35 - 39 */
	        ")",    "r",    "[42]", "[43]", "[44]",		/* 40 - 44 */
	        "[45]", "[46]", "[47]", "[48]", "[49]"		/* 45 - 49 */
        };
        /// <summary>
        /// Gets or sets the string representation of this annotation code.
        /// </summary>
        /// <remarks>
        /// The strings returned by EcgString are usually
        /// the same as those returned by <see cref="AnnotationCode.String"/>, but they can be modified only using this property's setter.
        /// </remarks>
        public string EcgString
        {
            get
            {
                if (0 <= Value && Value <= ACMax)
                {
                    return ecgMnemonicStrings[Value];
                }
                return "[" + Value.ToString() + "]";
            }
            set
            {
                if (0 <= Value && Value <= ACMax)
                    ecgMnemonicStrings[Value] = value;
                else
                    throw new ArgumentException("Illegal annotation code:" + Value);
            }       
        }


        /* descriptive strings for each code */
        private static string[] descriptions = {
            "",
            "Normal beat",
            "Left bundle branch block beat",
            "Right bundle branch block beat",
            "Aberrated atrial premature beat",
            "Premature ventricular contraction",
            "Fusion of ventricular and normal beat",
            "Nodal (junctional) premature beat",
            "Atrial premature beat",
            "Supraventricular premature or ectopic beat",
            "Ventricular escape beat",
            "Nodal (junctional) escape beat",
            "Paced beat",
            "Unclassifiable beat",
            "Change in signal quality",
            null,
            "Isolated QRS-like artifact",
            null,
            "ST segment change",
            "T-wave change",
            "Systole",
            "Diastole",
            "Comment annotation",
            "Measurement annotation",
            "P-wave peak",
            "Bundle branch block beat (unspecified)",
            "(Non-captured) pacemaker artifact",
            "T-wave peak",
            "Rhythm change",
            "U-wave peak",
            "Beat not classified during learning",
            "Ventricular flutter wave",
            "Start of ventricular flutter/fibrillation",
            "End of ventricular flutter/fibrillation",
            "Atrial escape beat",
            "Supraventricular escape beat",
            "Link to external data (aux contains URL)",
            "Non-conducted P-wave (blocked APC)",
            "Fusion of paced and normal beat",
            "Waveform onset",
            "Waveform end",
            "R-on-T premature ventricular contraction",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        };
        /// <summary>
        /// Gets or sets the description of the this annotation code.
        /// </summary>
        public string Description
        {
            get
            {
                if (0 <= Value && Value <= ACMax)
                    return descriptions[Value];
                else
                    return "Illegal annotation code:" + Value;
                //return Marshal.PtrToStringAnsi(PInvoke.anndesc(this));
            }
            set
            {
                if (0 <= Value && Value <= ACMax)
                    descriptions[Value] = value;
                else
                    throw new ArgumentException("Illegal annotation code:"+Value);
                //PInvoke.setanndesc(this, value);
            }
        }


        private static bool[] isQrs = {
        false, true,  true,  true,  true,  true,  true,  true,  true,  true,	/* 0 - 9 */
	    true,  true,  true,  true,  false, false, false, false, false, false,	/* 10 - 19 */
	    false, false, false, false, false, true,  false, false, false, false,	/* 20 - 29 */
	    true,  true,  false, false, true,  true,  false, false, true,  false,	/* 30 - 39 */
	    false, true,  false, false, false, false, false, false, false, false	/* 40 - 49 */
        };
        /// <summary>
        /// Gets or sets a value indicating whether or not this annotation code denotes a QRS complex.
        /// </summary>
        public bool IsQrs
        {
            get
            {
                return IsAnnotation ? isQrs[Value] : false;
            }
            set
            {
                if (IsAnnotation)
                    isQrs[Value]=value;
            }
        }

        private static AnnotationCode[] map1 = {
            NotQrs, Normal, Normal, Normal, Normal,		/* 0 - 4 */
	        Pvc,    Fusion, Normal, Normal, Normal,		/* 5 - 9 */
	        Pvc,    Normal, Normal, Normal, NotQrs,		/* 10 - 14 */
	        NotQrs, NotQrs, NotQrs, NotQrs, NotQrs,		/* 15 - 19 */
	        NotQrs, NotQrs, NotQrs, NotQrs, NotQrs,		/* 20 - 24 */
	        Normal, NotQrs, NotQrs, NotQrs, NotQrs,		/* 25 - 29 */
	        Learn,  Pvc,    NotQrs, NotQrs, Normal,		/* 30 - 34 */
	        Normal, NotQrs, NotQrs, Normal, NotQrs,		/* 35 - 39 */
	        NotQrs, Pvc,    NotQrs, NotQrs, NotQrs,		/* 40 - 44 */
	        NotQrs, NotQrs, NotQrs, NotQrs, NotQrs		/* 45 - 49 */
        };
        /// <summary>
        /// Gets or sets the resulting annotation code using <see cref="PInvoke.wfdb_map1"/> macro.
        /// The resulting annotation code is one of {<see cref="AnnotationCode.NotQrs"/>, <see cref="AnnotationCode.Normal"/>, 
        /// <see cref="AnnotationCode.Pvc"/>, <see cref="AnnotationCode.Fusion"/>, <see cref="AnnotationCode.Learn"/>}
        /// </summary>
        public AnnotationCode Map1
        {   
            get
            {
                return IsAnnotation ? map1[Value] : NotQrs;
            }
            set
            {
                if (IsAnnotation)
                    map1[Value] = value;
            }
        }


        private static AnnotationCode[] map2 = {
            NotQrs, Normal, Normal, Normal, Svpb,		/* 0 - 4 */
	        Pvc,    Fusion, Svpb,   Svpb,   Svpb,		/* 5 - 9 */
	        Pvc,    Normal, Normal, Normal, NotQrs,		/* 10 - 14 */
	        NotQrs, NotQrs, NotQrs, NotQrs, NotQrs,		/* 15 - 19 */
	        NotQrs, NotQrs, NotQrs, NotQrs, NotQrs,		/* 20 - 24 */
	        Normal, NotQrs, NotQrs, NotQrs, NotQrs,		/* 25 - 29 */
	        Learn,  Pvc,    NotQrs, NotQrs, Normal,		/* 30 - 34 */
	        Normal, NotQrs, NotQrs, Normal, NotQrs,		/* 35 - 39 */
	        NotQrs, Pvc,    NotQrs, NotQrs, NotQrs,		/* 40 - 44 */
	        NotQrs, NotQrs, NotQrs, NotQrs, NotQrs		/* 45 - 49 */
        };
        /// <summary>
        /// Gets or sets the resulting annotation code using <see cref="PInvoke.wfdb_map2"/> macro.
        /// The resulting annotation code is one of the set {<see cref="AnnotationCode.NotQrs"/>, 
        /// <see cref="AnnotationCode.Normal"/>, <see cref="AnnotationCode.Pvc"/>, <see cref="AnnotationCode.Fusion"/>, <see cref="AnnotationCode.Learn"/>}
        /// </summary>
        public AnnotationCode Map2
        {
            get
            {
                return IsAnnotation ? map2[Value] : NotQrs;
            }
            set
            {
                if (IsAnnotation)
                    map2[Value] = value;
            }
        }

        private static AnnotationPos[] annotationPos = {
            AnnotationPos.APUndef,AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,		/* 0 - 4 */
	        AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,		/* 5 - 9 */
	        AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APHigh,		/* 10 - 14 */
	        AnnotationPos.APUndef,AnnotationPos.APHigh, AnnotationPos.APUndef,AnnotationPos.APHigh, AnnotationPos.APHigh,		/* 15 - 19 */
	        AnnotationPos.APHigh, AnnotationPos.APHigh, AnnotationPos.APHigh, AnnotationPos.APHigh, AnnotationPos.APHigh,		/* 20 - 24 */
	        AnnotationPos.APStd,  AnnotationPos.APHigh, AnnotationPos.APHigh, AnnotationPos.APLow,  AnnotationPos.APHigh,		/* 25 - 29 */
	        AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,  AnnotationPos.APStd,		/* 30 - 34 */
	        AnnotationPos.APStd,  AnnotationPos.APHigh, AnnotationPos.APHigh, AnnotationPos.APStd,  AnnotationPos.APHigh,		/* 35 - 39 */
	        AnnotationPos.APHigh, AnnotationPos.APStd,  AnnotationPos.APUndef,AnnotationPos.APUndef,AnnotationPos.APUndef,	    /* 40 - 44 */
	        AnnotationPos.APUndef,AnnotationPos.APUndef,AnnotationPos.APUndef,AnnotationPos.APUndef,AnnotationPos.APUndef		/* 45 - 49 */
        };
        /// <summary>
        /// Gets or sets the appropriate position code for this annotation code.
        /// <remarks>
        /// This macro was first introduced in WFDB library version 6.0.
        /// </remarks>
        /// </summary>
        public AnnotationPos AnnotationPos
        {
            get
            {
                return IsAnnotation ? annotationPos[Value] : AnnotationPos.APUndef;
            }
            set
            {
                if(IsAnnotation)
                {
                    annotationPos[Value] = value;
                }
            }
        }
        #endregion

        #region operator overloads
        public static implicit operator AnnotationCode(byte value)
        {
            return new AnnotationCode(value);
        }

        public static implicit operator byte(AnnotationCode code)
        {
            return code.value;
        }

        public static bool operator ==(AnnotationCode code1, AnnotationCode code2)
        {
            return code1.Value == code2.Value;
        }

        public static bool operator !=(AnnotationCode code1, AnnotationCode code2)
        {
            return code1.Value != code2.Value;
        }

        #endregion

        #region Annotation Codes
        /// <summary>
        /// 0:Not Qrs, no meaning but legal.
        /// </summary>
        public static AnnotationCode NotQrs { get { return 0; } }

        /// <summary>
        /// 1:Normal beat 'N'.
        /// </summary>
        public static AnnotationCode Normal { get { return 1; } }


        /// <summary>
        /// 2:Left bundle branch block beat 'L'.
        /// </summary>
        public static AnnotationCode Lbbb { get { return 2; } }

        /// <summary>
        /// 3:Right bundle branch block beat 'R'.
        /// </summary>
        public static AnnotationCode Rbbb { get { return 3; } }

        /// <summary>
        /// 25:Bundle branch block beat (unspecified) 'B'.
        /// </summary>
        public static AnnotationCode Bbb { get { return 25; } }

        /// <summary>
        /// 8 Atrial premature beat 'A'.
        /// </summary>
        public static AnnotationCode Apc { get { return 8; } }

        /// <summary>
        /// 4:Aberrated atrial premature beat 'a'.
        /// </summary>
        public static AnnotationCode Aberr { get { return 4; } }

        /// <summary>
        /// 7:Nodal (junctional) premature beat 'J'.
        /// </summary>
        public static AnnotationCode Npc { get { return 7; } }

        /// <summary>
        /// 9:Supraventricular premature or ectopic beat (atrial or nodal) 'S'.
        /// </summary>
        public static AnnotationCode Svpb { get { return 9; } }

        /// <summary>
        /// 5:Premature ventricular contraction 'V'.
        /// </summary>
        public static AnnotationCode Pvc { get { return 5; } }

        /// <summary>
        /// 41:R-on-T premature ventricular contraction 'r'.
        /// </summary>
        public static AnnotationCode ROnT { get { return 41; } }

        /// <summary>
        /// 6:Fusion of ventricular and normal beat 'F'.
        /// </summary>
        public static AnnotationCode Fusion { get { return 6; } }

        /// <summary>
        /// 34:Atrial escape beat 'e'.
        /// </summary>
        public static AnnotationCode Aesc { get { return 34; } }

        /// <summary>
        /// 11:Nodal (junctional) escape beat 'j'.
        /// </summary>
        public static AnnotationCode Nesc { get { return 11; } }

        /// <summary>
        /// 35:Supraventricular escape beat (atrial or nodal) 'n'.
        /// <remarks>
        /// This code was first introduced in WFDB library version 4.0.
        /// </remarks>
        /// </summary>
        public static AnnotationCode Svesc { get { return 35; } }

        /// <summary>
        /// 10:Ventricular escape beat 'E'.
        /// </summary>
        public static AnnotationCode Vesc { get { return 10; } }

        /// <summary>
        /// 12:Paced beat '/'.
        /// </summary>
        public static AnnotationCode Pace { get { return 12; } }

        /// <summary>
        /// 38:Fusion of paced and normal beat 'f'.
        /// </summary>
        public static AnnotationCode Pfus { get { return 38; } }

        /// <summary>
        /// 13:Unclassifiable beat 'Q'.
        /// </summary>
        public static AnnotationCode Unknown { get { return 13; } }

        /// <summary>
        /// 30:Beat not classified during learning '?'.
        /// </summary>
        public static AnnotationCode Learn { get { return 30; } }

        #region Non-beat annotation codes:
        /// <summary>
        /// 32:Start of ventricular flutter/fibrillation '['
        /// </summary>
        public static AnnotationCode VfOn { get { return 32; } }

        /// <summary>
        /// 31:Ventricular flutter wave '!'
        /// </summary>
        public static AnnotationCode FLWav { get { return 31; } }

        /// <summary>
        /// 33:End of ventricular flutter/fibrillation ']'
        /// </summary>
        public static AnnotationCode VfOff { get { return 33; } }

        /// <summary>
        /// 37:Non-conducted P-wave (blocked APC) 'x'
        /// </summary>
        public static AnnotationCode NApc { get { return 37; } }

        /// <summary>
        /// 39:Waveform onset '('
        /// </summary>
        public static AnnotationCode WFOn { get { return 39; } }

        /// <summary>
        /// 40:Waveform end ')'
        /// </summary>
        public static AnnotationCode WFOff { get { return 40; } }

        /// <summary>
        /// 24:Peak of P-wave 'p'
        /// <remarks>
        /// This code was first introduced in DB library version
        /// 8.3. The ‘p’ mnemonic now assigned to <see cref="PWave"/> was formerly assigned to <see cref="NApc"/>.
        /// </remarks>
        /// </summary>
        public static AnnotationCode PWave { get { return 24; } }

        /// <summary>
        /// 27:Peak of T-wave 't'
        /// <remarks>
        /// This code was first introduced in DB library version
        /// 8.3. The ‘t’ mnemonic now assigned to <see cref="TWave"/> was formerly assigned to <see cref="TCh"/>.
        /// </remarks>
        /// </summary>
        public static AnnotationCode TWave { get { return 27; } }

        /// <summary>
        /// 29:Peak of U-wave 'u'
        /// <remarks>
        /// This code was first introduced in DB library version 8.3. 
        /// </remarks>
        /// </summary>
        public static AnnotationCode UWave { get { return 29; } }

        /// <summary>
        /// 39:PQ junction '‘'.
        /// <remarks>
        /// The obsolete code PQ (designating the PQ junction) is still defined , but is identical to <see cref="WFOn"/>.
        /// </remarks>
        /// </summary>
        [Obsolete]
        public static AnnotationCode PQ { get { return WFOn; } }

        /// <summary>
        /// 40:J-point '’'
        /// <remarks>
        /// The obsolete code JPt (designating the J-point) is still defined , but is identical to <see cref="WFOff"/>.
        /// </remarks>
        /// </summary>
        [Obsolete]
        public static AnnotationCode JPt { get { return WFOff; } }

        /// <summary>
        /// 26:(Non-captured) pacemaker artifact '^'
        /// </summary>
        public static AnnotationCode PaceSP { get { return 26; } }

        /// <summary>
        /// 16:Isolated QRS-like artifact '|'
        /// <remarks>
        /// In MIT and ESC DB ‘atr’ files, each non-zero bit in the subtyp field indicates that
        /// the corresponding signal contains noise (the least significant bit corresponds to signal
        /// 0).
        /// </remarks>
        /// </summary>
        public static AnnotationCode Arfct { get { return 16; } }

        /// <summary>
        /// 14:Change in signal quality '~'
        /// <remarks>
        /// In MIT and ESC DB ‘atr’ files, each non-zero bit in the subtyp field indicates that
        /// the corresponding signal contains noise (the least significant bit corresponds to signal
        /// 0).
        /// </remarks>
        /// </summary>
        public static AnnotationCode Noise { get { return 14; } }

        /// <summary>
        /// 28:Rhythm change '+'
        /// <remarks>
        /// The aux field contains an ASCII string (with prefixed byte count) describing the
        /// rhythm, ST segment, T-wave change, measurement, or the nature of the comment.
        /// By convention, the character that follows the byte count in the aux field of a RHYTHM
        /// annotation is ‘(’. See the MIT-BIH Arrhythmia Database Directory for a list of rhythm
        /// annotation strings.
        /// </remarks>
        /// </summary>
        public static AnnotationCode Rhythm { get { return 28; } }

        /// <summary>
        /// 18:ST segment change 's'
        /// <remarks>
        /// - This code was first introduced in WFDB library version 4.0.
        /// - The aux field contains an ASCII string (with prefixed byte count) describing the
        /// rhythm, ST segment, T-wave change, measurement, or the nature of the comment.
        /// By convention, the character that follows the byte count in the aux field of a <see cref="Rhythm"/>
        /// annotation is ‘(’. See the MIT-BIH Arrhythmia Database Directory for a list of rhythm
        /// annotation strings.
        /// </remarks>
        /// </summary>
        public static AnnotationCode STCh { get { return 18; } }

        /// <summary>
        /// 19:T-wave change 'T'
        /// <remarks>
        /// - This code was first introduced in WFDB library version 4.0.
        /// - The aux field contains an ASCII string (with prefixed byte count) describing the
        /// rhythm, ST segment, T-wave change, measurement, or the nature of the comment.
        /// By convention, the character that follows the byte count in the aux field of a <see cref="Rhythm"/>
        /// annotation is ‘(’. See the MIT-BIH Arrhythmia Database Directory for a list of rhythm
        /// annotation strings.
        /// - the ‘t’ mnemonic now assigned to TWAVE was formerly assigned to TCH.
        /// </remarks>
        /// </summary>
        public static AnnotationCode TCh { get { return 19; } }

        /// <summary>
        /// 20:Systole '*'
        /// <remarks>
        /// This code was first introduced in WFDB library version 7.0.
        /// </remarks>
        /// </summary>
        public static AnnotationCode Systole { get { return 20; } }

        /// <summary>
        /// 21:Diastole 'D'
        /// <remarks>
        /// - This code was first introduced in WFDB library version 7.0.
        /// </remarks>
        /// </summary>
        public static AnnotationCode Diastole { get { return 21; } }

        /// <summary>
        /// 23:Measurement annotation '='
        /// <remarks>
        /// - This code was first introduced in WFDB library version 7.0.
        /// - The aux field contains an ASCII string (with prefixed byte count) describing the
        /// rhythm, ST segment, T-wave change, measurement, or the nature of the comment.
        /// By convention, the character that follows the byte count in the aux field of a <see cref="Rhythm"/>
        /// annotation is ‘(’. See the MIT-BIH Arrhythmia Database Directory for a list of rhythm
        /// annotation strings.
        /// </remarks>
        /// </summary>
        public static AnnotationCode Measure { get { return 23; } }

        /// <summary>
        /// 22:Comment annotation '"'
        /// <remarks>
        /// The aux field contains an ASCII string (with prefixed byte count) describing the
        /// rhythm, ST segment, T-wave change, measurement, or the nature of the comment.
        /// By convention, the character that follows the byte count in the aux field of a <see cref="Rhythm"/>
        /// annotation is ‘(’. See the MIT-BIH Arrhythmia Database Directory for a list of rhythm
        /// annotation strings.
        /// </remarks>
        /// </summary>
        public static AnnotationCode Note { get { return 22; } }

        /// <summary>
        /// 36:Link to external data '@'.
        /// <remarks>
        /// The <see cref="Link"/>  code was first introduced in WFDB library version 9.6. The aux field
        /// of a LINK annotation contains a URL (a uniform resource locator, in the form
        /// ‘http://machine.name/some/data’, suitable for passing to a Web browser such as
        /// Netscape or Mosaic). LINK annotations may be used to associate extended text,
        /// images, or other data with an annotation file. If the aux field contains any whitespace,
        /// text following the first whitespace is taken as descriptive text to be displayed by a
        /// WFDB browser such as WAVE.
        /// </remarks>
        /// </summary>
        public static AnnotationCode Link { get { return 36; } }

        /// <summary>
        /// 49:Value of largest valid annotation code.
        /// </summary>
        public static AnnotationCode ACMax { get { return 49; } }

        /// <summary>
        /// User defined.
        /// </summary>
        public static AnnotationCode Reserved42 { get { return 42; } }

        /// <summary>
        /// User defined
        /// </summary>
        public static AnnotationCode Reserved43 { get { return 43; } }

        /// <summary>
        /// User defined
        /// </summary>
        public static AnnotationCode Reserved44 { get { return 44; } }

        /// <summary>
        /// User defined
        /// </summary>
        public static AnnotationCode Reserved45 { get { return 45; } }

        /// <summary>
        /// User defined
        /// </summary>
        public static AnnotationCode Reserved46 { get { return 46; } }

        /// <summary>
        /// User defined
        /// </summary>
        public static AnnotationCode Reserved47 { get { return 47; } }

        /// <summary>
        /// User defined
        /// </summary>
        public static AnnotationCode Reserved48 { get { return 48; } }

        #endregion



        #endregion

        #region Pseudo-Annotations Codes
        /// <summary>
        /// 59:Anotación nula, donde debe leer los siguientes 4 bytes para obtener el intervalo de tiempo de la siguiente
        /// annotación no nula
        ///  I=0; the next four bytes are the interval in PDP-11 long integer format (the high 16 bits first, then the low 16 bits, with the low byte first in each pair).
        /// </summary>
        public static AnnotationCode Skip { get { return 59; } }

        /// <summary>
        /// 60:I=annotation num field for current and subsequent annotations;
        /// otherwise, assume previous annotation num (initially 0).
        /// </summary>
        public static AnnotationCode AnnotatorNumber { get { return 60; } }

        /// <summary>
        ///  61:I=annotation subtyp field for current annotation only; otherwise, assume subtyp = 0
        /// </summary>
        public static AnnotationCode SubType { get { return 61; } }

        /// <summary>
        /// 62:I=annotation chan field for current and subsequent annotations; otherwise, assume previous chan (initially 0).
        /// </summary>
        public static AnnotationCode ChannelNumber { get { return 62; } }

        /// <summary>
        /// 63:I = number of bytes of auxiliary information (which is contained in the next I bytes); an extra null, not included in the byte count, is appended if I is odd.
        /// </summary>
        public static AnnotationCode Aux { get { return 63; } }


        #endregion

        #region IComparable<AnnotationCode> Members

        public int CompareTo(AnnotationCode other)
        {
            return Value.CompareTo(other.Value);
        }

        #endregion
    }
}
