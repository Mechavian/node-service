namespace System
{
    internal static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            object service = serviceProvider.GetService(typeof (T));
            return service as T;
        }
    }
}