using MyVet.Common.Helpers;
using MyVet.Common.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class HistoriesPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private PetResponse _pet;
        private ObservableCollection<HistoriesItemViewModel> _histories;

        public HistoriesPageViewModel(
            INavigationService navigationService):base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Histories";
            Pet = JsonConvert.DeserializeObject<PetResponse>(Settings.Pet);
            LoadHistories();

        }

        public PetResponse Pet
        {
            get => _pet;
            set => SetProperty(ref _pet, value);
        }
        public ObservableCollection<HistoriesItemViewModel> Histories 
        {   
            get => _histories;
            set => SetProperty(ref _histories, value);
        }

        //public override void OnNavigatedTo(INavigationParameters parameters)
        //{
        //    base.OnNavigatedTo(parameters);
        //    Pet = JsonConvert.DeserializeObject<PetResponse>(Settings.Pet);
        //    Title = $"pet of{Pet.Name}";
        //}

        private void LoadHistories()
        {
            Histories = new ObservableCollection<HistoriesItemViewModel>(Pet.Histories.Select(h => new HistoriesItemViewModel(_navigationService)
            {
                Id = h.Id,
                Date = h.Date,
                Description = h.Description,
                ServiceType = h.ServiceType,
                Remarks = h.Remarks
            }).ToList());
        }
    }
}
