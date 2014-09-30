namespace Mechavian.NodeService.Stubs
{
    internal interface IEnvironmentService
    {
        bool IsUserInteractiveMode();
        string[] GetCommandLineArgs();
    }
}