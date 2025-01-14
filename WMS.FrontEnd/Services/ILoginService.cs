using WMS.Share.DTOs;

namespace WMS.FrontEnd.Services
{
    public interface ILoginService
    {
        Task LoginAsync(TokenDTO session);

        Task LogoutAsync();

        Task<List<FormParentDTO>> GetMenu();
    }
}
