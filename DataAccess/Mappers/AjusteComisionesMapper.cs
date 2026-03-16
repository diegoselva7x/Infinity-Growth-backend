using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DTO;

namespace DataAccess.Mappers
{

    public class AjusteComisionesMapper
    {
        /// <summary>
        /// Convierte una lista de diccionarios (resultado del SqlDao)
        /// en una lista de objetos AjusteComisiones.
        /// </summary>
        /// <param name="listRows">Lista de filas, donde cada fila es un diccionario de nombre_columna -> valor.</param>
        /// <returns>Lista de objetos AjusteComisiones.</returns>
        public List<AjusteComisiones> BuildObjects(List<Dictionary<string, object>> listRows)
        {
            // Usamos LINQ para iterar y mapear cada diccionario a un objeto AjusteComisiones
            return listRows.Select(row => BuildObject(row)).ToList();
        }

        /// <summary>
        /// Convierte un único diccionario (una fila) en un objeto AjusteComisiones.
        /// </summary>
        /// <param name="row">Diccionario que representa una fila de la base de datos.</param>
        /// <returns>Un objeto AjusteComisiones.</returns>
        public AjusteComisiones BuildObject(Dictionary<string, object> row)
        {
            // Crear instancia del DTO
            var comision = new AjusteComisiones();

            // Mapear cada columna del diccionario a la propiedad correspondiente del DTO,
            // asegurándose de manejar posibles valores nulos (DBNull.Value) y conversiones de tipo.

            if (row.ContainsKey("id_Tipocomision") && row["id_Tipocomision"] != DBNull.Value)
            {
                comision.IdTipoComision = Convert.ToInt32(row["id_Tipocomision"]);
            }

            if (row.ContainsKey("tv_nombre") && row["tv_nombre"] != DBNull.Value)
            {
                comision.Nombre = Convert.ToString(row["tv_nombre"]) ?? string.Empty;
            }
            else // Asegurar que no sea null si la BD lo permite pero el DTO no
            {
                comision.Nombre = string.Empty;
            }


            // tv_descripcion puede ser null
            if (row.ContainsKey("tv_descripcion") && row["tv_descripcion"] != DBNull.Value)
            {
                comision.Descripcion = Convert.ToString(row["tv_descripcion"]);
            }
            else
            {
                comision.Descripcion = null; // Permitir null si el DTO lo hace (string?)
            }


            if (row.ContainsKey("tp_porcentaje") && row["tp_porcentaje"] != DBNull.Value)
            {
                // El tipo devuelto por SQL Server para DECIMAL puede ser Decimal directamente.
                comision.Porcentaje = Convert.ToDecimal(row["tp_porcentaje"]);
            }

            return comision;
        }
    }
}

