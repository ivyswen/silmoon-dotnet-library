using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Silmoon.MySilmoon
{
    /// <summary>
    /// �����²�Ʒ�����⹫�����Խ�������
    /// </summary>
    public class SilmoonProductGBCInternat :RunningAble, ISilmoonProductGBCInternat
    {
        private string _productString = "NULL";
        private string _releaseVersion = "0.0.0.0";
        private RunningState _runningState = RunningState.Stopped;
        private bool _initProduceInfo = false;

        public event OutputTextMessageHandler OnOutputTextMessage;
        public event OutputTextMessageHandler OnInputTextMessage;
        public event ThreadExceptionEventHandler OnThreadException;

        /// <summary>
        /// ��ʶ��Ʒ�����ַ���
        /// </summary>
        public string ProductString
        {
            get { return _productString; }
            set { _productString = value; }
        }
        /// <summary>
        /// ��Ʒ�������
        /// </summary>
        public string ReleaseVersion
        {
            get { return _releaseVersion; }
            set { _releaseVersion = value; }
        }

        public SilmoonProductGBCInternat()
        {
            
        }

        public void onOutputText(string message)
        {
            onOutputText(message, 0);
        }
        public void onOutputText(string message, int flag)
        {
            if (OnOutputTextMessage != null) OnOutputTextMessage(message, flag);
        }
        public void onInputText(string message)
        {
            onInputText(message, 0);
        }
        public void onInputText(string message, int flag)
        {
            if (OnInputTextMessage != null) OnInputTextMessage(message, flag);
        }
        public void onThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (OnThreadException != null) OnThreadException(sender, e);
        }

        public void ValidateLicense()
        {
            
        }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="productString">ָ����Ʒ�����ַ���</param>
        /// <param name="releaseVersion">ָ��������Ʒ�����</param>
        public bool InitProductInfo(string productString, string releaseVersion)
        {
            if (!_initProduceInfo)
            {
                _productString = productString;
                _releaseVersion = releaseVersion;
                return true;
            }
            else
                return false;
        }


    }
    public delegate void OutputTextMessageHandler(string message, int flag);
}