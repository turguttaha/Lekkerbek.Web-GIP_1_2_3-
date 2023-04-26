using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;

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
        public void Create(Chef chef)
        {
            _repository.AddToDataBase(chef);
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

                _repository.DeleteFromDataBase(entity);
            }
        }
        public IEnumerable<Chef> Read()
        {
            return GetAll();
        }
    }
}
