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
    public class ReporteAsesorCrud
    {
        ReportesMapper _mapper;
        ISqlDao _sqlDao;

        public ReporteAsesorCrud(ISqlDao sqlDao)
        {
            _mapper = new ReportesMapper();
            _sqlDao = sqlDao;
        }

        public List<T> RetrieveById<T>(int id)
        {
            List<T> finalResultList = new List<T>();
            ReporteAsesorResumen reporteAsesor = new ReporteAsesorResumen { Id_Usuario = id };
            SqlOperation operation = _mapper.GetReporteAsesor(reporteAsesor);
            List<Dictionary<string, object>> diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation);
            if (diccList.Count > 0)
            {
                var dtoList = diccList.Select(dicc => _mapper.BuildObjectAsesor(dicc)).ToList();
                foreach (var item in dtoList)
                {
                    finalResultList.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            return finalResultList;
        }
        public ReporteAsesorDetalle RetrieveAsesorDetalle(int id)
        {
            // Create a new instance of ReporteAsesorResumen with the provided id
            ReporteAsesorResumen reporteAsesor = new ReporteAsesorResumen { Id_Usuario = id };

            // Pass the instance of ReporteAsesorResumen to the mapper method
            SqlOperation operation = _mapper.GetReporteAsesorResumen(reporteAsesor);
            var diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation);

            if (diccList.Count > 0)
                return _mapper.BuildObjectAsesorDetalle(diccList.First());

            return null;
        }

    }

}
