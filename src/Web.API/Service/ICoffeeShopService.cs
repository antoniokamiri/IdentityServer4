using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Models;

namespace Web.API.Service
{
    public interface ICoffeeShopService
    {
        Task<List<CoffeeShopModel>> List();
    }
}
