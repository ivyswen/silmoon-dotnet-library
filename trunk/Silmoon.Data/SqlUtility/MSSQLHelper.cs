using System;
using System.Collections.Generic;
using System.Text;
using Silmoon.Data.SqlClient;
using System.Data;
using System.Collections;

namespace Silmoon.Data.SqlUtility
{
    /// <summary>
    /// MySQLʵ����
    /// </summary>
    public class MSSQLHelper
    {
        SmMSSQLClient _mssql;
        /// <summary>
        /// ʹ��ָ����ODBC�������Ӵ���MYSQLʵ�ù���
        /// </summary>
        /// <param name="odbc">ָ��һ���Ѿ�����ʹ�õ�ODBC����</param>
        public MSSQLHelper(SmMSSQLClient odbc)
        {
            _mssql = odbc;
        }
        /// <summary>
        /// ˢ�����ݿ����ж���
        /// </summary>
        public void Refresh()
        {
            _mssql.ExecNonQuery("RECONFIGURE WITH OVERRIDE");
        }

        /// <summary>
        /// ����һ�����ݿ�
        /// </summary>
        /// <param name="database">���ݿ�����</param>
        /// <returns></returns>
        public int CreateDatabase(string database)
        {
            if (IsExistDatabase(database)) throw new MSSQLException(null, "���ݿ��Ѵ���");
            return _mssql.ExecNonQuery("CREATE DATABASE " + database);
        }
        /// <summary>
        /// ɾ��һ�����ݿ�
        /// </summary>
        /// <param name="database">���ݿ�����</param>
        /// <returns></returns>
        public int DropDatabase(string database)
        {
            if (database.ToLower() == "master") throw new MSSQLException(null, "ϵͳ���ݿ��޷�ɾ��");
            if (!IsExistDatabase(database)) throw new MSSQLException(null, "ָ����һ�������ڵ����ݿ�");
            return _mssql.ExecNonQuery("DROP DATABASE " + database);
        }
        /// <summary>
        /// ����һ���û������ƶ������������ݿ�
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="database">ָ�������ݿ�</param>
        public int AddUserToDatabase(string username, string database)
        {
            if (!IsExistDatabase(database)) throw new MSSQLException(null, "ָ����һ�������ڵ����ݿ�");
            if (!IsExistUser(username)) throw new MSSQLException(null, "ָ����һ�������ڵ����ݿ�");

            int result = _mssql.ExecNonQuery("USE [" + database + "];CREATE USER [" + username + "] FOR LOGIN [" + username + "]");
            _mssql.ExecNonQuery("USE [" + database + "];EXEC sp_addrolemember N'db_owner', N'" + username + "'");
            _mssql.ExecNonQuery("USE [Master]");
            Refresh();
            return result;
        }
        /// <summary>
        /// ����һ���û�
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <returns></returns>
        public int CreateUser(string username, string password)
        {
            if (username == "") throw new MSSQLException(null, "�������յ��û���!");
            if (IsExistUser(username)) throw new MSSQLException(null, "�û����Ѿ�����!");
            int result = _mssql.ExecNonQuery("CREATE LOGIN [" + username + "] WITH PASSWORD=N'" + password + "', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF");
            Refresh();
            return result;
        }
        /// <summary>
        /// ʹ������ǿ��ɾ��һ���û�
        /// </summary>
        /// <param name="username">�û���</param>
        /// <returns></returns>
        public int RemoveUser(string username)
        {
            if (username.ToLower() == "sa") throw new MSSQLException(null, "��ֹɾ��sa�û���");
            int result = _mssql.ExecNonQuery("DROP LOGIN [" + username + "]");
            Refresh();
            return result;
        }
        /// <summary>
        /// �Ƴ�һ���û������ݿ��Ȩ��
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="database">���ݿ�</param>
        /// <returns></returns>
        public int RemoveUserGrant(string username, string database)
        {
            if (!IsExistDatabase(database)) throw new MSSQLException(null, "ָ����һ�������ڵ����ݿ�");
            string brforeDatabase = _mssql.GetFieldObjectForSingleQuery("select db_name()").ToString();
            int result = _mssql.ExecNonQuery("USE " + database + ";drop user [" + username + "]");
            _mssql.ExecNonQuery("USE " + brforeDatabase);
            Refresh();
            return result;
        }
        /// <summary>
        /// ������ݿ��Ƿ����
        /// </summary>
        /// <param name="database">Ҫ�������ݿ�</param>
        /// <returns></returns>
        public bool IsExistDatabase(string database)
        {
            DataTable dt = _mssql.GetDataTable("Select Name From Master..SysDatabases");
            ArrayList _array = new ArrayList();
            foreach (DataRow row in dt.Rows) _array.Add(row[0]);
            string[] nameArr = (string[])_array.ToArray(typeof(string));
            dt.Clear();
            dt.Dispose();
            return SmString.FindFormStringArray(nameArr, database);
        }
        /// <summary>
        /// ���һ���û����Ƿ����
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsExistUser(string username)
        {
            DataTable dt = _mssql.GetDataTable("select Name from Master..syslogins where isntname=0");
            ArrayList _array = new ArrayList();
            foreach (DataRow row in dt.Rows) _array.Add(row[0]);
            string[] nameArr = (string[])_array.ToArray(typeof(string));
            dt.Clear();
            dt.Dispose();
            return SmString.FindFormStringArray(nameArr, username);
        }
        /// <summary>
        /// �����û�����
        /// </summary>
        /// <param name="username">Ŀ���û�</param>
        /// <param name="password">����</param>
        /// <returns></returns>
        public int SetPassword(string username, string password)
        {
            int i = _mssql.ExecNonQuery("USE [master];ALTER LOGIN ["+username+"] WITH PASSWORD=N'"+password+"'");
            return i;
        }
        /// <summary>
        /// ����һ������MySQL ODBC 3.51��������������Դ�������ַ���
        /// </summary>
        /// <param name="hostname">������</param>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <param name="database">���ݿ�</param>
        /// <returns></returns>
        public static string MakeConnectionString(string hostname, string username, string password, string database)
        {
            return "DRIVER={MySQL ODBC 3.51 Driver};SERVER=" + hostname + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";
        }
    }
    /// <summary>
    /// ��ʾMySQL�쳣
    /// </summary>
    public class MSSQLException : Exception
    {
        string _message;
        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        override public string Message
        {
            get { return _message; }
        }
        Exception _innerException;
        /// <summary>
        /// ��ȡ�ڲ��쳣
        /// </summary>
        new public Exception InnerException
        {
            get { return _innerException; }
        }
        /// <summary>
        /// ʵ��������
        /// </summary>
        /// <param name="_innerException">������ǰ�쳣���ڲ��쳣</param>
        /// <param name="message">��Ϣ</param>
        public MSSQLException(Exception innerException, string message)
        {
            _innerException = innerException;
            _message = message;
        }
    }
}