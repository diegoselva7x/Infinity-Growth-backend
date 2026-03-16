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
    public class ReporteClienteCrud
    {
        ReportesMapper _mapper;
        ISqlDao _sqlDao;
        public ReporteClienteCrud(ISqlDao sqlDao)
        {
            _mapper = new ReportesMapper();
            _sqlDao = sqlDao;
        }
        public List<T> RetrieveById<T>(int id)
        {
            List<T> finalResultList = new List<T>();
            ReporteClienteResumen reporteCliente = new ReporteClienteResumen { Id_Usuario = id };
            SqlOperation operation = _mapper.GetReporteCliente(reporteCliente);
            List<Dictionary<string, object>> diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation);
            if (diccList.Count > 0)
            {
                var dtoList = diccList.Select(dicc => _mapper.BuildObjectCliente(dicc)).ToList();
                foreach (var item in dtoList)
                {
                    finalResultList.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            return finalResultList;
        }
        public ReporteClienteDetalle RetrieveClienteDetalle(int id)
        {
            // Create an instance of ReporteClienteResumen with the provided id
            ReporteClienteResumen reporteCliente = new ReporteClienteResumen { Id_Usuario = id };

            // Pass the instance of ReporteClienteResumen to the mapper method
            SqlOperation operation = _mapper.GetReporteClienteDetalle(reporteCliente);

            var diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation);

            if (diccList.Count > 0)
                return _mapper.BuildObjectClienteDetalle(diccList.First());

            return null;
        }

    }
}
