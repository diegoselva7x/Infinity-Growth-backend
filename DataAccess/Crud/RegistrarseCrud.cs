using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;
using Microsoft.Data.SqlClient;

namespace DataAccess.Crud
{
    public class RegistrarseCrud : CrudFactory
    {
        RegistrarseMapper _mapper;
        
        public RegistrarseCrud(ISqlDao sqlDao)
        {
            _mapper = new RegistrarseMapper();
            _sqlDao = sqlDao;
        }

        public override void Create(Usuarios dto)
        {
            try
            {
                var operation = _mapper.GetCreateStatement(dto);
                _sqlDao.ExecuteStoreProcedure(operation);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) //Si el numero de la exepcion es 2627, es porque el correo ya se encuentra registrado
                {
                    throw new System.Exception("El correo ya se encuentra registrado.");
                }
                else
                {
                    throw;
                }
            }
        }

        public override List<T> RetrieveAll<T>()
        {
            List<T> finalResultList = new List<T>();
            SqlOperation operation = _mapper.GetRetrieveAllStatement(); // me da la operacion 
            List<Dictionary<string, object>> diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation); // Ejecutamos al Dao la operacion

            if (diccList.Count > 0)
            {
                /*var dtoList = _mapper.BuildObjects(diccList); // le pido al mapper que me construya lo obtenido del DAO
                foreach (var item in dtoList)
                {
                    finalResultList.Add((T)Convert.ChangeType(item, typeof(T)));
                }*/
            }
            return finalResultList;
        }

        public override void Delete(Usuarios dto)
        {
            throw new NotImplementedException();
        }

        public override List<T> RetrieveById<T>(Usuarios dto)
        {
            List<T> finalResultList = new List<T>();
            SqlOperation operation = _mapper.GetRetrieveByIdStatement(dto); // me da la operacion 
            List<Dictionary<string, object>> diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation); // Ejecutamos al Dao la operacion

            if (diccList.Count > 0)
            {
                /*var dtoList = _mapper.BuildObjects(diccList); // le pido al mapper que me construya lo obtenido del DAO
                foreach (var item in dtoList)
                {
                    finalResultList.Add((T)Convert.ChangeType(item, typeof(T)));
                }*/
            }
            return finalResultList;
        }

        public override void Update(Usuarios dto)
        {
            throw new NotImplementedException();
        }
    }
}

