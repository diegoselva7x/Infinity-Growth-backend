using Microsoft.Data.SqlClient;

namespace DataAccess.Dao
{
    public class SqlOperation
    {
        public string ProcedureName { get; set; }
        public List<SqlParameter> Parameters { get; set; } // Change the type to SqlParameter

        public SqlOperation()
        {
            Parameters = new List<SqlParameter>(); // Change the type to SqlParameter
        }

        public void AddVarcharParam(string parameterName, string parameterValue)
        {
            Parameters.Add(new SqlParameter("@" + parameterName, parameterValue));
        }
        public void AddIntParam(string parameterName, int parameterValue)
        {
            Parameters.Add(new SqlParameter("@" + parameterName, parameterValue));
        }
        public void AddDateTimeParam(string parameterName, DateTime parameterValue)
        {
            Parameters.Add(new SqlParameter("@" + parameterName, parameterValue));

        }
        public void AddBooleanParam(string parameterName, bool parameterValue)
        {
            Parameters.Add(new SqlParameter("@" + parameterName, parameterValue));
        }
        public void AddVarBinaryParam(string parameterName, byte[] parameterValue)
        {
            Parameters.Add(new SqlParameter("@" + parameterName, parameterValue));
        }
        public void AddDecimalParam(string parameterName, decimal parameterValue)
        {
            Parameters.Add(new SqlParameter("@" + parameterName, parameterValue));
        }
        public void AddNullParam(string paramName)
        {
            Parameters.Add(new SqlParameter(paramName, DBNull.Value));
        }

    }
}
