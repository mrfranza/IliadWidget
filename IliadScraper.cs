using System;
using HtmlAgilityPack;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IliadWidget
{
    public class IliadScraper
    {
        private readonly HttpClient _httpClient;
        private readonly string _username;
        private readonly string _password;

        public IliadScraper(string username, string password)
        {
            _httpClient = new HttpClient();
            _username = username;
            _password = password;
        }

        public async Task<IliadData> GetIliadDataAsync()
        {
            // Effettua il login
            var loginSuccess = await LoginAsync();
            if (!loginSuccess)
            {
                return null;
            }

            // Recupera i dati dell'utilizzo della connessione internet
            var iliadData = await GetDataAsync();
            return iliadData;
        }

        private async Task<bool> LoginAsync()
        {
            // Effettua la richiesta GET alla pagina di login
            var loginPage = await _httpClient.GetStringAsync("https://www.iliad.it/account/");

            // Crea un'istanza di HtmlDocument
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(loginPage);

            // Recupera il form di login
            var loginForm = htmlDoc.GetElementbyId("login-form");
            if (loginForm == null)
            {
                return false;
            }

            // Recupera i campi di input per l'inserimento delle credenziali
            var usernameField = loginForm.SelectSingleNode("//input[@name='username']");
            var passwordField = loginForm.SelectSingleNode("//input[@name='password']");
            var tokenField = loginForm.SelectSingleNode("//input[@name='_token']");

            // Imposta i valori dei campi di input
            usernameField.SetAttributeValue("value", _username);
            passwordField.SetAttributeValue("value", _password);
            tokenField.SetAttributeValue("value", tokenField.GetAttributeValue("value", ""));

            // Crea l'oggetto FormUrlEncodedContent per inviare i dati del form
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", _username),
                new KeyValuePair<string, string>("password", _password),
                new KeyValuePair<string, string>("_token", tokenField.GetAttributeValue("value", ""))
            });

            // Effettua la richiesta POST per effettuare il login
            var loginResponse = await _httpClient.PostAsync("https://www.iliad.it/account/login", formData);

            // Verifica se il login è stato effettuato con successo
            if (!loginResponse.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        private async Task<IliadData> GetDataAsync()
        {
            // Effettua la richiesta GET alla pagina dei dati dell'utilizzo della connessione internet
            var dataPage = await _httpClient.GetStringAsync("https://www.iliad.it/account/");

            // Crea un'istanza di HtmlDocument
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(dataPage);

            // Crea un'istanza di IliadData
            var iliadData = new IliadData();

            // Utilizza i metodi di HtmlAgilityPack per estrarre i dati dell'utilizzo della connessione internet
            var dataUsageNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='traffic-data']");
            iliadData.DataUsage = dataUsageNode.InnerText;

            var dataLimitNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='traffic-limit']");
            iliadData.DataLimit = dataLimitNode.InnerText;

            var expirationDateNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='traffic-end']");
            iliadData.ExpirationDate = expirationDateNode.InnerText;

            return iliadData;
        }
    }
}
