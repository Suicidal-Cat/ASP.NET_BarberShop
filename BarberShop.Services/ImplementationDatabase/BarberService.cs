﻿using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Services.ImplementationDatabase
{
	public class BarberService : IBarberService
	{
		private readonly IUnitOfWork uow;
        public BarberService(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        public IEnumerable<Barber> Barbers => uow.BarberRepository.GetAll();

		public void Add(Barber barber)
		{
			uow.BarberRepository.Add(barber);
			uow.SaveChanges();
		}
		public void Delete(int id)
		{
			throw new NotImplementedException();
		}

		public Barber Get(int id)
		{
			return uow.BarberRepository.GetById(id);
		}

        public IEnumerable<Barber> SearchByName(string name)
        {
            return uow.BarberRepository.GetByCondition(b => b.FirstName.Contains(name) || b.LastName.Contains(name));
        }

        public void Update(Barber barber)
		{
			uow.BarberRepository.Update(barber);
		}
	}
}
