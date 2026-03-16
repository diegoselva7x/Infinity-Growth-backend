using DTO;

namespace DataAccess.Mappers.Interfaces
{
    public interface IObjectMapper
    {
        Usuarios BuildObject(Dictionary<string, object> row);
        List<Usuarios> BuildObjects(List<Dictionary<string, object>> listRows);
    }
}
