using System.Collections.Generic;

namespace DataAccess.Dao
{
    public interface ISqlDao
    {
        void ExecuteStoreProcedure(SqlOperation pOperation);
        List<Dictionary<string, object>> ExecuteStoreProcedureWithQuery(SqlOperation pOperation);
    }
}

