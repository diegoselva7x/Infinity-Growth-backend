using DataAccess.Crud;
using DTO;

namespace AppLogic
{
    public class UsuariosManager
    {
        private readonly UsuariosCrud _crud;

        public UsuariosManager(UsuariosCrud crud)
        {
            _crud = crud;
        }

        // Método para obtener todos los usuarios
        public List<UsuarioReporte> GetAllUsuarios()
        {
            try
            {
                return _crud.RetrieveAllUsuarios();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al recuperar los usuarios", ex);
            }
        }

        // Método para obtener un usuario por ID
        public UsuarioReporte GetUsuarioById(int idUsuario)
        {
            try
            {
                return _crud.RetrieveUsuarioById(idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al recuperar el usuario con ID {idUsuario}", ex);
            }
        }

        // Método para agregar un usuario
        public void CreateUsuario(Usuarios usuario)
        {
            try
            {
                _crud.CreateUsuario(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el usuario", ex);
            }
        }

        // Método para actualizar un usuario
        public void UpdateUsuario(Usuarios usuario)
        {
            try
            {
                _crud.UpdateUsuario(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el usuario", ex);
            }
        }

        // Método para eliminar un usuario
        public void DeleteUsuario(int idUsuario)
        {
            try
            {
                _crud.DeleteUsuario(idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el usuario con ID {idUsuario}", ex);
            }
        }

        // Metodo para filtrar un usuario
        public List<UsuarioReporte> FiltrarUsuarios(UsuarioReporte filtro)
        {
            try
            {
                return _crud.RetrieveUsuariosFiltrados(filtro);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al filtrar los usuarios", ex);
            }
        }

        // Metodo para actualizar el estado de un usuario
        public void UpdateEstado(int idUsuario, int estado)
        {
            try
            {
                _crud.UpdateEstadoUsuario(idUsuario, estado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el estado del usuario", ex);
            }
        }

        // Metodo para actualizar el tipo de usuario
        public void UpdateTipoUsuario(int idUsuario, int tipoUsuario)
        {
            try
            {
                _crud.UpdateTipoUsuario(idUsuario, tipoUsuario);  
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el tipo de usuario", ex);
            }
        }

        public List<TipoUsuario> ObtenerTiposUsuarios()
        {
            return _crud.ObtenerTiposUsuarios();
        }

        public List<EstadoUsuario> ObtenerEstadosUsuarios()
        {
            return _crud.ObtenerEstadosUsuarios();
        }

    }
}
