using System;
using System.Threading.Tasks;
using Azure;
using DataAccess.Crud;
using DTO;

namespace AppLogic
{
    public interface IPortafoliosManager
    {
  
    }

    public class PortafoliosManager : IPortafoliosManager
    {
       /* public List <Portafolios> ReatrivePortafolioByUserId(int pUserId)
        {
            try
            {
                var portafoliosCrud = new PortafoliosCrud();
                var appList = portafoliosCrud.RetrieveById<Portafolios>(new Portafolios { Usuario = pUserId });

                return appList; 
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
        }*/
    }
}
