using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DTO;

namespace DataAccess.Mappers
{
    public class AsignarAsesorClienteMapper
    {
        public SqlOperation GetAsignacionAsesorOperacion(int idAsesor, int idCliente)
        {
            var operation = new SqlOperation
            {
                ProcedureName = "SP_AsignarActualizarAsesorACliente"
            };

            operation.AddIntParam("id_asesor", idAsesor);
            operation.AddIntParam("id_cliente", idCliente);

            return operation;
        }
    }
}
