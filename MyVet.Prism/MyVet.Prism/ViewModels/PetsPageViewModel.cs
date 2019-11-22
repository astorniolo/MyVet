using MyVet.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class PetsPageViewModel : ViewModelBase
    {
        private OwnerResponse _owner;
        private ObservableCollection<PetItemViewModel> _pets; // si hay cambios en la lista se relflejen en la xaml


        public PetsPageViewModel(
            INavigationService navigationService): base(navigationService)
        {
            Title = "Pets";
        }

        public ObservableCollection<PetItemViewModel> Pets
        {
            get => _pets;
            set => SetProperty(ref _pets, value);
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("owner"))
            {
                _owner = parameters.GetValue<OwnerResponse>("owner");
                Title = $"Pets of:{_owner.Fullname}";
                Pets = new ObservableCollection<PetItemViewModel>(_owner.Pets.Select(
                    p=>new PetItemViewModel
                    {
                        Id = p.Id,
                        Born =p.Born,
                        Race=p.Race,
                        Histories=p.Histories,
                        ImageUrl=p.ImageUrl,
                        Name=p.Name,
                        Remarks=p.Remarks,
                        PetType=p.PetType
                    }).ToList());
            }
        }
    }
}
