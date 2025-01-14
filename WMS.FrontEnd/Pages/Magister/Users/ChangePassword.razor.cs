using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net;
using WMS.FrontEnd.Repositories;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;

namespace WMS.FrontEnd.Pages.Magister.Users
{
    [Authorize]
    public partial class ChangePassword
    {
        [EditorRequired, Parameter]
        public long Id_Local { get; set; }
        private ChangePasswordDTO changePasswordDTO = new();
        private bool loading;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            loading = true;
            //User user = new();
            var responseHttpuser = await Repository.GetAsync<User>($"/api/users/GetIdLocalAsync/{Id_Local}");
            loading = false;
            if (responseHttpuser.Error)
            {
                if (responseHttpuser.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/users");
                }
                else
                {
                    var messsage = await responseHttpuser.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", messsage, SweetAlertIcon.Error);
                }
            }
            else
            {
                var user = responseHttpuser.Response!;
                changePasswordDTO.Id_Local = Id_Local;
                changePasswordDTO.Email = user.Email!;
                changePasswordDTO.NewPassword = string.Empty;
                changePasswordDTO.Confirm = string.Empty;
            }
        }

            private async Task ChangePasswordAsync()
        {
            loading = true;
            var responseHttp = await Repository.PostAsync("/api/users/ChangeEmailPassword", changePasswordDTO);
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            NavigationManager.NavigateTo($"/users/edit/{changePasswordDTO.Id_Local}");
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Email/Contraseña cambiada con éxito.");
        }
    }
}