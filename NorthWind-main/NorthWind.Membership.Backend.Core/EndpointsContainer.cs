namespace Microsoft.AspNetCore.Builder;
public static class EndpointsContainer
{
    public static WebApplication UseMembershipEndpoints(this WebApplication app)
    {
        app.UseUserRegistrationController();
        app.UseUserLoginController();
        app.UseUserManagementController();

        return app;
    }
}
