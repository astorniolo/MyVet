﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Models;

namespace MyVet.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(
            DataContext dataContext,
            ICombosHelper combosHelper)
        {
            _dataContext = dataContext;
            _combosHelper = combosHelper;
        }


        public async Task<Pet> ToPetAsync(PetViewModel model, string path, bool isNew)
        {
            var pet = new Pet
            {
                Id = isNew ? 0 : model.Id,
                Name=model.Name,
                Agendas = model.Agendas,
                Born = model.Born,
                Histories = model.Histories,
                ImageUrl = path,
                Owner = await _dataContext.Owners.FindAsync(model.OwnerId),
                PetType = await _dataContext.PetTypes.FindAsync(model.PetTypeId),
                Race = model.Race,
                Remarks = model.Remarks
            };
            
            return pet;
        }

        public PetViewModel ToPetViewModel(Pet pet)
        {
            return new PetViewModel
            {
                Name = pet.Name,
                Agendas = pet.Agendas,
                Born = pet.Born,
                Histories = pet.Histories,
                ImageUrl = pet.ImageUrl,
                Owner = pet.Owner,
                PetType =pet.PetType,
                Race = pet.Race,
                Remarks = pet.Remarks,
                Id=pet.Id,
                OwnerId=pet.Owner.Id,
                PetTypeId=pet.PetType.Id,
                PetTypes=_combosHelper.GetComboPetTypes()
            };
            
        }
    }
}
