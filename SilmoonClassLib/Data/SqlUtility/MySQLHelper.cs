using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using Silmoon.Data;
using Silmoon.Data.Odbc;

namespace Silmoon.Data.SqlUtility
{
    /// <summary>
    /// MySQLʵ����
    /// </summary>
    public class MySQLHelper
    {
        SmOdbcClient _odbc;
        /// <summary>
        /// ʹ��ָ����ODBC�������Ӵ���MYSQLʵ�ù���
        /// </summary>
        /// <param name="odbc">ָ��һ���Ѿ�����ʹ�õ�ODBC����</param>
        public MySQLHelper(SmOdbcClient odbc)
        {
            _odbc = odbc;
        }
        /// <summary>
        /// ˢ�����ݿ����ж���
        /// </summary>
        public void Refresh()
        {
            _odbc.ExecNonQuery("FLUSH PRIVILEGES");
        }

        /// <summary>
        /// ����һ�����ݿ�
        /// </summary>
        /// <param name="database">���ݿ�����</param>
        /// <returns></returns>
        public int CreateDatabase(string database)
        {
            if (IsExistDatabase(database)) throw new MySQLException(null, "���ݿ��Ѵ���");
            return _odbc.ExecNonQuery("CREATE DATABASE " + database);
        }
        /// <summary>
        /// ɾ��һ�����ݿ�
        /// </summary>
        /// <param name="database">���ݿ�����</param>
        /// <returns></returns>
        public int DropDatabase(string database)
        {
            if (database.ToLower() == "mysql") throw new MySQLException(null, "ϵͳ���ݿ��޷�ɾ��");
            if (!IsExistDatabase(database)) throw new MySQLException(null, "ָ����һ�������ڵ����ݿ�");
            return _odbc.ExecNonQuery("DROP DATABASE " + database);
        }
        /// <summary>
        /// ���һ���û������ƶ�����������ݿ�
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="database">ָ�������ݿ�</param>
        public int AddUserToDatabase(string username, string database)
        {
            if (!IsExistDatabase(database)) throw new MySQLException(null, "ָ����һ�������ڵ����ݿ�");
            if (!IsExistUser(username)) throw new MySQLException(null, "ָ����һ�������ڵ����ݿ�");


            int result = _odbc.ExecNonQuery("GRANT ALL PRIVILEGES ON `" + database + "`.* TO '" + username + "'@'%'");
            Refresh();
            return result;
        }
        /// <summary>
        /// ����һ���û������ƶ�����������ݿ�
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="password">�û�����</param>
        /// <param name="database">ָ�������ݿ�</param>
        /// <returns></returns>
        public int CreateUserToDatabase(string username, string password, string database)
        {
            if (!IsExistDatabase(database)) throw new MySQLException(null, "ָ����һ�������ڵ����ݿ�");

            _odbc.ExecNonQuery("GRANT USAGE ON *.* TO '" + username + "'@'%' IDENTIFIED BY '" + password + "'");
            int result = _odbc.ExecNonQuery("GRANT ALL PRIVILEGES ON `" + database + "`.* TO '" + username + "'@'%'");
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

            if (IsExistUser(username)) throw new MySQLException(null, "�û����Ѿ����ڡ�");

            int result = _odbc.ExecNonQuery("GRANT USAGE ON *.* TO '" + username + "'@'%' IDENTIFIED BY '" + password + "'");
            Refresh();
            return result;
        }
        /// <summary>
        /// ʹ������ǿ��ɾ��һ���û�
        /// </summary>
        /// <param name="username">�û���</param>
        /// <returns></returns>
        public int ForceRemoveUser(string username)
        {
            if (username.ToLower() == "root") throw new MySQLException(null, "��ֹɾ��Root�û���");
            _odbc.ExecNonQuery("DELETE FROM mysql.db WHERE user = '" + username + "'");
            int result = _odbc.ExecNonQuery("DELETE FROM mysql.user WHERE user = '" + username + "'");
            Refresh();
            return result;
        }
        /// <summary>
        /// ʹ������ǿ��ɾ��һ���û�
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="host">�������ӵ�����</param>
        /// <returns></returns>
        public int ForceRemoveUser(string username, string host)
        {
            if (username.ToLower() == "root") throw new MySQLException(null, "��ֹɾ��Root�û���");
            _odbc.ExecNonQuery("DELETE FROM mysql.db WHERE user = '" + username + "'");
            int result = _odbc.ExecNonQuery("DELETE FROM mysql.user WHERE user = '" + username + "' and host = '" + host + "'");
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
            if (!IsExistDatabase(database)) throw new MySQLException(null, "ָ����һ�������ڵ����ݿ�");

            int result = _odbc.ExecNonQuery("DELETE FROM mysql.db WHERE user = '" + username + "' and db='" + database + "'");
            Refresh();
            return result;
        }
        /// <summary>
        /// �Ƴ�һ���û������ݿ��Ȩ��
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="database">���ݿ�</param>
        /// <param name="host">����</param>
        /// <returns></returns>
        public int RemoveUserGrant(string username, string database, string host)
        {
            if (!IsExistDatabase(database)) throw new MySQLException(null, "ָ����һ�������ڵ����ݿ�");

            int result = _odbc.ExecNonQuery("DELETE FROM mysql.db WHERE user = '" + username + "' and db='" + database + "' and host = '" + host + "'");
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
            DataTable dt = _odbc.GetDataTable("SHOW DATABASES");
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
            DataTable dt = _odbc.GetDataTable("select user from mysql.user");
            ArrayList _array = new ArrayList();
            foreach (DataRow row in dt.Rows) _array.Add(row[0]);
            string[] nameArr = (string[])_array.ToArray(typeof(string));
            dt.Clear();
            dt.Dispose();
            return SmString.FindFormStringArray(nameArr, username);
        }

        /// <summary>
        /// ����һ���ɰ��㷨��������û�
        /// </summary>
        /// <param name="username">Ŀ���û�</param>
        /// <param name="password">����</param>
        /// <returns></returns>
        public int SetOldPassword(string username, string password)
        {
            int i = _odbc.ExecNonQuery("UPDATE mysql.user SET PASSWORD = OLD_PASSWORD('" + password + "') WHERE USER = '" + username + "'");
            Refresh();
            return i;
        }
        /// <summary>
        /// �����û�����
        /// </summary>
        /// <param name="username">Ŀ���û�</param>
        /// <param name="password">����</param>
        /// <returns></returns>
        public int SetPassword(string username, string password)
        {
            int i = _odbc.ExecNonQuery("UPDATE mysql.user SET PASSWORD = PASSWORD('" + password + "') WHERE USER = '" + username + "'");
            Refresh();
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
        /// <summary>
        /// ����һ������MySQL ODBC 3.51��������������Դ�������ַ���
        /// </summary>
        /// <param name="odbcDriverName">ODBC����Դ��������</param>
        /// <param name="hostname">������</param>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <param name="database">���ݿ�</param>
        /// <returns></returns>
        public static string MakeConnectionString(string odbcDriverName, string hostname, string username, string password, string database)
        {
            return "DRIVER={" + odbcDriverName + "};SERVER=" + hostname + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";
        }
    }
    /// <summary>
    /// ��ʾMySQL�쳣
    /// </summary>
    public class MySQLException : Exception
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
        /// <param name="innerException">������ǰ�쳣���ڲ��쳣</param>
        /// <param name="message">��Ϣ</param>
        public MySQLException(Exception innerException, string message)
        {
            _innerException = innerException;
            _message = message;
        }
    }
}
