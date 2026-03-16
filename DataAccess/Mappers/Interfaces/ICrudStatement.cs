using DataAccess.Dao;
using DTO;

namespace DataAccess.Mappers.Interfaces
{
    public interface ICrudStatements
    {
        SqlOperation GetCreateStatement(Usuarios dto);   //create
        SqlOperation GetUpdateStatement(Usuarios dto);  //update
        SqlOperation GetDeleteStatement(Usuarios dto);  //delet8 e
        SqlOperation GetRetrieveAllStatement();  //read
        SqlOperation GetRetrieveByIdStatement(Usuarios pId);  //read
    }
    
}
