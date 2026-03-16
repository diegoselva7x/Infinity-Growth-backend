using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogic;
using DataAccess.Mappers;
using DTO;
using InfinityGrowth.DataAccess.Crud;

namespace AppLogic
{
    public interface IAjusteComisionesManager
    {
        List<AjusteComisiones> GetAllComisiones();
        void UpdateComisiones(List<AjusteComisiones> comisionesParaActualizar);
    }

    public class AjusteComisionesManager : IAjusteComisionesManager
    {
        private readonly CatalogoComisionesCrud _catalogoComisionesCrud;

        public AjusteComisionesManager(CatalogoComisionesCrud catalogoComisionesCrud)
        {
            _catalogoComisionesCrud = catalogoComisionesCrud;
        }

        /// <summary>
        /// Obtiene todas las configuraciones de comisiones.
        /// </summary>
        public List<AjusteComisiones> GetAllComisiones()
        {
            return _catalogoComisionesCrud.RetrieveAll();
        }

        /// <summary>
        /// Actualiza los porcentajes de una lista de comisiones.
        /// </summary>
        public void UpdateComisiones(List<AjusteComisiones> comisionesParaActualizar)
        {
            if (comisionesParaActualizar == null || !comisionesParaActualizar.Any())
            {
                return; // O lanzar excepción
            }

            foreach (var comision in comisionesParaActualizar)
            {
                // Validaciones opcionales aquí...
                _catalogoComisionesCrud.UpdatePorcentaje(comision.IdTipoComision, comision.Porcentaje);
            }
        }
    }
}



    
     