namespace Assessment
{
    public static class ServiceHelper
    {
        public static IServiceProvider Services =>
            IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("Service provider not initialized");

        public static T GetService<T>() where T : class =>
            Services.GetRequiredService<T>();
    }
}
