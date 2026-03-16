using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DTO;

namespace DataAccess.Mappers
{
    public class ReportesMapper
    {
        public SqlOperation GetReporteCliente(ReporteClienteResumen pcliente)
        {
            SqlOperation sqlOperation = new SqlOperation { ProcedureName = "SP_ObtenerTransaccionesPorUsuario" };
            sqlOperation.AddIntParam("id_usuario", pcliente.Id_Usuario);
            return sqlOperation;
        }
        public ReporteClienteResumen BuildObjectCliente(Dictionary<string, object> row)
        {
            ReporteClienteResumen cliente = new ReporteClienteResumen();

            cliente.Fecha = DateTime.Parse(row["tf_fecha"].ToString());
            cliente.Nombre = row["NombreActivo"].ToString();
            cliente.TipoTransaccion = bool.Parse(row["TipoTransaccion"].ToString());
            cliente.Cantidad = decimal.Parse(row["Cantidad"].ToString());
            cliente.PrecioCompra = decimal.Parse(row["PrecioCompra"].ToString());
            cliente.PrecioTotal = decimal.Parse(row["PrecioTotal"].ToString());

            return cliente;
        }
        public ReporteClienteDetalle BuildObjectClienteDetalle(Dictionary<string, object> row)
        {
            ReporteClienteDetalle detalle = new ReporteClienteDetalle();

            detalle.TotalInvertido = decimal.Parse(row["TotalInvertido"].ToString());
            detalle.GananciaAcumulada = decimal.Parse(row["GananciaAcumulada"].ToString());
            detalle.PerdidaAcumulada = decimal.Parse(row["PerdidaAcumulada"].ToString());
            detalle.ComisionPagada = decimal.Parse(row["ComisionPagada"].ToString());

            // Estos valores ahora los asignás en el controller, no vienen del SP
            detalle.AccionMasRentable = "";
            detalle.AccionMenosRentable = "";

            return detalle;
        }

        public SqlOperation GetReporteClienteDetalle(ReporteClienteResumen pcliente)
        {
            SqlOperation sqlOperation = new SqlOperation { ProcedureName = "SP_ObtenerResumenClienteDetalle" };
            sqlOperation.AddIntParam("id_usuario", pcliente.Id_Usuario);
            return sqlOperation;
        }


        public ReporteAsesorResumen BuildObjectAsesor(Dictionary<string, object> row)
        {
            ReporteAsesorResumen asesor = new ReporteAsesorResumen();

            asesor.Id_Usuario = int.Parse(row["id_usuario"].ToString());
            asesor.Nombre = row["nombre_cliente"].ToString(); // ya viene concatenado
            asesor.Apellido = "";

            asesor.InversionTotal = decimal.Parse(row["td_inversionTotal"].ToString());
            asesor.Ganancias = decimal.Parse(row["td_ganancias"].ToString());
            asesor.Perdidas = decimal.Parse(row["td_perdidas"].ToString());
            asesor.ComisionGeneradaDelUsuario = decimal.Parse(row["td_comisionGeneradaDelUsuario"].ToString());
            asesor.RendimientoTotal = decimal.Parse(row["td_rendimientoTotal"].ToString());

            return asesor;
        }
        public SqlOperation GetReporteAsesorResumen(ReporteAsesorResumen pAsesor)
       {
            SqlOperation sqlOperation = new SqlOperation { ProcedureName = "SP_ObtenerResumenAsesorDetalle" };
            sqlOperation.AddIntParam("id_usuario", pAsesor.Id_Usuario);
            return sqlOperation;
        }
        public ReporteAsesorDetalle BuildObjectAsesorDetalle(Dictionary<string, object> row)
        {
            ReporteAsesorDetalle asesor = new ReporteAsesorDetalle();

            asesor.TotalInvertidoPorClientes = decimal.Parse(row["TotalInvertidoPorClientes"].ToString());
            asesor.TotalComisionesGeneradas = decimal.Parse(row["TotalComisionesGeneradas"].ToString());
            asesor.GananciasAcumuladas = decimal.Parse(row["GananciasAcumuladas"].ToString());
            asesor.PerdidasAcumuladas = decimal.Parse(row["PerdidasAcumuladas"].ToString());
            asesor.AccionMasRentable = row["AccionMasRentable"].ToString();
            asesor.AccionMenosRentable = row["AccionMenosRentable"].ToString();
            return asesor;
        }

        public SqlOperation GetReporteAsesor(ReporteAsesorResumen pasesor)
        {
            SqlOperation sqlOperation = new SqlOperation { ProcedureName = "SP_ObtenerTransaccionesPorAsesor" };
            sqlOperation.AddIntParam("id_usuario", pasesor.Id_Usuario);
            return sqlOperation;
        }
        public SqlOperation GetResumenFinancieroAdmin()
        {
            return new SqlOperation { ProcedureName = "SP_ObtenerResumenFinancieroAdmin" };
        }
        public ReporteAdminResumen BuildObjectAdminResumen(Dictionary<string, object> row)
        {
            ReporteAdminResumen admin = new ReporteAdminResumen();

            admin.TotalInvertidoPlataforma = decimal.Parse(row["TotalInvertidoPlataforma"].ToString());
            admin.ComisionPlataformaTotal = decimal.Parse(row["ComisionPlataformaTotal"].ToString());
            admin.GananciasGeneradas = decimal.Parse(row["GananciasGeneradas"].ToString());
            admin.PerdidasGeneradas = decimal.Parse(row["PerdidasGeneradas"].ToString());
            admin.VolumenMovidoMensual = decimal.Parse(row["VolumenMovidoMensual"].ToString());

            return admin;
        }
        public SqlOperation GetDetalleClientesAdmin()
        {
            return new SqlOperation { ProcedureName = "SP_ObtenerDetalleClientesAdmin" };
        }
        public ReporteAdminDetalle BuildObjectAdminDetalle(Dictionary<string, object> row)
        {
            ReporteAdminDetalle admin = new ReporteAdminDetalle();

            admin.Id_usuario = int.Parse(row["id_cliente"].ToString());
            admin.Nombre = row["nombre_cliente"].ToString();
            admin.Apellido = "";
            admin.NombreAsesor = row["nombre_asesor"] != DBNull.Value ? row["nombre_asesor"].ToString() : "Sin asignar";
            admin.Ingresos = decimal.Parse(row["Ingresos"].ToString());

            return admin;
        }



    }
}
