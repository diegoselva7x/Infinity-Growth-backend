using DataAccess.Crud;
using DTO;
using System.Collections.Generic;

namespace AppLogic
{
    public class AsesoresManager
    {
        private readonly AsesoresCrud _crud;

        public AsesoresManager(AsesoresCrud crud)
        {
            _crud = crud;
        }

        public List<Asesor> ObtenerTodos()
        {
            return _crud.RetrieveAllAsesores();
        }
    }
}
