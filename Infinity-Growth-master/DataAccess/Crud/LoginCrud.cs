using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;

namespace DataAccess.Crud
{
    public class LoginCrud : CrudFactory
    {
        LoginMapper _mapper;

        public LoginCrud(ISqlDao sqlDao)
        {
            _mapper = new LoginMapper();
            _sqlDao = sqlDao;
        }

        public List<T> RetrieveByCorreo<T>(string correo)
        {
            List<T> finalResultList = new List<T>();

            Usuarios usuario = new Usuarios { Correo = correo };

            SqlOperation operation = _mapper.GetRetrieveByCorreo(usuario);

            List<Dictionary<string, object>> diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation);

            foreach (var dicc in diccList) // Se valida si la imagen es nula 
            {
                if (dicc["bt_profileFoto"] == DBNull.Value)
                {
                    dicc["bt_profileFoto"] = new byte[0];
                }
            }

            if (diccList.Count > 0)
            {
                var dtoList = diccList.Select(dicc => _mapper.BuildObject(dicc)).ToList();

                foreach (var item in dtoList) // Se valida si el objeto es de tipo Usuarios para agregarlo a la lista final
                {
                    if (typeof(T) == typeof(Usuarios))
                    {
                        finalResultList.Add((T)(object)item);
                    }
                    else
                    {
                        finalResultList.Add((T)Convert.ChangeType(item, typeof(T)));
                    }
                }
            }

            return finalResultList;
        }

        public List<T> RetrieveByUserId<T>(int id) // Se obtiene el usuario por su ID 
        {
            List<T> finalResultList = new List<T>();
            Usuarios usuario = new Usuarios { Id = id };
            SqlOperation operation = _mapper.GetRetrieveById(usuario);

            List<Dictionary<string, object>> diccList = _sqlDao.ExecuteStoreProcedureWithQuery(operation);
            if (diccList.Count > 0)
            {
                var dtoList = diccList.Select(dicc => _mapper.BuildObject(dicc)).ToList();
                foreach (var item in dtoList)
                {
                    finalResultList.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            return finalResultList;
        }

        public override void Create(Usuarios dto)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Usuarios dto)
        {
            throw new NotImplementedException();
        }

        public override List<T> RetrieveAll<T>()
        {
            throw new NotImplementedException();
        }

        public override List<T> RetrieveById<T>(Usuarios pId)
        {
            throw new NotImplementedException();
        }

        public override void Update(Usuarios dto)
        {
            throw new NotImplementedException();
        }

    }
}

