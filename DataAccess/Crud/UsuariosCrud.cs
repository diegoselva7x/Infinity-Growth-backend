using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;
using System;
using System.Collections.Generic;

namespace DataAccess.Crud
{
    public class UsuariosCrud
    {
        private readonly ISqlDao dao;

        public UsuariosCrud(ISqlDao dao)
        {
            this.dao = dao;
        }

        public List<int> ObtenerIdsClientes()
        {
            var operation = new SqlOperation { ProcedureName = "SP_ObtenerIdsClientes" };

            var results = dao.ExecuteStoreProcedureWithQuery(operation);
            List<int> clientes = new List<int>();

            foreach (var row in results)
            {
                clientes.Add(int.Parse(row["id_usuario"].ToString()));
            }

            return clientes;
        }

        // Obtener todos los usuarios
        public List<UsuarioReporte> RetrieveAllUsuarios()
        {
            var mapper = new UsuariosMapper();
            var lstUsuarios = new List<UsuarioReporte>();

            var resultado = dao.ExecuteStoreProcedureWithQuery(mapper.GetRetrieveAllUsuarios());

            foreach (var row in resultado)
            {
                var usuario = mapper.BuildObject(row);
                lstUsuarios.Add(usuario);
            }

            return lstUsuarios;
        }

        // Obtener un usuario por ID
        public UsuarioReporte RetrieveUsuarioById(int idUsuario)
        {
            var operation = new SqlOperation { ProcedureName = "SP_GET_USUARIO_BY_ID" };
            operation.AddIntParam("id_usuario", idUsuario);

            var result = dao.ExecuteStoreProcedureWithQuery(operation);
            return result.Count > 0 ? new UsuariosMapper().BuildObject(result[0]) : null;
        }

        // Crear Usuario
        public void CreateUsuario(Usuarios usuario)
        {
            var operation = new SqlOperation { ProcedureName = "SP_CREATE_USUARIO" };
            operation.AddVarcharParam("tv_nombre", usuario.Nombre);
            operation.AddVarcharParam("tv_apellido1", usuario.Apellido1);
            operation.AddVarcharParam("tv_apellido2", usuario.Apellido2);
            operation.AddIntParam("id_tipoUsuario", usuario.TipoUsuario);

            dao.ExecuteStoreProcedureWithQuery(operation);
        }
        // Actualizar Usuario
        public void UpdateUsuario(Usuarios usuario)
        {
            var operation = new SqlOperation { ProcedureName = "SP_UPDATE_USUARIO" };
            operation.AddIntParam("id_usuario", usuario.Id);
            operation.AddVarcharParam("tv_nombre", usuario.Nombre);
            operation.AddVarcharParam("tv_apellido1", usuario.Apellido1);
            operation.AddVarcharParam("tv_apellido2", usuario.Apellido2);
            operation.AddIntParam("id_tipoUsuario", usuario.TipoUsuario);

            dao.ExecuteStoreProcedureWithQuery(operation);
        }


        // Eliminar usuario
        public void DeleteUsuario(int idUsuario)
        {
            var operation = new SqlOperation { ProcedureName = "SP_DELETE_USUARIO" };
            operation.AddIntParam("id_usuario", idUsuario);

            dao.ExecuteStoreProcedureWithQuery(operation);
        }

        //Metodo para filtrar un usuario
        public List<UsuarioReporte> RetrieveUsuariosFiltrados(UsuarioReporte filtro)
        {
            var mapper = new UsuariosMapper();
            var lstUsuarios = new List<UsuarioReporte>();

            var resultado = dao.ExecuteStoreProcedureWithQuery(mapper.GetFilteredUsuarios(filtro));

            foreach (var row in resultado)
            {
                lstUsuarios.Add(mapper.BuildObject(row));
            }

            return lstUsuarios;
        }

        // Metodo para actualizar el estado de un usuario
        public void UpdateEstadoUsuario(int idUsuario, int estado)
        {
            var operation = new SqlOperation { ProcedureName = "SP_UPDATE_ESTADO_USUARIO" };

            operation.AddIntParam("tv_estado", estado);  
            operation.AddIntParam("id_usuario", idUsuario);

            dao.ExecuteStoreProcedureWithQuery(operation);
        }

        // Metodo para actualizar el tipo de usuario
        public void UpdateTipoUsuario(int idUsuario, int tipoUsuario)
        {
            var operation = new SqlOperation { ProcedureName = "SP_UPDATE_TIPO_USUARIO" };

            operation.AddIntParam("id_usuario", idUsuario);
            operation.AddIntParam("id_tipoUsuario", tipoUsuario); 

            dao.ExecuteStoreProcedureWithQuery(operation);
        }

        public List<TipoUsuario> ObtenerTiposUsuarios()
        {
            var operation = new SqlOperation { ProcedureName = "SP_GET_Tipo_Usuarios" };
            var result = dao.ExecuteStoreProcedureWithQuery(operation);

            List<TipoUsuario> lista = new List<TipoUsuario>();
            foreach (var row in result)
            {
                lista.Add(new TipoUsuario
                {
                    Id_tipo_usuario = int.Parse(row["id_tipoUsuario"].ToString()),
                    Rol = row["tv_rol"].ToString()
                });
            }
            return lista;
        }

        public List<EstadoUsuario> ObtenerEstadosUsuarios()
        {
            var operation = new SqlOperation { ProcedureName = "SP_GET_Estado_Usuarios" };
            var result = dao.ExecuteStoreProcedureWithQuery(operation);

            List<EstadoUsuario> lista = new List<EstadoUsuario>();
            foreach (var row in result)
            {
                lista.Add(new EstadoUsuario
                {
                    id_estado = int.Parse(row["id_estado"].ToString()),
                    descripcion = row["tv_descripcion"].ToString()
                });
            }
            return lista;
        }

    }
}
