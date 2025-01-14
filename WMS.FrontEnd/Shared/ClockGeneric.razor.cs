using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using System.Net;
using WMS.FrontEnd.Repositories;
using WMS.Share.DTOs;

namespace WMS.FrontEnd.Shared
{
    public partial class ClockGeneric
    {
        [EditorRequired, Parameter] public UserUpdateDTO Model { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        private bool loading;

        protected override async Task OnParametersSetAsync()
        {
            loading = true;
            //User user = new();
            var responseHttpuser = await Repository.PostAsync<UserUpdateDTO,UserUpdateDTO>($"/api/users/Clock", Model);
            if (responseHttpuser.Error)
            {
                if (responseHttpuser.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/bintypes");
                }
                else
                {
                    var messsage = await responseHttpuser.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", messsage, SweetAlertIcon.Error);
                }
            }
            else
            {
                Model = responseHttpuser.Response!;
            }
            loading = false;
        }
    }
}