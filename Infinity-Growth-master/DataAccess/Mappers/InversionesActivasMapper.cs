using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DTO;

namespace DataAccess.Mappers
{
    public class InversionesActivasMapper
    {
        public InversionesActivas BuildObject(Dictionary<string, object> row)
        {
            InversionesActivas inversion = new InversionesActivas();
            
            inversion.Id = int.Parse(row["id_activo"].ToString());
            inversion.Ticker = row["tv_ticker"].ToString();
            inversion.Nombre = row["tv_nombre"].ToString();
            inversion.PrecioCompra = decimal.Parse(row["tm_precioCompra"].ToString());
            inversion.PrecioVenta = decimal.Parse(row["tm_precioVenta"].ToString());
            inversion.Activo = bool.Parse(row["tb_activo"].ToString());
            inversion.Id_usuario = int.Parse(row["id_usuario"].ToString());
            inversion.Cantidad = decimal.Parse(row["td_cantidad"].ToString());
            inversion.PrecioTotal = decimal.Parse(row["PrecioTotal"].ToString());


            return inversion;
        }

        public List<InversionesActivas> BuildObjects(List<Dictionary<string, object>> rows)
        {
            var list = new List<InversionesActivas>();
            foreach (var row in rows)
            {
                list.Add(BuildObject(row));
            }
            return list;
        }

        public SqlOperation GetCreateStatement(InversionesActivas dto)
        {

            var inversion = (InversionesActivas)dto;

            var operation = new SqlOperation();
            operation.ProcedureName = "SP_INSERT_INVERSION";

            operation.AddVarcharParam("tv_ticker", inversion.Ticker);
            operation.AddVarcharParam("tv_nombre", inversion.Nombre);
            operation.AddDecimalParam("tm_precioCompra", inversion.PrecioCompra);
            operation.AddDecimalParam("tm_precioVenta", inversion.PrecioVenta);
            operation.AddBooleanParam("activo", inversion.Activo);
            operation.AddIntParam("id_usuario", inversion.Id_usuario);
            operation.AddDecimalParam("td_cantidad", inversion.Cantidad);

            return operation;
        }

        public SqlOperation GetRetrieveByUserId(int userId)
        {
            SqlOperation operation = new SqlOperation
            { 
                ProcedureName = "SP_GET_PORTAFOLIOS_BY_ID"
            };
            operation.AddIntParam("idUsuario", userId);
            return operation;
        }

        public SqlOperation GetSellStatement(VenderAccion dto)
        {
            var inversion = (VenderAccion)dto;
            var operation = new SqlOperation();
            operation.ProcedureName = "SP_VENDER_ACCION";
            operation.AddIntParam("IdActivo", inversion.Id);
            operation.AddDecimalParam("PrecioVenta", inversion.PrecioVenta);
            return operation;
        }
    }
}
