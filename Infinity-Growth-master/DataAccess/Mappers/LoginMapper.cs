using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DataAccess.Mappers.Interfaces;
using DTO;

namespace DataAccess.Mappers
{
    class LoginMapper : ICrudStatements
    {
        public Usuarios BuildObject(Dictionary<string, object> row)
        {
            Usuarios usuario = new Usuarios();

            usuario.Id = int.Parse(row["id_usuario"].ToString());
            usuario.Correo = row["tv_correo"].ToString();
            usuario.Password = row["tv_password"].ToString();
            usuario.TipoUsuario = int.Parse(row["id_tipoUsuario"].ToString());
            usuario.Estado = int.Parse(row["id_estado"].ToString());
            usuario.Nombre = row["tv_nombre"].ToString();
            usuario.Apellido1 = row["tv_apellido1"].ToString();
            usuario.Apellido2 = row["tv_apellido2"].ToString();
            usuario.FechaNacimiento = (DateTime)row["tf_fechaNacimiento"];
            usuario.Direccion = row["tv_direccion"].ToString();
            usuario.ProfileFoto = row["bt_profileFoto"] != DBNull.Value
                ? (byte[])row["bt_profileFoto"]
                : new byte[0];
            usuario.isPasswordTemp = Convert.ToInt32(row["tb_isPassWordTemp"]) == 1;

            return usuario;
        }

        public SqlOperation GetRetrieveByCorreo(Usuarios usuario)
        {
            SqlOperation operation = new SqlOperation();

            operation.ProcedureName = "SP_GET_USUARIO_BY_CORREO";
            operation.AddVarcharParam("correo", usuario.Correo);

            return operation;
        }

        public SqlOperation GetRetrieveById(Usuarios usuario)
        {
            SqlOperation operation = new SqlOperation();

            operation.ProcedureName = "SP_GET_USUARIO_BY_ID";
            operation.AddIntParam("id_usuario", usuario.Id);

            return operation;
        }
        public SqlOperation GetCreateStatement(Usuarios dto)
        {
            throw new NotImplementedException();
        }

        public SqlOperation GetDeleteStatement(Usuarios dto)
        {
            throw new NotImplementedException();
        }

        public SqlOperation GetRetrieveAllStatement()
        {
            throw new NotImplementedException();
        }

        public SqlOperation GetRetrieveByIdStatement(Usuarios pId)
        {
            throw new NotImplementedException();
        }

        public SqlOperation GetUpdateStatement(Usuarios dto)
        {
            throw new NotImplementedException();
        }
    }
}
