﻿using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;
using Lekkerbek.Web.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Lekkerbek.Web.Repositories
{
    public class ChefRepository
    {
        private readonly LekkerbekContext _context;

        public ChefRepository(LekkerbekContext context)
        {
            _context = context;
        }
        public List<Chef> GetChef()
        {
            return _context.Chefs.ToList();

        }

        public List<ChefViewModel> chefViewModels()
        {
             return _context.Chefs.Select(chef => new ChefViewModel
            //This is another way to make a new object
            {
                ChefId = chef.ChefId,
                ChefName = chef.ChefName,
                Email= chef.IdentityUser.Email,
                IdentityId = chef.IdentityUser.Id

            }).ToList();
        }
        public void AddToDataBase(Chef chef)
        {
            _context.Add(chef);
            _context.SaveChanges();
        }
        public void DeleteFromDataBase(Chef entity)
        {

            _context.Chefs.Remove(entity);
            _context.SaveChanges();
        }
        public List<TimeSlot> GetTimeSlots()
        {
            return _context.TimeSlots.Include(c=>c.Chef).ToList();
        }

        public void Update(Chef entity)
        {
            _context.Chefs.Attach(entity);
            _context.Update(entity);
            _context.SaveChanges();
        }
    }
}
