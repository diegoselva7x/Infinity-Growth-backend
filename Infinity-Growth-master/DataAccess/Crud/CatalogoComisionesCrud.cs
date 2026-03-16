
using DataAccess.Dao;
using DataAccess.Mappers; 
using DTO;
using System.Collections.Generic;
using System.Data;
using DataAccess.Mappers;
using Microsoft.Data.SqlClient; 

namespace InfinityGrowth.DataAccess.Crud 
{
    public class CatalogoComisionesCrud
    {
        private readonly ISqlDao _dao;
        private readonly AjusteComisionesMapper _mapper;

        
        public CatalogoComisionesCrud(ISqlDao dao, AjusteComisionesMapper mapper)
        {
            _dao = dao;
            _mapper = mapper;
        }

       
        public List<AjusteComisiones> RetrieveAll()
        {
            var operation = new SqlOperation { ProcedureName = "SP_GET_ALL_CATALOGO_COMISIONES" };

            // Ejecutar el SP que devuelve datos
            var result = _dao.ExecuteStoreProcedureWithQuery(operation);

            // Mapear el resultado (List<Dictionary<string, object>>) a una lista de DTOs
            return _mapper.BuildObjects(result).Cast<AjusteComisiones>().ToList();
        }

        // Método SÍNCRONO para actualizar el porcentaje de una comisión
        public void UpdatePorcentaje(int idTipoComision, decimal porcentaje)
        {
            var operation = new SqlOperation { ProcedureName = "SP_UPDATE_CATALOGO_COMISION_PORCENTAJE" };

            // Agregar los parámetros
            operation.Parameters.Add(new SqlParameter("@id_Tipocomision", SqlDbType.Int) { Value = idTipoComision });
            operation.Parameters.Add(new SqlParameter("@tp_porcentaje", SqlDbType.Decimal)
            {
                Value = porcentaje,
                Precision = 5, // Asegúrate que coincida con la definición de la tabla/SP (decimal(5,2))
                Scale = 2
            });

            // Ejecutar el SP de acción (no devuelve datos)
            _dao.ExecuteStoreProcedure(operation);
        }

       
    }
}