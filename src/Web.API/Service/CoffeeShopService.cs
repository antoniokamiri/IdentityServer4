using DataAcess.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.API.Models;

namespace Web.API.Service
{
    public class CoffeeShopService : ICoffeeShopService
    {
        private readonly ApplicationDbContext _context;

        public CoffeeShopService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CoffeeShopModel>> List()
        {
            var coffeeShop = await _context.CoffeeShops.ToListAsync();

            var coffeeModelList = new List<CoffeeShopModel>();


            foreach (var item in coffeeShop)
            {
                var coffeeModel = new CoffeeShopModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    OpeningHours = item.OpeningHours,
                    Address = item.Address,
                };
                coffeeModelList.Add(coffeeModel);
            }


            return coffeeModelList;
        }
    }
}
