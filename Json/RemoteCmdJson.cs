
public class RemoteCmdJsonConf
{
    //Server Conf
    public ServerImap ServerImap { get; set; } = new ServerImap();
    //Execution Conf
    public ParamsExecution ParamsExecution { get; set; } = new ParamsExecution();
    //Service Conf
    public ServiceConf ServiceConf{ get; set; } = new ServiceConf();

}