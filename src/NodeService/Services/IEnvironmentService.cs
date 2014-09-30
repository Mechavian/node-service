namespace Mechavian.NodeService.Services
{
    internal interface IEnvironmentService
    {
        bool IsUserInteractiveMode();
        string[] GetCommandLineArgs();
    }
}