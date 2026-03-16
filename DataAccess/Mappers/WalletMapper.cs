using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dao;
using DTO;

namespace DataAccess.Mappers
{
    public class WalletMapper
    {
        /// <summary>
        /// Convierte una lista de diccionarios en una lista de objetos Wallet
        /// </summary>
        public List<Wallet> BuildObjects(List<Dictionary<string, object>> listRows)
        {
            return listRows.Select(row => BuildObject(row)).ToList();
        }

        /// <summary>
        /// Convierte un diccionario en un objeto Wallet
        /// </summary>
        public Wallet BuildObject(Dictionary<string, object> row)
        {
            var wallet = new Wallet();

            if (row.ContainsKey("id_wallet") && row["id_wallet"] != DBNull.Value)
            {
                wallet.IdWallet = Convert.ToInt32(row["id_wallet"]);
            }

            if (row.ContainsKey("tb_tipoFondo") && row["tb_tipoFondo"] != DBNull.Value)
            {
                wallet.TipoFondo = Convert.ToBoolean(row["tb_tipoFondo"]);
            }

            if (row.ContainsKey("id_usuario") && row["id_usuario"] != DBNull.Value)
            {
                wallet.IdUsuario = Convert.ToInt32(row["id_usuario"]);
            }

            if (row.ContainsKey("tm_monto") && row["tm_monto"] != DBNull.Value)
            {
                wallet.Monto = Convert.ToDecimal(row["tm_monto"]);
            }

            if (row.ContainsKey("tf_fecha") && row["tf_fecha"] != DBNull.Value)
            {
                wallet.Fecha = Convert.ToDateTime(row["tf_fecha"]);
            }

            return wallet;
        }

        /// <summary>
        /// Convierte un diccionario en un objeto WalletBalance
        /// </summary>
        public WalletBalance BuildBalanceObject(Dictionary<string, object> row)
        {
            var balance = new WalletBalance();

            if (row.ContainsKey("id_usuario") && row["id_usuario"] != DBNull.Value)
            {
                balance.IdUsuario = Convert.ToInt32(row["id_usuario"]);
            }

            if (row.ContainsKey("balance") && row["balance"] != DBNull.Value)
            {
                balance.Balance = Convert.ToDecimal(row["balance"]);
            }

            return balance;
        }
    }
}