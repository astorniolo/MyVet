using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MyVet.Common.Models;
using Newtonsoft.Json;

namespace MyVet.Common.Services
{
    public class ApiService : IApiService
    {
        public async Task<Response> GetOwnerByEmailAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string email)
        {
            try
            {
                var request = new EmailRequest { Email = email };
                var requestString = JsonConvert.SerializeObject(request);
                var content = new StringContent(requestString, Encoding.UTF8, "application/json");
                var client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase)
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                var url = $"{servicePrefix}{controller}";
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var owner = JsonConvert.DeserializeObject<OwnerResponse>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = owner
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    



        public async Task<Response> GetTokenAsync(
            string urlBase,                                       // la urlBase es https://myvetweb2019.azurewebsites.net/Account/createtoken es de donde consumiremos todos los servicios
            string servicePrefix,                                 // https://myvetweb2019.azurewebsites.net
            string controller,                                    // Account/createtoken
            TokenRequest request)                                 // request... si vemos el postman es la combinacion de mail y contraseña
        {
            try
            {
                var requestString = JsonConvert.SerializeObject(request);                                   // instalar NUGET    // necesitamos el request que es la comb mail-Password y lo serializamos
                var content = new StringContent(requestString, Encoding.UTF8, "application/json");          // despues de serializarlo lo codificamos en  utf8 
                var client = new HttpClient                                                                 // creamos un cliente http con la urlbase https://myvetweb2019.azurewebsites.net/Account/createtoken
                {
                    BaseAddress = new Uri(urlBase)
                };

                var url = $"{servicePrefix}{controller}";                                // prefijo y ctrller =  Account/createtoken
                var response = await client.PostAsync(url, content);                     // hacemos el post....
                var result = await response.Content.ReadAsStringAsync();                 //  leemos resultado del status code          

                if (!response.IsSuccessStatusCode)    // si el resultado es falso entonces devuelvo el mensaje de porque fallo
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var token = JsonConvert.DeserializeObject<TokenResponse>(result);   // la rta llega como string asi que deserializamos la rta... deserializar es string-->obj y  serializar es obj-->string......
                return new Response
                {
                    IsSuccess = true,
                    Result = token
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
