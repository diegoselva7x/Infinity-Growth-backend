using DataAccess.Dao;
using DTO;

namespace DataAccess.Mappers
{
    class UsuariosMapper
    {

        /* public UsuarioReporte BuildObject(Dictionary<string, object> row)
        {
            UsuarioReporte usuario = new UsuarioReporte();

            usuario.Id = int.Parse(row["Id Usuario"].ToString());
            usuario.Correo = row["Correo Electrónico"].ToString();
            usuario.Nombre = row["Nombre"].ToString();
            usuario.Apellido1 = row["Primer Apellido"].ToString();
            usuario.Apellido2 = row["Segundo Apellido"].ToString();
            usuario.TipoUsuario = row["Tipo Usuario"].ToString();
            usuario.Estado = row["Estado"].ToString();
            usuario.FechaNacimiento = DateTime.Parse(row["Fecha de Nacimiento"].ToString());
            usuario.Direccion = row["Dirección"].ToString();

            return usuario;
        }
        */

        public UsuarioReporte BuildObject(Dictionary<string, object> row)
        {
            UsuarioReporte usuario = new UsuarioReporte();

            usuario.Id = int.Parse(row["Id Usuario"].ToString());
            usuario.Correo = row["Correo Electrónico"].ToString();
            usuario.Nombre = row["Nombre"].ToString();
            usuario.Apellido1 = row["Primer Apellido"].ToString();
            usuario.Apellido2 = row["Segundo Apellido"].ToString();

           
            usuario.TipoUsuario = row["Tipo Usuario"].ToString(); 
            usuario.Estado = row["Estado"].ToString();
            usuario.FechaNacimiento = DateTime.Parse(row["Fecha de Nacimiento"].ToString());
            usuario.Direccion = row["Dirección"].ToString();
            usuario.IdAsesor = row["Id Asesor"] != DBNull.Value ? int.Parse(row["Id Asesor"].ToString()) : (int?)null;
            usuario.NombreAsesor = row["Nombre Asesor"] != DBNull.Value ? row["Nombre Asesor"].ToString() : null;

            return usuario;
        }


        public SqlOperation GetRetrieveAllUsuarios()
        {
            SqlOperation operation = new SqlOperation
            {
                ProcedureName = "SP_GET_ALL_USUARIOS"
            };
            return operation;
        }

        public SqlOperation GetFilteredUsuarios(UsuarioReporte filtro)
        {
            SqlOperation operation = new SqlOperation
            {
                ProcedureName = "SP_FILTRAR_USUARIOS"
            };

            
            if (filtro.Id != 0) operation.AddIntParam("id_usuario", filtro.Id);
            if (!string.IsNullOrEmpty(filtro.Correo)) operation.AddVarcharParam("tv_correo", filtro.Correo);
            if (!string.IsNullOrEmpty(filtro.Nombre)) operation.AddVarcharParam("tv_nombre", filtro.Nombre);
            if (!string.IsNullOrEmpty(filtro.Apellido1)) operation.AddVarcharParam("tv_apellido1", filtro.Apellido1);
            if (!string.IsNullOrEmpty(filtro.Apellido2)) operation.AddVarcharParam("tv_apellido2", filtro.Apellido2);
            if (!string.IsNullOrEmpty(filtro.TipoUsuario)) operation.AddVarcharParam("tv_rol", filtro.TipoUsuario);
            if (!string.IsNullOrEmpty(filtro.Estado)) operation.AddVarcharParam("tv_estado", filtro.Estado);

            
            if (filtro.FechaNacimiento.HasValue && filtro.FechaNacimiento.Value != DateTime.MinValue)
            {
                operation.AddDateTimeParam("tf_fechaNacimiento", filtro.FechaNacimiento.Value);
            }

            if (!string.IsNullOrEmpty(filtro.Direccion)) operation.AddVarcharParam("tv_direccion", filtro.Direccion);

            return operation;
        }
        
    }

}