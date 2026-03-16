using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DataAccess.Mappers;
using DataAccess.Mappers.Interfaces;
using DTO;

namespace DataAccess.Crud
{
    
    public class OTPCrud 
    {
        protected ISqlDao _sqlDao;
        OTPMapper _mapper; 

        
        public OTPCrud(ISqlDao sqlDao)
        {
            _mapper = new OTPMapper();
            _sqlDao = sqlDao;
        }

        public int Create(OTP_Codes pOtp)
        {
            var operation = _mapper.GetCreateStatement(pOtp);
            var result = _sqlDao.ExecuteStoreProcedureWithQuery(operation);
            if (result.Count > 0 && result[0].TryGetValue("id_otp", out var idValue)) // Se obtiene el id del OTP creado 
            {
                return Convert.ToInt32(idValue); 
            }

            return 0; 
        }

        public bool SearchOTP(int pOtp)
        {
            var operation = _mapper.GetByOTPCode(pOtp);
            var result = _sqlDao.ExecuteStoreProcedureWithQuery(operation);
            if (result.Count > 0)
            {

                int otpResult = Convert.ToInt32(result[0]["Resultado"]); // Se obtiene el resultado de la búsqueda del OTP  

                return otpResult == 1;
            }

            return false;

        }
        public bool ValidateOTP(OTP_Codes pOTP)
        {
            var operation = _mapper.GetRetrieveByOTPStatement(pOTP);
            var result = _sqlDao.ExecuteStoreProcedureWithQuery(operation);

            if (result.Count > 0)
            {
                
                int otpResult = Convert.ToInt32(result[0]["Resultado"]);

                return otpResult == 1;
            }

            return false; 
        }


    }
}
