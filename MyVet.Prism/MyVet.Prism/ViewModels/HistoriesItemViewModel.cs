using System;
using System.Collections.Generic;
using System.Text;
using MyVet.Common.Models;
using Prism.Commands;
using Prism.Navigation;

namespace MyVet.Prism.ViewModels
{
    public class HistoriesItemViewModel : HistoryResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _SelectHistoryCommand;

        public HistoriesItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectHistoryCommand => _SelectHistoryCommand ?? (_SelectHistoryCommand = new DelegateCommand(SelectHistory));

        private async void SelectHistory()
        {
            var parameters = new NavigationParameters
            {
                { "history", this }
            };

            await _navigationService.NavigateAsync("HistoryPage",parameters);
            
        }
    }
}
