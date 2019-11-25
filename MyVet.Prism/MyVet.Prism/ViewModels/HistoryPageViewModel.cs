using MyVet.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class HistoryPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationSevice;
        private HistoryResponse _history;

        public HistoryPageViewModel(INavigationService navigationSevice) : base(navigationSevice)
        {
            _navigationSevice = navigationSevice;
            Title = "History";
        }

        public HistoryResponse History 
        {
            get => _history; 
            set => SetProperty(ref _history, value); 
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("history"))
            {
                History = parameters.GetValue<HistoryResponse>("history");
            }
        }
    }
}
