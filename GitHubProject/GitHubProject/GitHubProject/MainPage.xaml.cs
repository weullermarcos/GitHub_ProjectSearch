using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GitHubProject
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void GetAsyncRepositories(string user)
        {
            projectsStackLayout.IsVisible = false;
            searchStackLayout.IsVisible = true;

            string url = string.Format("https://api.github.com/users/{0}/repos", user);

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("User-Agent", "Other");

            try
            {
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                {
                    await DisplayAlert("Alerta", "Nenhum projeto encontrado, verifique o usuário digitado.", "OK");
                    return;
                }

                var json = JArray.Parse(content);

                var repositories = new List<string>();

                foreach (var item in json)
                {
                    var repository = item.Value<string>("full_name");
                    repositories.Add(repository);
                }


                PopulateList(repositories);
            }
            catch (Exception)
            {
                await DisplayAlert("Alerta", "Erro ao buscar projetos, verifique o usuário digitado e a sua conexão com a internet", "OK");

                searchStackLayout.IsVisible = false;
                projectsStackLayout.IsVisible = false;
            }
        }

        private void PopulateList(List<string> projectList)
        {
            if (projectList == null)
                return;

            projectListView.ItemsSource = projectList;

            searchStackLayout.IsVisible = false;
            projectsStackLayout.IsVisible = true;

        }

        void mySearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            var user = sender as SearchBar;

            if (user == null)
                return;

            GetAsyncRepositories(user.Text);
        }

        private void MySearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(mySearchBar.Text))
            {
                searchStackLayout.IsVisible = false;
                projectsStackLayout.IsVisible = false;
            }
        }
    }
}
