using DataAccess.Dao;

namespace DataAccess.Mappers
{
    public class RecuperarPasswordMapper
    {
        public SqlOperation GetUpdatePasswordStatement(string correo, string password)
        {
            SqlOperation operation = new SqlOperation();

            operation.ProcedureName = "SP_UPDATE_PASSWORD";
            operation.AddVarcharParam("correo", correo);
            operation.AddVarcharParam("newPassword", password);

            return operation;
        }
    }
}
