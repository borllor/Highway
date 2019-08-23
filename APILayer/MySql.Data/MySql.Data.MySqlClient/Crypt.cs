using MySql.Data.MySqlClient.Properties;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MySql.Data.MySqlClient
{
    internal class Crypt
    {
        // Methods
        public static string EncryptPassword(string password, string seed, bool new_ver)
        {
            long max = 0x3fffffffL;
            if (!new_ver)
            {
                max = 0x1ffffffL;
            }
            if ((password == null) || (password.Length == 0))
            {
                return password;
            }
            long[] numArray = Hash(seed);
            long[] numArray2 = Hash(password);
            long num2 = (numArray[0] ^ numArray2[0]) % max;
            long num3 = (numArray[1] ^ numArray2[1]) % max;
            if (!new_ver)
            {
                num3 = num2 / 2L;
            }
            char[] chArray = new char[seed.Length];
            for (int i = 0; i < seed.Length; i++)
            {
                double num5 = rand(ref num2, ref num3, max);
                chArray[i] = (char)((ushort)(Math.Floor((double)(num5 * 31.0)) + 64.0));
            }
            if (new_ver)
            {
                char ch = (char)((ushort)Math.Floor((double)(rand(ref num2, ref num3, max) * 31.0)));
                for (int j = 0; j < chArray.Length; j++)
                {
                    chArray[j] = (char)(chArray[j] ^ ch);
                }
            }
            return new string(chArray);
        }

        public static byte[] Get411Password(string password, string seed)
        {
            if (password.Length == 0)
            {
                return new byte[1];
            }
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] buffer = sha.ComputeHash(Encoding.Default.GetBytes(password));
            byte[] sourceArray = sha.ComputeHash(buffer);
            byte[] bytes = Encoding.Default.GetBytes(seed);
            byte[] destinationArray = new byte[bytes.Length + sourceArray.Length];
            Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
            Array.Copy(sourceArray, 0, destinationArray, bytes.Length, sourceArray.Length);
            byte[] buffer5 = sha.ComputeHash(destinationArray);
            byte[] buffer6 = new byte[buffer5.Length + 1];
            buffer6[0] = 20;
            Array.Copy(buffer5, 0, buffer6, 1, buffer5.Length);
            for (int i = 1; i < buffer6.Length; i++)
            {
                buffer6[i] = (byte)(buffer6[i] ^ buffer[i - 1]);
            }
            return buffer6;
        }

        private static long[] Hash(string P)
        {
            long num = 0x50305735L;
            long num2 = 0x12345671L;
            long num3 = 7L;
            for (int i = 0; i < P.Length; i++)
            {
                if ((P[i] != ' ') && (P[i] != '\t'))
                {
                    long num5 = '\x00ff' & P[i];
                    num ^= (((num & 0x3fL) + num3) * num5) + (num << 8);
                    num2 += (num2 << 8) ^ num;
                    num3 += num5;
                }
            }
            return new long[] { (num & 0x7fffffffL), (num2 & 0x7fffffffL) };
        }

        private static double rand(ref long seed1, ref long seed2, long max)
        {
            seed1 = (seed1 * 3L) + seed2;
            seed1 = seed1 % max;
            seed2 = ((seed1 + seed2) + 0x21L) % max;
            return (((double)seed1) / ((double)max));
        }

        private static void XorScramble(byte[] from, int fromIndex, byte[] to, int toIndex, byte[] password, int length)
        {
            if ((fromIndex < 0) || (fromIndex >= from.Length))
            {
                throw new ArgumentException(Resources.IndexMustBeValid, "fromIndex");
            }
            if ((fromIndex + length) > from.Length)
            {
                throw new ArgumentException(Resources.FromAndLengthTooBig, "fromIndex");
            }
            if (from == null)
            {
                throw new ArgumentException(Resources.BufferCannotBeNull, "from");
            }
            if (to == null)
            {
                throw new ArgumentException(Resources.BufferCannotBeNull, "to");
            }
            if ((toIndex < 0) || (toIndex >= to.Length))
            {
                throw new ArgumentException(Resources.IndexMustBeValid, "toIndex");
            }
            if ((toIndex + length) > to.Length)
            {
                throw new ArgumentException(Resources.IndexAndLengthTooBig, "toIndex");
            }
            if ((password == null) || (password.Length < length))
            {
                throw new ArgumentException(Resources.PasswordMustHaveLegalChars, "password");
            }
            if (length < 0)
            {
                throw new ArgumentException(Resources.ParameterCannotBeNegative, "count");
            }
            for (int i = 0; i < length; i++)
            {
                to[toIndex++] = (byte)(from[fromIndex++] ^ password[i]);
            }
        }
    }


}
