using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Helpers
{
    public interface IValidateSession
    {
        Task<ActionResponse<User>> GetValidateSession(HttpContext httpContext,int FormCode,string Action);
    }
}
