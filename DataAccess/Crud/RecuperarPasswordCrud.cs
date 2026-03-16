using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;

namespace DataAccess.Crud
{
    public class RecuperarPasswordCrud
    {
        private readonly ISqlDao _sqlDao;
        private readonly RecuperarPasswordMapper _mapper;

        public RecuperarPasswordCrud(ISqlDao sqlDao)
        {
            _sqlDao = sqlDao;
            _mapper = new RecuperarPasswordMapper();
        }

        public void UpadatePassword(string correo, string password)
        {
            var operation = _mapper.GetUpdatePasswordStatement(correo, password);
            _sqlDao.ExecuteStoreProcedure(operation);
        }


    }
}
