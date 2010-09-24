using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
namespace Silmoon.Configure
{
    ///  <summary>
    ///  读写ini文件的类
    ///  调用kernel32.dll中的两个api：WritePrivateProfileString，GetPrivateProfileString来实现对ini  文件的读写。
    ///
    ///  INI文件是文本文件,
    ///  由若干节(section)组成,
    ///  在每个带括号的标题下面,
    ///  是若干个关键词(key)及其对应的值(value)
    ///  
    ///[Section]
    ///Key=value
    ///
    ///  </summary>
    public sealed class IniFile : MarshalByRefObject
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        private string filePath;
        private int stringBufferSize;

        public IniFile()
        {
            this.stringBufferSize = 0x400;
        }
        public IniFile(string INIPath)
        {
            this.stringBufferSize = 0x400;
            this.filePath = INIPath;
        }

        public string ReadInivalue(string Section, string Key)
        {
            StringBuilder retVal = new StringBuilder(this.stringBufferSize);
            int num = GetPrivateProfileString(Section, Key, "", retVal, this.stringBufferSize, this.FilePath);
            return retVal.ToString();
        }
        public void WriteInivalue(string Section, string Key, string value)
        {
            WritePrivateProfileString(Section, Key, value, this.FilePath);
        }


        public string FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
            }
        }
        public int StringBufferSize
        {
            get
            {
                return this.stringBufferSize;
            }
            set
            {
                this.stringBufferSize = value;
            }
        }
    }


}
