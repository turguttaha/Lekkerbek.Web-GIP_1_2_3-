using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;
using Lekkerbek.Web.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Services
{
    public class ChefService
    {
        private static bool UpdateDatabase = true;//If true, stores info in the database. If false, session.

        private readonly ChefRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public ChefService(ChefRepository chefRepository, UserManager<IdentityUser> userManager)
        {
            _repository = chefRepository;
            _userManager = userManager;
        }

        private IList<Chef> GetAll()
        {

            var result = _repository.GetChef();

            return result;
        }

        public IList<ChefViewModel> GetChefsWithIdentity() {
            return _repository.chefViewModels();
        }
        public async void Create(ChefViewModel chef)
        {
            var user = await _userManager.FindByIdAsync(chef.IdentityId);
            Chef chef1 = new Chef()
        {ChefId=chef.ChefId,
        ChefName=chef.ChefName,
        IdentityUser = user,

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
                entity.IdentityUser = chef.IdentityUser;

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
