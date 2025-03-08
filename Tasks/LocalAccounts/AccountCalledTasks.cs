namespace remoteCmd.Tasks.LocalAccounts
{
    public static class CalledAccountTasks
    {
        public static void ActionPreDefinedsToExecute(string body, AppSettings _appSettings)
        {
            var ChangePassword = body.Contains("ChangePassword", StringComparison.OrdinalIgnoreCase);
            if (ChangePassword)
            {
                bool pwdExpires = false;
                bool pwdNeverExpires = false;

                if (body.Contains("expires", StringComparison.OrdinalIgnoreCase))
                    pwdExpires = true;

                if (body.Contains("never", StringComparison.OrdinalIgnoreCase))
                    pwdNeverExpires = true;

                var userName_NewPassword = body.Split("|");

                LocalAccountsManagement.ChangePassword(userName_NewPassword[1], userName_NewPassword[2], pwdExpires, pwdNeverExpires);
            }

            var CreateLocalAccountDefault = body.Contains("CreateLocalAccountDefault", StringComparison.OrdinalIgnoreCase);
            if (CreateLocalAccountDefault)
            {
                var userName_NewPassword = body.Split("|");

                bool pwdExpires = false;
                bool pwdNeverExpires = false;

                if (body.Contains("Groups", StringComparison.OrdinalIgnoreCase))
                {
                    if (body.Contains("expires", StringComparison.OrdinalIgnoreCase))
                        pwdExpires = true;

                    if (body.Contains("never", StringComparison.OrdinalIgnoreCase))
                        pwdNeverExpires = true;

                    if (GroupsAccount(userName_NewPassword).Any())
                        LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], GroupsAccount(userName_NewPassword), pwdExpires, pwdNeverExpires);
                    else
                        LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], Array.Empty<string>(), pwdExpires, pwdNeverExpires);
                }

            }


            var CreateLocalAccountPasswordNeverExpires = body.Contains("CreateLocalAccountPasswordNeverExpires", StringComparison.OrdinalIgnoreCase);
            if (CreateLocalAccountPasswordNeverExpires)
            {
                var userName_NewPassword = body.Split("|");

                if (GroupsAccount(userName_NewPassword).Any())
                    LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], GroupsAccount(userName_NewPassword), false, true);
                else
                    LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], Array.Empty<string>(), false, true);
            }


            var GetAllUsers = body.Contains("GetAllUsers", StringComparison.OrdinalIgnoreCase);
            if (GetAllUsers)
            {
                var userName_NewPassword = body.Split("|");
                LocalAccountsManagement.GetAllLocalAccounts();
            }


            var DisableAccount = body.Contains("DisableAccount", StringComparison.OrdinalIgnoreCase);
            if (DisableAccount)
            {
                var userName = body.Split("|");
                LocalAccountsManagement.EnableDisableAccount(userName[1], false);
            }

            var EnableAccount = body.Contains("EnableAccount", StringComparison.OrdinalIgnoreCase);
            if (EnableAccount)
            {
                var userName = body.Split("|");
                LocalAccountsManagement.EnableDisableAccount(userName[1], true);
            }

        }

        private static string[] GroupsAccount(string[] groupsHandle)
        {
            var getGroup = groupsHandle.FirstOrDefault(part => part
                   .StartsWith("groups:", StringComparison.OrdinalIgnoreCase));


            if (getGroup != null)
            {
                int startIndex = getGroup.IndexOf('[') + 1;
                int endIndex = getGroup.IndexOf(']');
                string groupsPart = getGroup.Substring(startIndex, endIndex - startIndex);
                var groups = groupsPart.Split('#');
                return groups;
            }
            return Array.Empty<string>();
        }

    }




}