using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoWFDB
{
    public static class FileStreamExtension
    {
        /// <summary>
        /// read a 16-bit integer in PDP-11 format
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static ushort Read2BytesPDP_11(this FileStream fs)
        {
            int lsb = fs.ReadByte();
            int msb = fs.ReadByte();
            ushort r = (ushort)((msb << 8) | (lsb & 0xff));
            return r;
        }

        /// <summary>
        ///  read a 32-bit integer in PDP-11 format
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static uint Read4BytesPDP_11(this FileStream fs)
        {
            ushort msw = Read2BytesPDP_11(fs);
            ushort lsw = Read2BytesPDP_11(fs);
            uint r =(uint)( (msw << 16) | (lsw & 0xffff));
            return r;
        }

        /// <summary>
        /// Read a 24-bits in little endian
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static int ReadF24bits(this FileStream fs)
        {
            int lastByteRead=0;
            int[] bytes = new int[3];
            int i,result;

            //Leer los bytes de menos a mas significativo, y desplazar hacia la izquierda
            for (i = 0; i < bytes.Length; i++)
            {
                lastByteRead = fs.ReadByte();               
                bytes[i] = lastByteRead << (i * 8);
            }
          
            //Si es negativo el byte mas significativo, extender el signo
            //[1111 1111] [xxxx xxxx] [xxxx xxxx] [xxxx xxxx]
            if (lastByteRead > 127)
                result = 0b1111_1111 << 24;
            else
                result = 0;

            //Concatenar los bytes comenzando por el más significativo
            for (i = bytes.Length - 1; i >= 0; i--)
            {
                result = result | bytes[i];
            }

            return result;
        }



        /// <summary>
        /// Read a 16-bits in little endian
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static short ReadF16bits(this FileStream fs)
        {
            int LSByte = fs.ReadByte();
           
            int MSByte = fs.ReadByte();

            // Desplazar 1 byte a la derecha: abcd efgh 0000 0000
            ushort wordMSB = (ushort)(MSByte << 8);
            ushort wordLSB = (ushort)(LSByte);
            short result = (short)(wordMSB | wordLSB);

            return result;
        }



        /// <summary>
        /// Read a 16-bits in big endian, format 61
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static short ReadF61bits(this FileStream fs)
        {
            int MSByte = fs.ReadByte();

            int LSByte = fs.ReadByte();

            // Desplazar 1 byte a la derecha: abcd efgh 0000 0000
            ushort wordMSB = (ushort)(MSByte << 8);
            ushort wordLSB = (ushort)(LSByte);
            short result = (short)(wordMSB | wordLSB);

            return result;
        }
    }
}
