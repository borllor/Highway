using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace JinRi.Notify.Frame.Util
{
    public class CryptographyHelper
    {
        public static string Decrypt(string original, string key)
        {
            return Decrypt(original, key, Encoding.Default);
        }

        /// <summary>
        /// 使用指定密钥解密
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">字符编码方案</param>
        /// <returns>明文</returns>
        private static string Decrypt(string encrypted, string key, Encoding encoding)
        {
            byte[] buff = Convert.FromBase64String(encrypted);
            byte[] kb = Encoding.Default.GetBytes(key);
            return encoding.GetString(Decrypt(buff, kb));
        }

        /// <summary>
        /// 使用指定密钥解密数据
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        private static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD5(key);
            des.Mode = CipherMode.ECB;

            return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }

        //----------------------------------------------------------------------------------------

        /// <summary>
        /// 使用指定密钥加密
        /// </summary>
        /// <param name="original">需要加密的字符串</param>
        /// <param name="key">自定义的密钥</param>
        /// <returns>返回加密字符串</returns>
        public static string Encrypt(string original, string key)
        {
            byte[] buff = Encoding.Default.GetBytes(original);
            byte[] kb = Encoding.Default.GetBytes(key);
            return Convert.ToBase64String(Encrypt(buff, kb));
        }

        /// <summary>
        /// 使用指定密钥加密
        /// </summary>
        /// <param name="original">需要加密的字符串</param>
        /// <param name="key">自定义的密钥</param>
        /// <returns>返回加密字符串</returns>
        private static byte[] Encrypt(byte[] original, byte[] key)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD5(key);
            des.Mode = CipherMode.ECB;

            return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
        }


        //----------------------------------------------------------------------------------------

        /// <summary>
        /// 生成MD5摘要
        /// </summary>
        /// <param name="original">数据源</param>
        /// <returns>摘要</returns>
        private static byte[] MakeMD5(byte[] original)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyhash = hashmd5.ComputeHash(original);
            hashmd5 = null;
            return keyhash;
        }


        //默认DES密钥向量
        private static byte[] DesKeys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串,异常返回字符串</returns>
        public static string DesEncode(string encryptString, string encryptKey)
        {
            #region
            try
            {
                encryptKey = GetSubString(encryptKey, 8, "");
                encryptKey = encryptKey.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = DesKeys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray()).Replace("+", "[");
            }
            catch
            {
                return "";
            }
            #endregion
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串,异常返回字符串</returns>
        public static string DesDecode(string decryptString, string decryptKey)
        {
            #region
            try
            {
                decryptString = decryptString.Replace("[", "+");
                decryptKey = GetSubString(decryptKey, 8, "");
                decryptKey = decryptKey.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = DesKeys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return "";
            }
            #endregion
        }

        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        internal static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            #region
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
            #endregion
        }

        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        internal static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            #region
            string myResult = p_SrcString;
            try
            {
                Byte[] bComments = Encoding.UTF8.GetBytes(p_SrcString);
                foreach (char c in Encoding.UTF8.GetChars(bComments))
                {    //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
                    if ((c > '\u0800' && c < '\u4e00') || (c > '\xAC00' && c < '\xD7A3'))
                    {
                        //if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") || System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
                        //当截取的起始位置超出字段串长度时
                        if (p_StartIndex >= p_SrcString.Length)
                        {
                            return "";
                        }
                        else
                        {
                            return p_SrcString.Substring(p_StartIndex,
                                                           ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                        }
                    }
                }
                if (p_Length >= 0)
                {
                    byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                    //当字符串长度大于起始位置
                    if (bsSrcString.Length > p_StartIndex)
                    {
                        int p_EndIndex = bsSrcString.Length;

                        //当要截取的长度在字符串的有效长度范围内
                        if (bsSrcString.Length > (p_StartIndex + p_Length))
                        {
                            p_EndIndex = p_Length + p_StartIndex;
                        }
                        else
                        {   //当不在有效范围内时,只取到字符串的结尾

                            p_Length = bsSrcString.Length - p_StartIndex;
                            p_TailString = "";
                        }
                        int nRealLength = p_Length;
                        int[] anResultFlag = new int[p_Length];
                        byte[] bsResult = null;
                        int nFlag = 0;
                        for (int i = p_StartIndex; i < p_EndIndex; i++)
                        {

                            if (bsSrcString[i] > 127)
                            {
                                nFlag++;
                                if (nFlag == 3)
                                {
                                    nFlag = 1;
                                }
                            }
                            else
                            {
                                nFlag = 0;
                            }

                            anResultFlag[i] = nFlag;
                        }
                        if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        {
                            nRealLength = p_Length + 1;
                        }
                        bsResult = new byte[nRealLength];
                        Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);
                        myResult = Encoding.Default.GetString(bsResult);
                        myResult = myResult + p_TailString;
                    }
                }
            }
            catch
            {
                myResult = "";
            }
            return myResult;
            #endregion
        }
    }
}
