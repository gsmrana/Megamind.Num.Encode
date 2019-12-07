using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Megamind.Num.Encode
{
    public enum Sequence
    {
        LsbFirst,
        MsbFirst
    }

    public static class MU
    {
        #region byte <--> byte[] convertion

        public static byte[] BoolToArray(bool sts)
        {
            return new[] { (byte)(sts ? 1 : 0) };
        }

        public static void BoolToArray(bool sts, ref byte[] buffer, int offset = 0)
        {
            buffer[offset] = (byte)(sts ? 1 : 0);
        }

        public static byte[] ByteToArray(byte val)
        {
            return new[] { val };
        }

        public static void ByteToArray(int val, ref byte[] buffer, int offset = 0)
        {
            buffer[offset] = (byte)val;
        }

        public static byte Int16MSB(int val)
        {
            return (byte)((val >> 8) & 0xFF);
        }

        public static byte Int16LSB(int val)
        {
            return (byte)(val & 0xFF);
        }

        #endregion

        #region Int16 <--> Array

        public static int GetInt16(byte msb, byte lsb)
        {
            return ((msb << 8) | lsb);
        }

        public static int ArrayToInt16(byte[] bytes, int offset = 0, Sequence seq = Sequence.LsbFirst)
        {
            int val = 0;
            if (seq == Sequence.LsbFirst)
            {
                val += bytes[offset + 0] << 0;
                val += bytes[offset + 1] << 8;
            }
            else
            {
                val += bytes[offset + 1] << 0;
                val += bytes[offset + 0] << 8;
            }
            return val;
        }

        public static byte[] Int16ToArray(int val, Sequence seq = Sequence.LsbFirst)
        {
            var buffer = new byte[2];
            Int16ToArray(val, ref buffer, 0, seq);
            return buffer;
        }

        public static void Int16ToArray(int val, ref byte[] buffer, int offset = 0, Sequence seq = Sequence.LsbFirst)
        {
            if (seq == Sequence.LsbFirst)
            {
                buffer[offset + 0] = (byte)((val >> 0) & 0xFF);
                buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
            }
            else
            {
                buffer[offset + 1] = (byte)((val >> 0) & 0xFF);
                buffer[offset + 0] = (byte)((val >> 8) & 0xFF);
            }
        }

        #endregion

        #region Int32 <--> Array

        public static int ArrayToInt32(byte[] bytes, int offset = 0, Sequence seq = Sequence.LsbFirst)
        {
            int val = 0;
            if (seq == Sequence.LsbFirst)
            {
                val += bytes[offset + 0] << 0;
                val += bytes[offset + 1] << 8;
                val += bytes[offset + 2] << 16;
                val += bytes[offset + 3] << 24;
            }
            else
            {
                val += bytes[offset + 3] << 0;
                val += bytes[offset + 2] << 8;
                val += bytes[offset + 1] << 16;
                val += bytes[offset + 0] << 24;
            }
            return val;
        }

        public static void Int32ToArray(int val, ref byte[] buffer, int offset = 0, Sequence seq = Sequence.LsbFirst)
        {
            if (seq == Sequence.LsbFirst)
            {
                buffer[offset + 0] = (byte)((val >> 0) & 0xFF);
                buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
                buffer[offset + 2] = (byte)((val >> 16) & 0xFF);
                buffer[offset + 3] = (byte)((val >> 24) & 0xFF);
            }
            else
            {
                buffer[offset + 3] = (byte)((val >> 0) & 0xFF);
                buffer[offset + 2] = (byte)((val >> 8) & 0xFF);
                buffer[offset + 1] = (byte)((val >> 16) & 0xFF);
                buffer[offset + 0] = (byte)((val >> 24) & 0xFF);
            }
        }

        #endregion

        #region String <--> Array

        public static void StringToByteArray(string str, ref byte[] bytes, int index = 0)
        {
            var data = Encoding.ASCII.GetBytes(str);
            Array.Copy(data, 0, bytes, index, data.Length);
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static string ByteArrayToString(byte[] bytes, int index, int count)
        {
            return Encoding.ASCII.GetString(bytes, index, count);
        }

        #endregion

        #region Hex Encoding

        public static string ByteArrayToHexString(byte[] bytes, string separator = "")
        {
            return BitConverter.ToString(bytes).Replace("-", separator);
        }

        public static string ByteArrayToHexString(byte[] bytes, int index, int count, string separator = "")
        {
            return BitConverter.ToString(bytes, index, count).Replace("-", separator);
        }

        public static byte[] HexStringToByteArray(string hexstr)
        {
            hexstr = hexstr.Replace("-", "");
            hexstr = hexstr.Replace(" ", "");
            return Enumerable.Range(0, hexstr.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexstr.Substring(x, 2), 16))
                             .ToArray();
        }

        #endregion

        #region DateTime <--> byte[] conversion

        public static byte[] DateTimeToArray(DateTime dt)
        {
            var buffer = new byte[7];
            DateTimeToArray(dt, ref buffer);
            return buffer;
        }

        public static void DateTimeToArray(DateTime dt, ref byte[] buffer, int offset = 0)
        {
            //38-05-0A-06-07-E107
            buffer[offset + 0] = (byte)dt.Second;
            buffer[offset + 1] = (byte)dt.Minute;
            buffer[offset + 2] = (byte)dt.Hour;           //24hr
            buffer[offset + 3] = (byte)dt.Day;
            buffer[offset + 4] = (byte)dt.Month;
            buffer[offset + 5] = (byte)(dt.Year & 0xFF);  //lsb
            buffer[offset + 6] = (byte)(dt.Year >> 8);    //msb
        }

        public static DateTime ArrayToDateTime(byte[] bytes, int offset = 0)
        {
            if (bytes.Length < (offset + 7)) throw new Exception("Minimum 7 bytes data needed for parsing DateTime.");

            //38-05-0A-06-07-E107
            var second = bytes[offset + 0];
            var minute = bytes[offset + 1];
            var hour = bytes[offset + 2];
            var day = bytes[offset + 3];
            var month = bytes[offset + 4];
            var year = (bytes[offset + 6] << 8) | bytes[offset + 5];
            return new DateTime(year, month, day, hour, minute, second);
        }

        #endregion
    }
}
