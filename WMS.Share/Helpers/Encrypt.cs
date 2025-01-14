using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace WMS.Share.Helpers
{
    public class Encrypt
    {
        //private static string EncryptionKey = WMS.Share.Properties.Resources.hash;
        //private static byte[] key = { };
        //private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        //public static string Encryptstring(string texto)
        //{

        //    string returnstring = "";
        //    try
        //    {
        //        key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 8));
        //        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //        Byte[] inputByteArray = Encoding.UTF8.GetBytes(texto);
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
        //        cs.Write(inputByteArray, 0, inputByteArray.Length);
        //        cs.FlushFinalBlock();
        //        returnstring = Convert.ToBase64String(ms.ToArray());


        //        return returnstring;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}
        //public static string Decryptstring(string stringToDecrypt)
        //{

        //    Byte[] inputByteArray = new Byte[stringToDecrypt.Length];
        //    try
        //    {
        //        key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 8));
        //        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //        inputByteArray = Convert.FromBase64String(stringToDecrypt);
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
        //        cs.Write(inputByteArray, 0, inputByteArray.Length);
        //        cs.FlushFinalBlock();
        //        Encoding encoding = Encoding.UTF8;


        //        return encoding.GetString(ms.ToArray());
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }

        //}



        //public static string Encryptstring(string message)
        //{
        //    byte[] data = UTF32Encoding.UTF8.GetBytes(message);
        //    MD5 mD5 = MD5.Create();
        //    TripleDES tripleDES = TripleDES.Create();
        //    tripleDES.Key = mD5.ComputeHash(UTF32Encoding.UTF8.GetBytes(WMS.Share.Properties.Resources.hash));
        //    tripleDES.Mode = CipherMode.ECB;
        //    ICryptoTransform transform = tripleDES.CreateEncryptor();
        //    byte[] resul = transform.TransformFinalBlock(data, 0, data.Length);
        //    return Convert.ToBase64String(resul);
        //}

        //public static string Decryptstring(string message)
        //{
        //    try
        //    {
        //        byte[] data = Convert.FromBase64String(message);
        //        MD5 mD5 = MD5.Create();
        //        TripleDES tripleDES = TripleDES.Create();
        //        tripleDES.Key = mD5.ComputeHash(UTF32Encoding.UTF8.GetBytes(WMS.Share.Properties.Resources.hash));
        //        tripleDES.Mode = CipherMode.ECB;
        //        ICryptoTransform transform = tripleDES.CreateDecryptor();
        //        byte[] resul = transform.TransformFinalBlock(data, 0, data.Length);
        //        return UTF32Encoding.UTF8.GetString(resul);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}

    }
}
