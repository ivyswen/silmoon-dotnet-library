using System;
using System.Collections.Generic;
using System.Text;

namespace Silmoon.Security
{
    public class SmHash
    {
        public SmHash()
        {

        }

        public static string Get16MD5(string strSource)
        {
            //new 
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //��ȡ�����ֽ����� 
            byte[] bytResult = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strSource));

            //ת�����ַ�������ȡ9��25λ 
            string strResult = BitConverter.ToString(bytResult, 4, 8);
            //ת�����ַ�����32λ 
            //string strResult = BitConverter.ToString(bytResult); 

            //BitConverterת���������ַ�������ÿ���ַ��м����һ���ָ�������Ҫȥ���� 
            strResult = strResult.Replace("-", "");
            return strResult;
        }
        //// <summary> 
        /// ����MD5��32λ����
        /// </summary> 
        /// <param name="strSource">��Ҫ���ܵ�����</param> 
        /// <returns>����32λ���ܽ��</returns> 
        public static string Get32MD5(string strSource)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strSource, "MD5");
        }
    }
}
