//   if (_singleMessage.Body.Contains("AdvancedElevatedExecution"))
//                     {
//                         Console.WriteLine($"Advanced Elevated Execution - was received. Script {filePath} will execute in the next reboot.");
//                         EventLog.WriteEntry("RemoteCmd", $"Advanced Elevated Execution - was received. Script {filePath} will execute in the next reboot.", EventLogEntryType.Information);

//                         var userNamePassword = HttpUtility.HtmlDecode(_singleMessage.Body).Split('|');

//                         Advanced.GetSaveCurrentUserNameLogged();
//                         Advanced.ExecuteScriptElevatedAfterLogon(filePath);
//                         Advanced.ConfigureAutoLogon(filePath, _appSettings, userNamePassword[1], userNamePassword[2], userNamePassword[3]);
//                         BasicsManagement.Reboot(_appSettings);
//                     }

//                     if (_singleMessage.Body.Contains("PowershellScriptRun"))
//                         Advanced.PowershellScriptRun(filePath, _appSettings);

//                     if (_singleMessage.Body.Contains("ScheduleBasicTaskPowerShellScript"))
//                         Advanced.ScheduleBasicTaskPowerShellScript(filePath, _appSettings);

//                     if (_singleMessage.Body.Contains("ScheduleTaskHighestPowerShellScript"))
//                     {
//                         var userNamePassword = HttpUtility.HtmlDecode(_singleMessage.Body).Split('|');
//                         Advanced.ScheduleTaskHighestPowerShellScript(filePath, _appSettings, userNamePassword[1], userNamePassword[2]);
//                     }

//                        var shutdown = body.Contains("shutdown", StringComparison.OrdinalIgnoreCase);
//         if (shutdown)
//             BasicsManagement.Shutdown(_appSettings);

//         var logoff = body.Contains("logoff", StringComparison.OrdinalIgnoreCase);
//         if (logoff)
//             BasicsManagement.Logoff(_appSettings);

//         var reboot = body.Contains("reboot", StringComparison.OrdinalIgnoreCase);
//         if (reboot)
//             BasicsManagement.Reboot(_appSettings);
       
//         var getScreen = body.Contains("getScreen", StringComparison.OrdinalIgnoreCase);
//         if (getScreen)
//             GetScreen.GetPrintScreen(_appSettings);

//               var ChangePassword = body.Contains("ChangePassword", StringComparison.OrdinalIgnoreCase);
//             if (ChangePassword)
//             {
//                 bool pwdExpires = false;
//                 bool pwdNeverExpires = false;

//                 if (body.Contains("expires", StringComparison.OrdinalIgnoreCase))
//                     pwdExpires = true;

//                 if (body.Contains("never", StringComparison.OrdinalIgnoreCase))
//                     pwdNeverExpires = true;

//                 var userName_NewPassword = body.Split("|");

//                 LocalAccountsManagement.ChangePassword(userName_NewPassword[1], userName_NewPassword[2], pwdExpires, pwdNeverExpires);
//             }

//             var CreateLocalAccountDefault = body.Contains("CreateLocalAccountDefault", StringComparison.OrdinalIgnoreCase);
//             if (CreateLocalAccountDefault)
//             {
//                 var userName_NewPassword = body.Split("|");

//                 bool pwdExpires = false;
//                 bool pwdNeverExpires = false;

//                 if (body.Contains("Groups", StringComparison.OrdinalIgnoreCase))
//                 {
//                     if (body.Contains("expires", StringComparison.OrdinalIgnoreCase))
//                         pwdExpires = true;

//                     if (body.Contains("never", StringComparison.OrdinalIgnoreCase))
//                         pwdNeverExpires = true;

//                     if (GroupsAccount(userName_NewPassword).Any())
//                         LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], GroupsAccount(userName_NewPassword), pwdExpires, pwdNeverExpires);
//                     else
//                         LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], Array.Empty<string>(), pwdExpires, pwdNeverExpires);
//                 }

//             }


//             var CreateLocalAccountPasswordNeverExpires = body.Contains("CreateLocalAccountPasswordNeverExpires", StringComparison.OrdinalIgnoreCase);
//             if (CreateLocalAccountPasswordNeverExpires)
//             {
//                 var userName_NewPassword = body.Split("|");

//                 if (GroupsAccount(userName_NewPassword).Any())
//                     LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], GroupsAccount(userName_NewPassword), false, true);
//                 else
//                     LocalAccountsManagement.CreateLocalAccount(userName_NewPassword[1], userName_NewPassword[2], Array.Empty<string>(), false, true);
//             }


//             var GetAllUsers = body.Contains("GetAllUsers", StringComparison.OrdinalIgnoreCase);
//             if (GetAllUsers)
//             {
//                 var userName_NewPassword = body.Split("|");
//                 LocalAccountsManagement.GetAllLocalAccounts();
//             }


//             var DisableAccount = body.Contains("DisableAccount", StringComparison.OrdinalIgnoreCase);
//             if (DisableAccount)
//             {
//                 var userName = body.Split("|");
//                 LocalAccountsManagement.EnableDisableAccount(userName[1], false);
//             }

//             var EnableAccount = body.Contains("EnableAccount", StringComparison.OrdinalIgnoreCase);
//             if (EnableAccount)
//             {
//                 var userName = body.Split("|");
//                 LocalAccountsManagement.EnableDisableAccount(userName[1], true);
//             }
//  bool logsAttached =
//         body.Contains("EventLogs(Application)", StringComparison.OrdinalIgnoreCase)
//         ||
//         body.Contains("EventLogs(System)", StringComparison.OrdinalIgnoreCase)
//         ||
//         body.Contains("EventLogs(Security)", StringComparison.OrdinalIgnoreCase);
//  var softwareReport = body.Contains("softwareReport", StringComparison.OrdinalIgnoreCase);
//         if (softwareReport)
//         {
//             SoftwareManagement.GetListAllInstalledSoftware(_appSettings);
//         }
//            var getIpAll = body.Contains("getIpAll", StringComparison.OrdinalIgnoreCase);
//         if (getIpAll)
//             NetworkManagement.GetIpAll(_appSettings);

//         var firewallDisable = body.Contains("firewallDisable", StringComparison.OrdinalIgnoreCase);
//         if (firewallDisable)
//             NetworkManagement.FirewallEnableDisable(false, _appSettings);

//         var firewallEnable = body.Contains("firewallEnable", StringComparison.OrdinalIgnoreCase);
//         if (firewallEnable)
//             NetworkManagement.FirewallEnableDisable(true, _appSettings);

//         var getFirewallState = body.Contains("getFirewallState", StringComparison.OrdinalIgnoreCase);
//         if (getFirewallState)
//             NetworkManagement.GetFirewallState(_appSettings);

//         var RemoteDesktopEnable = body.Contains("RemoteDesktopEnable", StringComparison.OrdinalIgnoreCase);
//         if (RemoteDesktopEnable)
//             NetworkManagement.RemoteDesktopEnableDisable(0, _appSettings);

//         var RemoteDesktopDisable = body.Contains("RemoteDesktopDisable", StringComparison.OrdinalIgnoreCase);
//         if (RemoteDesktopDisable)
//             NetworkManagement.RemoteDesktopEnableDisable(1, _appSettings);
//              var hardware = body.Contains("hardware", StringComparison.OrdinalIgnoreCase);
//         if (hardware)
//             Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Hardware Report - {Environment.MachineName} - {DateTime.Now}", await HardwareManagement.GetHardwareReportAsync(), "", _appSettings);
