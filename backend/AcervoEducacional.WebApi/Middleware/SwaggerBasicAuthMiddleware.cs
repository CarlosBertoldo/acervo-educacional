using System.Text;

public class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _username = "admin";
    private readonly string _password = "Swagger123";

    public SwaggerBasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var encoding = Encoding.GetEncoding("iso-8859-1");
                var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var split = usernamePassword.Split(':');

                if (split[0] == _username && split[1] == _password)
                {
                    await _next.Invoke(context);
                    return;
                }
            }

            context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger UI\"";
            context.Response.StatusCode = 401;
            return;
        }

        await _next.Invoke(context);
    }
}
