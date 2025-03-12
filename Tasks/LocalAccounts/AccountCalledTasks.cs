namespace remoteCmd.Tasks.LocalAccounts
{
    public static class CalledAccountTasks
    {
        public static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
        {
            var ChangePassword = body.Contains("ChangePassword", StringComparison.OrdinalIgnoreCase);
            var ChangePasswordNever = body.Contains("ChangePasswordNever", StringComparison.OrdinalIgnoreCase);
            var ChangePasswordExpires = body.Contains("ChangePasswordExpires", StringComparison.OrdinalIgnoreCase);

            if (ChangePassword || ChangePasswordNever || ChangePasswordExpires)
            {
                bool pwdExpires = ChangePasswordExpires ? true : false;
                bool pwdNeverExpires = ChangePasswordNever ? true : false;
                var userName_NewPassword = body.Split("|");

                LocalAccountsManagement.ChangePassword(userName_NewPassword[1], userName_NewPassword[2], pwdExpires, pwdNeverExpires);
            }

            var CreateLocalAccountDefault = body.Contains("CreateLocalAccountDefault", StringComparison.OrdinalIgnoreCase);
            var CreateLocalAccountPasswordNeverExpires = body.Contains("CreateLocalAccountPasswordNeverExpires", StringComparison.OrdinalIgnoreCase);
            var CreateLocalAccountPasswordExpires = body.Contains("CreateLocalAccountPasswordExpires", StringComparison.OrdinalIgnoreCase);

            if (CreateLocalAccountDefault || CreateLocalAccountPasswordNeverExpires || CreateLocalAccountPasswordExpires)
            {
                bool pwdExpires = CreateLocalAccountPasswordExpires ? true : false;
                bool pwdNeverExpires = CreateLocalAccountPasswordNeverExpires ? true : false;

                var userName_NewPassword = body.Split("|");

                LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], GroupsAccount(userName_NewPassword) ?? Array.Empty<string>(), pwdExpires, pwdNeverExpires);
            }

            var DisableAccount = body.Contains("DisableAccount", StringComparison.OrdinalIgnoreCase);
            var EnableAccount = body.Contains("EnableAccount", StringComparison.OrdinalIgnoreCase);

            if (DisableAccount || EnableAccount)
            {
                bool EnableDisable = EnableAccount ? false : true;

                var userName = body.Split("|");
                LocalAccountsManagement.EnableDisableAccount(userName[1], EnableDisable);
            }

            var GetAllUsers = body.Contains("GetAllUsers", StringComparison.OrdinalIgnoreCase);
            if (GetAllUsers)
                LocalAccountsManagement.GetAllLocalAccounts();

        }

        private static string[] GroupsAccount(string[] groupsHandle)
        {
            var getGroup = groupsHandle.FirstOrDefault(part => part
                   .StartsWith("groups:", StringComparison.OrdinalIgnoreCase));
            if (getGroup != null)
            {
                if (getGroup.Any())
                {
                    int startIndex = getGroup.IndexOf('[') + 1;
                    int endIndex = getGroup.IndexOf(']');
                    string groupsPart = getGroup.Substring(startIndex, endIndex - startIndex);
                    var groups = groupsPart.Split('#');
                    return groups;
                }
            }
            return Array.Empty<string>();
        }

    }




}