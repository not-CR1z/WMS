using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Helpers;
using WMS.FrontEnd.Repositories;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;
using WMS.Share.Models.Location;
using Blazored.Modal.Services;
using Blazored.Modal;
using WMS.Share.Models.Magister;

namespace WMS.FrontEnd.Pages.Location.Companies
{
    public partial class CompanyForms
    {
        private EditContext editContext = null!;
        private List<Country>? countries;
        private List<State>? states;
        private List<City>? cities;
        private bool loading;

        private List<DocumentTypeUser>? DocumentTypeUsersList;
        private List<MultipleSelectorModel> selected { get; set; } = new();
        private List<MultipleSelectorModel> nonSelected { get; set; } = new();
        public Company Model { get; set; } = null!;

        [EditorRequired, Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired, Parameter]
        public EventCallback ReturnAction { get; set; }

        [Parameter]
        public List<UserType> SelectedUserTypes { get; set; } = new();

        [Parameter, EditorRequired]
        public List<UserType> NonSelectedUserTypes { get; set; } = new();

        [Parameter]
        public bool IsCreate { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        public string? imageUrl;

        public bool FormPostedSuccessfully { get; set; }

        private void ImageSelected(string imagenBase64)
        {
            Model.Logo = imagenBase64;
            imageUrl = null;
        }

        protected async override Task OnInitializedAsync()
        {
            loading = true;
            var responseHttp = await Repository.GetAsync<Company>("/api/companies");
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            Model = responseHttp.Response!;
            if (!string.IsNullOrEmpty(Model!.Logo))
            {
                imageUrl = Model.Logo;
                Model.Logo = null;
            }
            await LoadCountriesAsync();
            if (Model.CityId != 0)
            {
                await LoadStatesAsyn(Model!.City!.State!.Country!.Id);
                await LoadCitiesAsyn(Model!.City!.State!.Id);
            }
        }

        private async Task CountryChangedAsync(ChangeEventArgs e)
        {
            var selectedCountry = Convert.ToInt32(e.Value!);
            states = null;
            cities = null;
            Model!.CityId = 0;
            await LoadStatesAsyn(selectedCountry);
        }

        private async Task StateChangedAsync(ChangeEventArgs e)
        {
            var selectedState = Convert.ToInt32(e.Value!);
            cities = null;
            Model!.CityId = 0;
            await LoadCitiesAsyn(selectedState);
        }

        private async Task LoadCountriesAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            countries = responseHttp.Response;
        }

        private async Task LoadStatesAsyn(long countryId)
        {
            var responseHttp = await Repository.GetAsync<List<State>>($"/api/states/combo/{countryId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            states = responseHttp.Response;
        }

        private async Task LoadCitiesAsyn(long stateId)
        {
            var responseHttp = await Repository.GetAsync<List<City>>($"/api/cities/combo/{stateId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            cities = responseHttp.Response;
        }

        private async Task SaveAsync()
        {
            loading = true;
            var httpResponse = await Repository.PostAsync("/api/companies", Model);
            loading = false;
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            NavigationManager.NavigateTo("/company",true);
        }
        private async Task OnDataAnnotationsValidatedAsync()
        {
            await OnValidSubmit.InvokeAsync();
        }
        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWasEdited = editContext.IsModified();
            if (!formWasEdited || FormPostedSuccessfully)
            {
                return;
            }

            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Deseas abandonar la página y perder los cambios?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = !string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }
            context.PreventNavigation();
        }
    }
}