using DataAccess.Dao;
using DTO;
using System;
using System.Collections.Generic;

namespace DataAccess.Mappers
{
    public class AsesoresMapper
    {
        public SqlOperation GetRetrieveAllAsesores()
        {
            return new SqlOperation
            {
                ProcedureName = "SP_GET_ALL_ASESORES"
            };
        }

        public Asesor BuildObject(Dictionary<string, object> row)
        {
            Asesor asesor = new Asesor();

            asesor.Id = int.Parse(row["Id Usuario"].ToString());
            asesor.Correo = row["Correo Electrónico"].ToString();
            asesor.Nombre = row["Nombre"].ToString();
            asesor.Apellido1 = row["Primer Apellido"].ToString();
            asesor.Apellido2 = row["Segundo Apellido"].ToString();
            asesor.TipoUsuario = row["Tipo Usuario"].ToString();
            asesor.Estado = row["Estado"].ToString();
            asesor.FechaNacimiento = DateTime.Parse(row["Fecha de Nacimiento"].ToString());
            asesor.Direccion = row["Dirección"].ToString();

            return asesor;
        }

        public List<Asesor> BuildObjects(List<Dictionary<string, object>> rows)
        {
            List<Asesor> lista = new List<Asesor>();
            foreach (var row in rows)
            {
                lista.Add(BuildObject(row));
            }
            return lista;
        }
    }
}
