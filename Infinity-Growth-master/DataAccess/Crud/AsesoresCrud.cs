using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;
using System.Collections.Generic;

namespace DataAccess.Crud
{
    public class AsesoresCrud
    {
        private readonly ISqlDao dao;

        public AsesoresCrud(ISqlDao dao)
        {
            this.dao = dao;
        }

        // Obtener todos los asesores
        public List<Asesor> RetrieveAllAsesores()
        {
            var mapper = new AsesoresMapper();
            var asesores = new List<Asesor>();

            var resultado = dao.ExecuteStoreProcedureWithQuery(mapper.GetRetrieveAllAsesores());

            foreach (var row in resultado)
            {
                var asesor = mapper.BuildObject(row);
                asesores.Add(asesor);
            }

            return asesores;
        }
    }
}
