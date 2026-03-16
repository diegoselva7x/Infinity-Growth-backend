using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;
using System.Collections.Generic;

namespace DataAccess.Crud
{
    public class RelacionAsesorClienteCrud
    {
        private readonly ReportesMapper _mapper = new ReportesMapper();
        private readonly AsignarAsesorClienteMapper _mapperAsignarAsesor = new AsignarAsesorClienteMapper();
        private readonly ISqlDao _sqlDao;

        public RelacionAsesorClienteCrud(ISqlDao sqlDao)
        {
            _sqlDao = sqlDao;
        }

        public List<int> GetClientesByAsesor(int idAsesor)
        {
            SqlOperation operation = new SqlOperation
            {
                ProcedureName = "SP_ObtenerClientesPorAsesor"
            };
            operation.AddIntParam("id_asesor", idAsesor);

            List<Dictionary<string, object>> results = _sqlDao.ExecuteStoreProcedureWithQuery(operation);

            List<int> clientes = new List<int>();
            foreach (var row in results)
            {
                clientes.Add(int.Parse(row["id_cliente"].ToString()));
            }

            return clientes;
        }
        public string AsignarAsesorACliente(int idAsesor, int idCliente)
        {
            try
            {
                var operation = _mapperAsignarAsesor.GetAsignacionAsesorOperacion(idAsesor, idCliente);
                _sqlDao.ExecuteStoreProcedure(operation);
                return "Asignación realizada correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
