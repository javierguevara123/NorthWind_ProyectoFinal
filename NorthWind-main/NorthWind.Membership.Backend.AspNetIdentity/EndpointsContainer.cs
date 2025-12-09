namespace Microsoft.AspNetCore.Builder;
public static class EndpointsContainer
{
    public static WebApplication UseMembershipUsersEndpoints(this WebApplication app)
    {
        app.UsePerformanceUsersController();

        return app;
    }
}
