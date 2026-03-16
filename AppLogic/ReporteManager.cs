using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Crud;
using DTO;

namespace AppLogic
{
    public interface IReporteManager
    {
        // CLIENTE
        List<ReporteClienteResumen> ObtenerTransaccionesPorUsuario(int id);
        ReporteClienteDetalle ObtenerClienteDetalle(int id);

        // ASESOR
        List<ReporteAsesorResumen> ObtenerReporteAsesor(int id);
        ReporteAsesorDetalle ObtenerAsesorDetalle(int id);

        // ADMIN
        List<ReporteAdminResumen> ObtenerResumenFinancieroAdmin();
        List<ReporteAdminDetalle> ObtenerDetalleClientesAdmin();
    }




    public class ReporteManager : IReporteManager
    {
        private readonly ReporteClienteCrud _reporteClienteCrud;
        private readonly ReporteAsesorCrud _reporteAsesorCrud;
        private readonly ReporteAdminCrud _reporteAdminCrud;

        public ReporteManager(ReporteClienteCrud reporteClienteCrud, ReporteAsesorCrud reporteAsesorCrud, ReporteAdminCrud reporteAdminCrud)
        {
            _reporteClienteCrud = reporteClienteCrud;
            _reporteAsesorCrud = reporteAsesorCrud;
            _reporteAdminCrud = reporteAdminCrud;
        }
        // CLIENTE - Resumen financiero
        public List<ReporteClienteResumen> ObtenerTransaccionesPorUsuario(int id)
        {
            return _reporteClienteCrud.RetrieveById<ReporteClienteResumen>(id);
        }

        public ReporteClienteDetalle ObtenerClienteDetalle(int id)
        {
            return _reporteClienteCrud.RetrieveClienteDetalle(id);
        }

        // ASESOR - Resumen financiero global
        public List<ReporteAsesorResumen> ObtenerReporteAsesor(int id)
        {
            return _reporteAsesorCrud.RetrieveById<ReporteAsesorResumen>(id);
        }

        public ReporteAsesorDetalle ObtenerAsesorDetalle(int id)
        {
            return _reporteAsesorCrud.RetrieveAsesorDetalle(id);
        }

        // ADMIN - Resumen financiero
        public List<ReporteAdminResumen> ObtenerResumenFinancieroAdmin()
        {
            return _reporteAdminCrud.RetrieveResumenFinanciero();
        }

        public List<ReporteAdminDetalle> ObtenerDetalleClientesAdmin()
        {
            return _reporteAdminCrud.RetrieveDetalleClientes();
        }
    }
}
