using Microsoft.AspNetCore.Mvc;
using DTO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using AppLogic;

namespace API.Controllers
{
    [EnableCors("AllowAll")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosManager manager;

        public UsuariosController(UsuariosManager manager)
        {
            this.manager = manager;
        }

        // Método para obtener todos los usuarios con ApiResponse
        [HttpGet]
        [Route("GetAllUsuarios")]
        public API_Response GetAllUsuarios()
        {
            var response = new API_Response();

            try
            {
                var usuarios = manager.GetAllUsuarios();
                response.Result = "OK";
                response.Data = usuarios;
                response.Message = usuarios.Count > 0 ? "Usuarios recuperados correctamente" : "No hay usuarios registrados.";

            }
            catch (Exception ex)
            {
                response.Message = $"Error al recuperar usuarios: {ex.Message}";
                response.Result = "ERROR";
            }

            return response;
        }

        // Metodo para filtrar un usuario

        [HttpPost]
        [Route("FiltrarUsuarios")]
        public API_Response FiltrarUsuarios([FromBody] UsuarioReporte filtro)
        {
            var response = new API_Response();

            try
            {
                var usuarios = manager.FiltrarUsuarios(filtro);
                response.Result = "OK";
                response.Data = usuarios;
                response.Message = usuarios.Count > 0 ? "Usuarios filtrados correctamente" : "No se encontraron coincidencias.";
            }
            catch (Exception ex)
            {
                response.Message = $"Error al filtrar usuarios: {ex.Message}";
                response.Result = "ERROR";
            }

            return response;
        }

        // Metodo para actualizar el estado de un usuario

        [HttpPut]
        [Route("ActualizarEstadoUsuario")]
        public API_Response ActualizarEstadoUsuario([FromBody] UsuarioEstadoUpdateDTO usuarioEstado)
        {
            var response = new API_Response();

            try
            {
                manager.UpdateEstado(usuarioEstado.IdUsuario, usuarioEstado.Estado);
                response.Result = "OK";
                response.Message = "Estado del usuario actualizado correctamente";
            }
            catch (Exception ex)
            {
                response.Message = $"Error al actualizar el estado del usuario: {ex.Message}";
                response.Result = "ERROR";
            }

            return response;
        }



        // Metodo para actualizar el tipo de usuario


        [HttpPut]
        [Route("ActualizarTipoUsuario")]
        public API_Response ActualizarTipoUsuario([FromBody] UsuarioTipoUpdateDTO usuarioTipo)
        {
            var response = new API_Response();

            try
            {
                
                manager.UpdateTipoUsuario(usuarioTipo.IdUsuario, usuarioTipo.TipoUsuario);
                response.Result = "OK";
                response.Message = "Tipo de usuario actualizado correctamente";
            }
            catch (Exception ex)
            {
                response.Message = $"Error al actualizar el tipo de usuario: {ex.Message}";
                response.Result = "ERROR";
            }

            return response;
        }


        [HttpGet("ObtenerTiposUsuarios")]
        public API_Response ObtenerTiposUsuarios()
        {
            var data = manager.ObtenerTiposUsuarios();
            return new API_Response
            {
                Result = data.Count > 0 ? "OK" : "ERROR",
                Data = data,
                Message = data.Count > 0 ? "Tipos de usuario cargados correctamente." : "No se encontraron tipos de usuario."
            };
        }

        [HttpGet("ObtenerEstadosUsuarios")]
        public API_Response ObtenerEstadosUsuarios()
        {
            var data = manager.ObtenerEstadosUsuarios();
            return new API_Response
            {
                Result = data.Count > 0 ? "OK" : "ERROR",
                Data = data,
                Message = data.Count > 0 ? "Estados de usuario cargados correctamente." : "No se encontraron estados de usuario."
            };
        }

    }
}
