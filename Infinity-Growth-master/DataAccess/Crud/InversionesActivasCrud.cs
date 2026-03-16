using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;
using Microsoft.Data.SqlClient;

namespace DataAccess.Crud
{
    public class InversionesActivasCrud
    {
        InversionesActivasMapper _mapper;
        ISqlDao _sqlDao;

        public InversionesActivasCrud(ISqlDao sqlDao)
        {
            _mapper = new InversionesActivasMapper();
            _sqlDao = sqlDao;
        }

        public  void Create(InversionesActivas dto)
        {
            try
            {
                var operation = _mapper.GetCreateStatement(dto);
                _sqlDao.ExecuteStoreProcedure(operation);
            }
            catch (SqlException ex)
            {
               System.Exception exception = new System.Exception("Error al crear la inversion activa", ex);

                throw exception;    
            }
        }

        public List<InversionesActivas> RetrieveByUserId(int userId)
        {
            List<InversionesActivas> finalResultList = new List<InversionesActivas>();
            SqlOperation operation = _mapper.GetRetrieveByUserId(userId); // me da la operacion 
            List<Dictionary<string, object>> diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation); // Ejecutamos al Dao la operacion

            if (diccList != null && diccList.Count > 0)
            {
                var dtoList = _mapper.BuildObjects(diccList); // ← usar BuildObjects para lista
                foreach (var item in dtoList)
                {
                    finalResultList.Add((InversionesActivas)Convert.ChangeType(item, typeof(InversionesActivas)));
                }
            }

            return finalResultList;
        }

        //venta Accion
        public void SellInversion(VenderAccion dto)
        {
            try
            {
                var operation = _mapper.GetSellStatement(dto);
                _sqlDao.ExecuteStoreProcedure(operation);
            }
            catch (SqlException ex)
            {
                System.Exception exception = new System.Exception("Error al crear al vender la acción", ex);
                throw exception;
            }
        }

    }
}