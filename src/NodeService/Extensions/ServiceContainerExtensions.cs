namespace System.ComponentModel.Design
{
    internal static class ServiceContainerExtensions
    {
        public static void AddService<T>(this IServiceContainer serviceContainer, Func<IServiceProvider, T> serviceCreateCallback)
        {
            serviceContainer.AddService(typeof (T), (c, t) => serviceCreateCallback(c));
        }

        public static void AddService<T>(this IServiceContainer serviceContainer, T service)
        {
            serviceContainer.AddService(typeof(T), service);
        }
    }
}