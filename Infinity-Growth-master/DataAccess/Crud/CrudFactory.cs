using DataAccess.Dao;
using DTO;

namespace DataAccess.Crud
{
    public abstract class CrudFactory
    {
        protected ISqlDao _sqlDao;

        public abstract void Create(Usuarios dto);
        public abstract void Update(Usuarios dto);
        public abstract void Delete(Usuarios dto);
        public abstract List<T> RetrieveAll<T>();
        public abstract List<T> RetrieveById<T>(Usuarios pId);

    }
}
