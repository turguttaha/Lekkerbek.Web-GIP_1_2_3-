using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Services
{
    public class ChefService
    {
        private static bool UpdateDatabase = true;//If true, stores info in the database. If false, session.

        private readonly ChefRepository _repository;

        public ChefService(ChefRepository chefRepository)
        {
            _repository = chefRepository;
        }

        private IList<Chef> GetAll()
        {

            var result = _repository.GetChef();

            return result;
        }
        public void Create(ChefViewModel chef)
        {   Chef chef1 = new Chef()
        {ChefId=chef.ChefId,
        ChefName=chef.ChefName,
        };
            _repository.AddToDataBase(chef1);
        }
        public void Destroy(Chef chef)
        {
            if (!UpdateDatabase)
            {
                var target = GetAll().FirstOrDefault(p => p.ChefId == chef.ChefId);
                if (target != null)
                {
                    GetAll().Remove(target);
                }
            }
            else
            {
                var entity = new Chef();

                entity.ChefId = chef.ChefId;
                entity.ChefName = chef.ChefName;

                _repository.DeleteFromDataBase(entity);
            }
        }

        public List<TimeSlot> GetTimeSlots()
        {
            return _repository.GetTimeSlots();
        }

        public void Update(Chef product)
        {
           
                var entity = new Chef();

                entity.ChefId = product.ChefId;
                entity.ChefName = product.ChefName;
            _repository.Update(entity);

        }

        public IEnumerable<Chef> Read()
        {
            return GetAll();
        }
    }
}
