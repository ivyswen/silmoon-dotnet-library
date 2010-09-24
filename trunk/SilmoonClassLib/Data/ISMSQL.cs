using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Silmoon.Data
{
    public interface ISMSQL
    {
        void Open();
        void Close();
        int ExecNonQuery(string sqlCommand);
        int GetRecordCount(string sqlcommand);
        object GetDataReader(string sqlcommand);
        object GetCommand(string sqlcommand);
        object GetDataAdapter(string sqlcommand);
        DataTable GetDataTable(string sqlcommand);
        object GetFieldObjectForSingleQuery(string tablename, string resulefield, string fieldname, string fieldvalue);
        object GetFieldObjectForSingleQuery(string sqlcommand);
        int UpdateFieldForSingleQuery(string tablename, string updatefield, string updatevalue, string fieldname, string fieldvalue);
        bool ExistRecord(string sqlcommand);
        string ExistRecord(string sqlcommand, string fieldname);
        object GetConnection();
        string __chkSqlstr(string sqlcommand);
    }
}
