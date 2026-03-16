using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Dao
{
    public class SqlDao : ISqlDao
    {
        private readonly string _connectionString;
        
        public SqlDao(IConfiguration configuration)
        {
            // IConfiguration already includes environment variables in ASP.NET Core.
            // We also explicitly check Environment as a fallback for non-standard hosts.
            _connectionString =
                GetFirstNonEmpty(
                    configuration["IG_DB_CONNECTIONSTRING"],
                    configuration["Azure:ConnectionStrings:DefaultConnectionDB"],
                    configuration["ConnectionStrings:DefaultConnectionDB"],
                    Environment.GetEnvironmentVariable("IG_DB_CONNECTIONSTRING"),
                    Environment.GetEnvironmentVariable("Azure__ConnectionStrings__DefaultConnectionDB"),
                    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnectionDB")
                )
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured. Set IG_DB_CONNECTIONSTRING (preferred) or Azure__ConnectionStrings__DefaultConnectionDB / ConnectionStrings__DefaultConnectionDB."
                );
        }

        private static string? GetFirstNonEmpty(params string?[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return null;
        }

        // ExecuteStoreProcedure CREATE, UPDATE, DELETE
        public void ExecuteStoreProcedure(SqlOperation pOperation)
        {
            try
            {
                var connection = new SqlConnection(_connectionString);
                var command = connection.CreateCommand();

                command.Connection = connection;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = pOperation.ProcedureName;

                foreach (var item in pOperation.Parameters)
                {
                    command.Parameters.Add(item);
                }

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // ExecuteStoreProcedure SELECT
        public List<Dictionary<string, object>> ExecuteStoreProcedureWithQuery(SqlOperation pOperation)
        {
            try
            {
                var listResult = new List<Dictionary<string, object>>();

                var connection = new SqlConnection(_connectionString);
                var command = connection.CreateCommand();

                command.Connection = connection;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = pOperation.ProcedureName;

                foreach (var item in pOperation.Parameters)
                {
                    command.Parameters.Add(item);
                }

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        listResult.Add(row);
                    }
                }

                connection.Close();
                return listResult;
            }
            catch (Exception ex)
            {
                throw;
            }

           
        }


    }
}
