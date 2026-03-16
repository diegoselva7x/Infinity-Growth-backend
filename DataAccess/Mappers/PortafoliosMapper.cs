using DataAccess.Dao;
using DTO;

public class PortafoliosMapper
{
    public Portafolios BuildObject(Dictionary<string, object> row)
    {
        Portafolios portafolios = new Portafolios();

        portafolios.Id = int.Parse(row["id_portafolio"].ToString());
        portafolios.Usuario = int.Parse(row["id_usuario"].ToString());
        portafolios.Activo = int.Parse(row["id_activo"].ToString());
        portafolios.SaldoCuenta = decimal.Parse(row["tm_saldoCuenta"].ToString());

        return portafolios;
    }

    public List<Portafolios> BuildObjects(List<Dictionary<string, object>> rows)
    {
        List<Portafolios> result = new List<Portafolios>();
        foreach (var row in rows)
        {
            result.Add(BuildObject(row));
        }
        return result;
    }

    public SqlOperation GetRetrieveById(Portafolios portafolios)
    {
        SqlOperation operation = new SqlOperation();
        operation.ProcedureName = "SP_GET_PORTAFOLIOS_BY_ID";
        operation.AddIntParam("idUsuario", portafolios.Usuario);
        return operation;
    }
}
