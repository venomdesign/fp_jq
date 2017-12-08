using netApi.Repositories.Administration.Model;
using netApi.Repositories.Authorization.Model;
using System.Collections.Generic;
using static netApi.Common.Helpers;

namespace NetEasyPay.Interfaces
{
    public interface IAdministrationService
    {
        List<APP_USER> GetUsers();
        APP_USER GetUser(long UserId);
        APP_USER GetUser(string username);
        Response AddUser(EasyPayAuth0User newUser);
        APP_USER UpdateUser(APP_USER userToUpdate);
        APP_USER DeleteUser(long UserId);
        List<APP_ROLE> GetRoles();
        List<APP_ROLE> GetActiveRoles();
        APP_ROLE GetRole(int Id);
        List<APP_ROLE> GetRolesByUser(long UserId);
        APP_ROLE AddRole(APP_ROLE roleToAdd);
        APP_ROLE DeleteRole(int RoleId);
        USER_ROLE AddRoleToUser(USER_ROLE urToAdd);
        USER_ROLE RemoveRoleFromUser(long UserId, int RoleId);
        bool DeletePermissionFromRole(int RoleId, int PermissionId);
        bool DeletePermission(int PermissionId);
        List<APP_PERMISSION> GetPermissionsByRole(int RoleId);
        List<APP_PERMISSION> GetPermissions();
        APP_PERMISSION AddPermission(APP_PERMISSION perm);
        ROLE_PERMISSION AddPermissionToRole(int RoleId, int PermissionId);
        USER_TOKEN SaveUserToken(long userId, long token);
        bool DeleteUserToken(long userId, long token);
        List<USER_TOKEN> GetTokensByUser(long userId);
        object AssociateCRRARContactToUser(USER_CONTACT newUC);
        List<APP_USER> GetPendingUsers(string status);
        bool ForgotPassword(string emailAddress);

    }
}
