using DataAccess.Crud;

namespace AppLogic
{
    public class RelacionAsesorClienteManager
    {
        private readonly RelacionAsesorClienteCrud _crud;

        public RelacionAsesorClienteManager(RelacionAsesorClienteCrud crud)
        {
            _crud = crud;
        }

        public string AsignarAsesorACliente(int idAsesor, int idCliente)
        {
            return _crud.AsignarAsesorACliente(idAsesor, idCliente);
        }
    }
}

