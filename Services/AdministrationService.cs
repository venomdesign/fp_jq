using netApi.Repositories.Administration.Interfaces;
using netApi.Repositories.Administration.Model;
using netApi.Repositories.Administration.Repositories;
using netApi.Repositories.Authorization.Model;
using NetEasyPay.Interfaces;
using System.Collections.Generic;
using static netApi.Common.Helpers;

namespace NetEasyPay.Services
{
    public class AdministrationService : IAdministrationService
    {
        private readonly FopsAuthorization _context = new FopsAuthorization();

        private readonly IAdministrationRepository _repository;

        public AdministrationService()
        {
            _repository = new AdministrationRepository(_context);
        }

        public APP_ROLE AddRole(APP_ROLE roleToAdd)
        {
            return _repository.AddRole(roleToAdd);
        }

        public USER_ROLE AddRoleToUser(USER_ROLE urToAdd)
        {
            return _repository.AddRoleToUser(urToAdd);
        }

        public Response AddUser(EasyPayAuth0User newUser)
        {
            return _repository.AddUser(newUser);
        }

        public APP_ROLE DeleteRole(int RoleId)
        {
            return _repository.DeleteRole(RoleId);
        }

        public APP_USER DeleteUser(long UserId)
        {
            return _repository.DeleteUser(UserId);
        }

        public List<APP_ROLE> GetActiveRoles()
        {
            return _repository.GetActiveRoles();
        }

        public APP_ROLE GetRole(int Id)
        {
            return _repository.GetRole(Id);
        }

        public List<APP_ROLE> GetRoles()
        {
            return _repository.GetRoles();
        }

        public List<APP_ROLE> GetRolesByUser(long UserId)
        {
            return _repository.GetRolesByUser(UserId);
        }

        public APP_USER GetUser(long UserId)
        {
            return _repository.GetUser(UserId);
        }

        public APP_USER GetUser(string username)
        {
            return _repository.GetUser(username);
        }

        public List<APP_USER> GetPendingUsers(string status)
        {
            return _repository.GetPendingUsers(status);
        }

        public List<APP_USER> GetUsers()
        {
            return _repository.GetUsers();
        }

        public USER_ROLE RemoveRoleFromUser(long UserId, int RoleId)
        {
            return _repository.RemoveRoleFromUser(UserId, RoleId);
        }

        public bool DeletePermissionFromRole(int RoleId, int PermissionId)
        {
            return _repository.DeletePermissionFromRole(RoleId, PermissionId);
        }

        public bool DeletePermission(int PermissionId)
        {
            return _repository.DeletePermission(PermissionId);
        }

        public APP_USER UpdateUser(APP_USER userToUpdate)
        {
            return _repository.UpdateUser(userToUpdate);
        }

        public List<APP_PERMISSION> GetPermissionsByRole(int RoleId)
        {
            return _repository.GetPermissionsByRole(RoleId);
        }

        public List<APP_PERMISSION> GetPermissions()
        {
            return _repository.GetPermissions();
        }

        public APP_PERMISSION AddPermission(APP_PERMISSION perm)
        {
            return _repository.AddPermission(perm);
        }

        public ROLE_PERMISSION AddPermissionToRole(int RoleId, int PermissionId)
        {
            return _repository.AddPermissionToRole(RoleId, PermissionId);
        }

        public USER_TOKEN SaveUserToken(long userId, long token)
        {
            return _repository.SaveUserToken(userId, token);
        }

        public bool DeleteUserToken(long userId, long token)
        {
            return _repository.DeleteUserToken(userId, token);
        }

        public List<USER_TOKEN> GetTokensByUser(long userId)
        {
            return _repository.GetTokensByUserId(userId);
        }

        public object AssociateCRRARContactToUser(USER_CONTACT newUC)
        {
            return _repository.AssociateCRRARContactToUser(newUC);
        }

        public bool ForgotPassword(string emailAddress)
        {
            return _repository.ForgotPassword(emailAddress);
        }
    }
}
