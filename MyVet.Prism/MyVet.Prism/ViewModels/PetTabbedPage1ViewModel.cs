using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class PetTabbedPage1ViewModel : ViewModelBase
    {
        public PetTabbedPage1ViewModel(INavigationService navigationService )
        :base(navigationService)
        {
            Title = "Pet tab";
        }
    }
}
