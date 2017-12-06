using NetEasyPay.Interfaces;
using netApi.Repositories.Authorization.Interfaces;
using netApi.Repositories.Authorization.Model;
using netApi.Repositories.Authorization.Repositories;

namespace NetEasyPay.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly FopsAuthorization _context = new FopsAuthorization();
                
        private readonly IAuthorizationRepository _repository;

        public AuthorizationService()
        {
            _repository = new AuthorizationRepository(_context);
        }

        public APP_USER LoadUser(long UserId)
        {
            return _repository.LoadUser(UserId);
        }

        public object GetClaims(System.Security.Principal.IPrincipal principal)
        {
            return _repository.GetClaims(principal);
        }

        public object ValidateEmail(string email)
        {
            return _repository.ValidateEmail(email);
        }

        public object GetFnfAuthenticationServiceToken(string client_id, string client_secret)
        {
            return _repository.GetFnfAuthenticationServiceToken(client_id, client_secret);
        }
    }
}