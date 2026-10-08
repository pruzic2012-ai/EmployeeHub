using System.Net.Http.Headers;

namespace EmployeeHub.Blazor.Services;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public JwtAuthorizationHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _tokenService.Token;

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}