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
   /*public class TransaccionesMapper 
    {
        public BaseClass BuildObject(Dictionary<string, object> row)
        {
            Transacciones transaccion = new Transacciones();

            transaccion.Id = int.Parse(row["id_transaccion"].ToString());
            transaccion.Usuario = int.Parse(row["id_usuario"].ToString());
            transaccion.Activo = int.Parse(row["id_activo"].ToString());
            transaccion.Tipo = bool.Parse(row["tb_tipoTransaccion"].ToString());
            transaccion.Fecha = (DateTime)row["tf_fecha"];
            transaccion.Cantidad = decimal.Parse(row["td_cantidad"].ToString());

            return transaccion;
        }
        public SqlOperation GetCreateStatement(BaseClass dto)
        {
            var transaccion = (Transacciones)dto;

            var operation = new SqlOperation();
            operation.ProcedureName = "SP_INSERT_TRANSACCION";

            operation.AddIntParam("id_transaccion", transaccion.Id);
            operation.AddIntParam("id_usuario", transaccion.Usuario);
            operation.AddIntParam("activo", transaccion.Activo);
            operation.AddBooleanParam("tipo", transaccion.Tipo);
            operation.AddDateTimeParam("fecha", transaccion.Fecha);
            operation.AddDecimalParam("cantidad", transaccion.Cantidad);
            
            return operation;
        }*/
    
    }

