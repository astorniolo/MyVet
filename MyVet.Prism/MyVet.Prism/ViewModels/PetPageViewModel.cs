﻿using MyVet.Common.Helpers;
using MyVet.Common.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class PetPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private PetResponse _pet;

        public PetPageViewModel(
            INavigationService navigationService) :base(navigationService)
        {
            Title = "Details";
            _navigationService = navigationService;
        }
        public PetResponse Pet
        {
            get => _pet;
            set => SetProperty(ref _pet, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Pet = JsonConvert.DeserializeObject<PetResponse>(Settings.Pet);
            //ejecuta laos navigate to de la primer pestaña.........
        }
    }
}
