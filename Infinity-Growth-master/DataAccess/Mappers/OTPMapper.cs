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
    class OTPMapper 
    {

        public SqlOperation GetCreateStatement(OTP_Codes pOTP)
        {
            SqlOperation sqlOperation = new SqlOperation { ProcedureName = "SP_CREATE_OTP" };
            sqlOperation.AddIntParam("id_usuario", pOTP.Id_usuario);
            sqlOperation.AddIntParam("codigo", pOTP.Codigo);
            sqlOperation.AddDateTimeParam("fechaEnvio", pOTP.FechaEnvio);
            sqlOperation.AddDateTimeParam("fechaVencimiento", pOTP.FechaVencimiento);
            sqlOperation.AddBooleanParam("activo", pOTP.Activo);
            return sqlOperation;
        }

        //search by OTP
        public SqlOperation GetByOTPCode(int pOtp)
        {
            SqlOperation operation = new SqlOperation();
            operation.ProcedureName = "SP_SEARCH_OTP";
            operation.AddIntParam("codigo", pOtp);
            return operation;
        }

        public SqlOperation GetRetrieveByOTPStatement(OTP_Codes pOTP)
        {
            SqlOperation operation = new SqlOperation();

            operation.ProcedureName = "SP_VALIDATE_OTP";
            operation.AddIntParam("id_usuario", pOTP.Id_usuario);
            operation.AddIntParam("codigo", pOTP.Codigo);
           
            return operation;
        }
    }
}
