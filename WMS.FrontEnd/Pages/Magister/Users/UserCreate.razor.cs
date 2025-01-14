using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Repositories;
using WMS.FrontEnd.Services;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;
using WMS.Share.Models;
using WMS.FrontEnd.Helpers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Authorization;
using WMS.Share.Models.Magister;

namespace WMS.FrontEnd.Pages.Magister.Users
{
    [Authorize]
    public partial class UserCreate
    {
        private EditContext editContext = null!;
        public UserForm? form;
        private UserDTO Model = new();
        private bool loading;
        private List<DocumentTypeUser>? DocumentTypeUsersList;
        private List<MultipleSelectorModel> selected { get; set; } = new();
        private List<MultipleSelectorModel> nonSelected { get; set; } = new();
        public List<UserType> SelectedUserTypes { get; set; } = new();
        public List<UserType> NonSelectedUserTypes { get; set; } = new();

        public EventCallback OnValidSubmit { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ILoginService LoginService { get; set; } = null!;

        public string? imageUrl;
        public bool FormPostedSuccessfully { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var httpResponse1 = await Repository.GetAsync<List<UserType>>("/api/usertype");
            if (httpResponse1.Error)
            {
                var message = await httpResponse1.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            NonSelectedUserTypes = httpResponse1.Response!;
            nonSelected = NonSelectedUserTypes.Select(x => new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();

            var httpResponse2 = await Repository.GetAsync<List<DocumentTypeUser>>("/api/DocumentTypeUsers/full");
            if (httpResponse2.Error)
            {
                var message = await httpResponse2.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            DocumentTypeUsersList = httpResponse2.Response!;
            editContext = new(Model);
            loading=false;
        }

        private async Task CreateAsync()
        {
            Model.UserTyepIds = selected.Select(x => long.Parse(x.Key)).ToList();
            Model.UserName = Model.Email;
            loading = true;
            var responseHttp = await Repository.PostAsync<UserDTO, TokenDTO>("/api/users/CreateUser", Model);
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            //await LoginService.LoginAsync(responseHttp.Response!.Token);
            NavigationManager.NavigateTo("/users");
        }

        private void ImageSelected(string imagenBase64)
        {
            Model.Photo = imagenBase64;
            imageUrl = null;
        }

        private async Task OnDataAnnotationsValidatedAsync()
        {
            Model.UserTyepIds = selected.Select(x => long.Parse(x.Key)).ToList();
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

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/users");
        }
    }
}