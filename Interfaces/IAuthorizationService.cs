using netApi.Repositories.Authorization.Model;

namespace NetEasyPay.Interfaces
{
    public interface IAuthorizationService
    {
        APP_USER LoadUser(long UserId);
        object GetClaims(System.Security.Principal.IPrincipal principal);
        object ValidateEmail(string email);
        object GetFnfAuthenticationServiceToken(string client_id, string client_secret);
    }
}
