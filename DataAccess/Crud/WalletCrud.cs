using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;

namespace DataAccess.Crud
{
    public class WalletCrud
    {
        private readonly ISqlDao _dao;
        private WalletMapper _mapper;

        public WalletCrud(ISqlDao dao)
        {
            _dao = dao;
            _mapper = new WalletMapper();
        }

        /// <summary>
        /// Obtiene el balance del wallet de un usuario
        /// </summary>
        public WalletBalance GetWalletBalance(int idUsuario)
        {
            var operation = new SqlOperation();
            operation.ProcedureName = "SP_GET_WALLET_BALANCE";
            operation.AddIntParam("idUsuario", idUsuario);

            var result = _dao.ExecuteStoreProcedureWithQuery(operation);
            if (result.Count > 0)
            {
                return _mapper.BuildBalanceObject(result[0]);
            }
            else
            {
                // Si no hay balance, retornar uno con valor 0
                return new WalletBalance { IdUsuario = idUsuario, Balance = 0 };
            }
        }

        /// <summary>
        /// Obtiene las transacciones del wallet de un usuario
        /// </summary>
        public List<Wallet> GetWalletTransactionsByUser(int idUsuario)
        {
            var operation = new SqlOperation();
            operation.ProcedureName = "SP_GET_WALLET_TRANSACTIONS_BY_USER";
            operation.AddIntParam("idUsuario", idUsuario);

            var result = _dao.ExecuteStoreProcedureWithQuery(operation);
            return _mapper.BuildObjects(result);
        }

        /// <summary>
        /// Registra una transacción en el wallet
        /// </summary>
        public void RegisterWalletTransaction(WalletTransaction transaction)
        {
            var operation = new SqlOperation();
            operation.ProcedureName = "SP_REGISTER_WALLET_TRANSACTION";
            operation.AddIntParam("idUsuario", transaction.IdUsuario);
            operation.AddVarcharParam("monto", transaction.Monto.ToString(System.Globalization.CultureInfo.InvariantCulture));
            operation.AddBooleanParam("tipoFondo", transaction.TipoFondo);
            
            // NOTA: El SP no acepta el parámetro "concepto", así que no lo enviamos
            // Si en el futuro se modifica el SP para aceptar concepto, descomentar el código siguiente:
            /*
            if (!string.IsNullOrEmpty(transaction.Concepto))
            {
                operation.AddVarcharParam("concepto", transaction.Concepto);
            }
            else
            {
                operation.AddVarcharParam("concepto", transaction.TipoFondo ? "Depósito" : "Retiro");
            }
            */

            _dao.ExecuteStoreProcedure(operation);
        }
    }
}
