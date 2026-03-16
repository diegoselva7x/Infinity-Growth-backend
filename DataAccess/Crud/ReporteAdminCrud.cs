using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;

namespace DataAccess.Crud
{
    public class ReporteAdminCrud
    {
        private readonly ReportesMapper _mapper;
        private readonly ISqlDao _sqlDao;

        public ReporteAdminCrud(ISqlDao sqlDao)
        {
            _mapper = new ReportesMapper();
            _sqlDao = sqlDao;
        }

        public List<ReporteAdminResumen> RetrieveResumenFinanciero()
        {
            List<ReporteAdminResumen> finalResultList = new List<ReporteAdminResumen>();
            SqlOperation operation = _mapper.GetResumenFinancieroAdmin();

            var diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation);
            if (diccList.Count > 0)
            {
                var dtoList = diccList.Select(d => _mapper.BuildObjectAdminResumen(d)).ToList();
                finalResultList.AddRange(dtoList);
            }

            return finalResultList;
        }

        public List<ReporteAdminDetalle> RetrieveDetalleClientes()
        {
            List<ReporteAdminDetalle> finalResultList = new List<ReporteAdminDetalle>();
            SqlOperation operation = _mapper.GetDetalleClientesAdmin();

            var diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation);
            if (diccList.Count > 0)
            {
                var dtoList = diccList.Select(d => _mapper.BuildObjectAdminDetalle(d)).ToList();
                finalResultList.AddRange(dtoList);
            }

            return finalResultList;
        }
    }
}
