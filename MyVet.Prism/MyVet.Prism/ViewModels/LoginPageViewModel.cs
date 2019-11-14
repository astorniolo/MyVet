using MyVet.Common.Models;
using MyVet.Common.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MyVet.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;

        //Atributos privados
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _loginCommnad;

        //Constructor
        public LoginPageViewModel(
            INavigationService navigationService,                //  Navigation service me permite pasar de una pagina a otra y pasar parametro
            IApiService  apiService) : base(navigationService)
        {
            Title = "Login";
            _isEnabled = true;
            _apiService = apiService;
        }

        //propiedades
        public DelegateCommand LoginCommand => _loginCommnad ?? (_loginCommnad = new DelegateCommand(Login));

        public string Email { get; set; } //asi las prop qe no vana cambiar desde la vie model

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }// esto lo hacemos con toda prop que cdo cambie en la viewmodel se reflejee en la interfaz de usuario

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        //Metodos Publicos
        private async void Login()
        {
           if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter an email", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a password", "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false; //desabilito el boton

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            //CPNSUMIR SERVICIO
            // que necesitamos consumir  cual es la url dde vamos a consumir los servicios..... la traigo del dicionario de recursos
            var url = App.Current.Resources["UrlApi"].ToString();
            //el api service lo hicimos como clase e interfaz para poderlo inyectar...... asi que inyectemos... en el code behind del servicio  en app.xml.cs y inyectar en ctor
            // ahora que nec saber ....si ese man pudo....
            var response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request); // url viene de recursos, prefijo controlador y reques qu cree recien

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Email or Password incorrecto", "Accept");
                Password = string.Empty;
                return;
            }

            IsRunning = false;
            IsEnabled = true;

            await App.Current.MainPage.DisplayAlert("Error", "Fuck yeahhh!!!", "Accept");
        }

        //Metodos Privados
        
    }
}
