using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Common.Utilities
{
    public class CreateSqlParameter
    {
        // Create_Param
        public SqlParameter Create_Param(string Param_Name, int Param_Value, SqlDbType sqldbtype, int Size, ParameterDirection P_Direction = ParameterDirection.Input)
        {
            SqlParameter NewParam = new SqlParameter();
            NewParam.ParameterName = Param_Name;
            NewParam.Value = Param_Value;
            NewParam.SqlDbType = sqldbtype;
            NewParam.Size = Size;
            NewParam.Direction = P_Direction;
            NewParam.IsNullable = false;
            return NewParam;
        }
        public SqlParameter Create_Param(string Param_Name, decimal Param_Value, SqlDbType sqldbtype, int Size, ParameterDirection P_Direction = ParameterDirection.Input)
        {
            SqlParameter NewParam = new SqlParameter();
            NewParam.ParameterName = Param_Name;
            NewParam.Value = Param_Value;
            NewParam.SqlDbType = sqldbtype;
            NewParam.Size = Size;
            NewParam.Direction = P_Direction;
            NewParam.IsNullable = false;
            return NewParam;
        }
        public SqlParameter Create_Param(string Param_Name, string Param_Value, SqlDbType sqldbtype, int Size, ParameterDirection P_Direction = ParameterDirection.Input)
        {
            SqlParameter NewParam = new SqlParameter();
            NewParam.ParameterName = Param_Name;
            NewParam.Value = Param_Value;
            NewParam.SqlDbType = sqldbtype;
            NewParam.Size = Size;
            NewParam.Direction = P_Direction;
            NewParam.IsNullable = false;
            return NewParam;
        }
        public SqlParameter Create_Param(string Param_Name, Guid Param_Value, SqlDbType sqldbtype, int Size, ParameterDirection P_Direction = ParameterDirection.Input)
        {
            SqlParameter NewParam = new SqlParameter();
            NewParam.ParameterName = Param_Name;
            NewParam.Value = Param_Value;
            NewParam.SqlDbType = sqldbtype;
            NewParam.Size = Size;
            NewParam.Direction = P_Direction;
            NewParam.IsNullable = false;
            return NewParam;
        }
        public SqlParameter Create_Param(string Param_Name, bool Param_Value, SqlDbType sqldbtype, int Size, ParameterDirection P_Direction = ParameterDirection.Input)
        {
            SqlParameter NewParam = new SqlParameter();
            NewParam.ParameterName = Param_Name;
            NewParam.Value = Param_Value;
            NewParam.SqlDbType = sqldbtype;
            NewParam.Size = Size;
            NewParam.Direction = P_Direction;
            NewParam.IsNullable = false;
            return NewParam;
        }
        public SqlParameter Create_Param(string Param_Name, byte[] Param_Value, SqlDbType sqldbtype, int Size)
        {
            SqlParameter NewParam = new SqlParameter();
            NewParam.ParameterName = Param_Name;
            NewParam.Value = Param_Value;
            NewParam.SqlDbType = sqldbtype;
            NewParam.Size = Size;
            NewParam.IsNullable = false;
            return NewParam;
        }
        public SqlParameter Create_Param(string Param_Name, DataTable Param_Value, ParameterDirection P_Direction = ParameterDirection.Input)
        {
            SqlParameter NewParam = new SqlParameter();
            NewParam.ParameterName = Param_Name;
            NewParam.Value = Param_Value;
            NewParam.Direction = P_Direction;
            NewParam.IsNullable = false;
            return NewParam;
        }
    }
}
