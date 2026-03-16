using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;

namespace DataAccess.Crud
{
    public class PortafoliosCrud
    {
        PortafoliosMapper _mapper;
        ISqlDao _sqlDao;

        public PortafoliosCrud(ISqlDao sqlDao)
        {
            _mapper = new PortafoliosMapper();
            _sqlDao = sqlDao;
        }

        public List<T> RetrieveById<T>(Portafolios dto)
        {
            List<T> finalResultList = new List<T>();
            SqlOperation operation = _mapper.GetRetrieveById(dto); // me da la operacion 
            List<Dictionary<string, object>> diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation); // Ejecutamos al Dao la operacion

            if (diccList != null && diccList.Count > 0)
            {
                var dtoList = _mapper.BuildObjects(diccList); // ← usar BuildObjects para lista
                foreach (var item in dtoList)
                {
                    finalResultList.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }

            return finalResultList;
        }

    }
}
