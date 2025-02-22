public class Appsettings
{
    //Server Conf
    public ServerImap ServerImap { get; set; } = new ServerImap();
    public ServerSmtp ServerSmtp { get; set; } = new ServerSmtp();
    //Execution Conf
    public ParamsExecution ParamsExecution { get; set; } = new ParamsExecution();
    //Service Conf
    public ServiceConf ServiceConf { get; set; } = new ServiceConf();
    // public AppSettingsPath AppSettingsPath { get; set; } = new AppSettingsPath();

}