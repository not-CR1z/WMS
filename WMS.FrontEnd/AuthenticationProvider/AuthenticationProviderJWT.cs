using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using WMS.FrontEnd.Helpers;
using WMS.FrontEnd.Pages.Security.Auth;
using WMS.FrontEnd.Services;
using WMS.Share.DTOs;
using WMS.Share.Helpers;

namespace WMS.FrontEnd.AuthenticationProvider
{
    public class AuthenticationProviderJWT : AuthenticationStateProvider, ILoginService
    {
        private readonly IJSRuntime _jSRuntime;
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigation;
        private readonly string _tokenKey;
        private readonly AuthenticationState _anonimous;

        public AuthenticationProviderJWT(IJSRuntime jSRuntime, HttpClient httpClient, NavigationManager navigation)
        {
            _jSRuntime = jSRuntime;
            _httpClient = httpClient;
            _navigation = navigation;
            _tokenKey = "TOKEN_KEY";
            _anonimous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _jSRuntime.GetLocalStorage(_tokenKey);
            if (token is null)
            {
                return _anonimous;
            }

            return BuildAuthenticationState(token.ToString()!);
        }

        public async Task LoginAsync(TokenDTO session)
        {
            await _jSRuntime.SetLocalStorage(_tokenKey, session.Token);
            var authState = BuildAuthenticationState(session.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task LogoutAsync()
        {
            await _jSRuntime.RemoveLocalStorage(_tokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(_anonimous));
        }

        private AuthenticationState BuildAuthenticationState(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var claims = ParseClaimsFromJWT(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }
        private IEnumerable<Claim> ParseClaimsFromJWT(string token)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var unserializedToken = jwtSecurityTokenHandler.ReadJwtToken(token);
            //if(unserializedToken.ValidTo< DateTime.UtcNow)
            //{
            //    _navigation.NavigateTo("/Login", true);
            //    await _jSRuntime.RemoveLocalStorage(_tokenKey);
            //    _httpClient.DefaultRequestHeaders.Authorization = null;
            //    NotifyAuthenticationStateChanged(Task.FromResult(_anonimous));
            //    _navigation.NavigateTo("/Login", true);
            //}
            return unserializedToken.Claims;
        }
        public async Task<List<FormParentDTO>> GetMenu()
        {
            var token = await _jSRuntime.GetLocalStorage(_tokenKey);
            if (token is null)
            {
                return new List<FormParentDTO>();
            }
            var claims = ParseClaimsFromJWT(token.ToString()!);
            var jsonForm = claims.Where(w => w.Type == "Forms").FirstOrDefault()!.Value!;
            if (jsonForm is null)
            {
                return new List<FormParentDTO>();
            }
            var lsMenu = JsonConvert.DeserializeObject<List<FormParentDTO>>(jsonForm);
            return (lsMenu!);
        }
    }
}
