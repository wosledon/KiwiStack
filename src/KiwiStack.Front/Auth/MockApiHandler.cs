using System.Net;
using System.Net.Http.Json;
using KiwiStack.Front.Services;
using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Dtos.DatabaseConnection;
using KiwiStack.Shared.Dtos.EtlConnector;
using KiwiStack.Shared.Dtos.Project;
using KiwiStack.Shared.Dtos.ProjectComponent;
using KiwiStack.Shared.Dtos.User;

namespace KiwiStack.Front.Auth;

// Simple mock HTTP handler to simulate API endpoints when backend is unavailable
public class MockApiHandler(MockApiStore store) : DelegatingHandler
{
    private readonly MockApiStore _store = store;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _store.EnsureSeed();

        var path = request.RequestUri?.AbsolutePath ?? string.Empty;
        var method = request.Method.Method.ToUpperInvariant();

        try
        {
            // route: /api/v1/Project/list
            if (path.StartsWith("/api/v1/Project/list", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Get.Method)
            {
                var keyword = System.Web.HttpUtility.ParseQueryString(request.RequestUri!.Query).Get("Keyword");
                var list = _store.Projects.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(keyword))
                    list = list.Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || p.Description.Contains(keyword!, StringComparison.OrdinalIgnoreCase));
                return Ok(MockApiStore.Result(list));
            }

            // route: /api/v1/DatabaseConnection/list
            if (path.StartsWith("/api/v1/DatabaseConnection/list", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Get.Method)
            {
                var keyword = System.Web.HttpUtility.ParseQueryString(request.RequestUri!.Query).Get("Keyword");
                var list = _store.DbConns.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(keyword))
                    list = list.Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || p.Host.Contains(keyword!, StringComparison.OrdinalIgnoreCase));
                return Ok(MockApiStore.Result(list));
            }

            // route: /api/v1/ProjectComponent/list
            if (path.StartsWith("/api/v1/ProjectComponent/list", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Get.Method)
            {
                var keyword = System.Web.HttpUtility.ParseQueryString(request.RequestUri!.Query).Get("Keyword");
                var list = _store.SubProjects.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(keyword))
                    list = list.Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || p.Description.Contains(keyword!, StringComparison.OrdinalIgnoreCase));
                return Ok(MockApiStore.Result(list));
            }

            // route: /api/v1/EtlConnector/list
            if (path.StartsWith("/api/v1/EtlConnector/list", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Get.Method)
            {
                var keyword = System.Web.HttpUtility.ParseQueryString(request.RequestUri!.Query).Get("Keyword");
                var list = _store.EtlConns.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(keyword))
                    list = list.Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || p.Description.Contains(keyword!, StringComparison.OrdinalIgnoreCase));
                return Ok(MockApiStore.Result(list));
            }

            // Create/Update/Delete mocks for basic flows
            if (path.Equals("/api/v1/Project", StringComparison.OrdinalIgnoreCase))
            {
                if (method == HttpMethod.Post.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<ProjectCreateOrUpdateDto>(cancellationToken: cancellationToken) ?? new ProjectCreateOrUpdateDto();
                    var dto = new ProjectDto { Id = Guid.NewGuid(), Name = body.Name, Description = body.Description, Prefix = body.Prefix, CreatedAt = DateTime.UtcNow };
                    _store.Projects.Add(dto);
                    return Created(dto);
                }
                if (method == HttpMethod.Put.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<ProjectCreateOrUpdateDto>(cancellationToken: cancellationToken) ?? new ProjectCreateOrUpdateDto();
                    var it = _store.Projects.FirstOrDefault(x => x.Id == body.Id);
                    if (it is null) return new HttpResponseMessage(HttpStatusCode.NotFound);
                    it.Name = body.Name; it.Description = body.Description; it.Prefix = body.Prefix;
                    return NoContent();
                }
            }
            if (path.StartsWith("/api/v1/Project/", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Delete.Method)
            {
                if (Guid.TryParse(path.Split('/').Last(), out var id))
                {
                    _store.Projects.RemoveAll(x => x.Id == id);
                    return NoContent();
                }
            }

            if (path.Equals("/api/v1/DatabaseConnection", StringComparison.OrdinalIgnoreCase))
            {
                if (method == HttpMethod.Post.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<DatabaseConnectionCreateOrUpdateDto>(cancellationToken: cancellationToken) ?? new();
                    var dto = new DatabaseConnectionDto { Id = Guid.NewGuid(), Name = body.Name, DbType = body.DbType, Host = body.Host, Port = body.Port, Username = body.Username, Database = body.Database, ProjectId = body.ProjectId, CreatedAt = DateTime.UtcNow };
                    _store.DbConns.Add(dto);
                    return Created(dto);
                }
                if (method == HttpMethod.Put.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<DatabaseConnectionCreateOrUpdateDto>(cancellationToken: cancellationToken) ?? new();
                    var it = _store.DbConns.FirstOrDefault(x => x.Id == body.Id);
                    if (it is null) return new HttpResponseMessage(HttpStatusCode.NotFound);
                    it.Name = body.Name; it.DbType = body.DbType; it.Host = body.Host; it.Port = body.Port; it.Username = body.Username; it.Database = body.Database; it.ProjectId = body.ProjectId;
                    return NoContent();
                }
            }
            if (path.StartsWith("/api/v1/DatabaseConnection/", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Delete.Method)
            {
                if (Guid.TryParse(path.Split('/').Last(), out var id))
                {
                    _store.DbConns.RemoveAll(x => x.Id == id);
                    return NoContent();
                }
            }

            if (path.Equals("/api/v1/ProjectComponent", StringComparison.OrdinalIgnoreCase))
            {
                if (method == HttpMethod.Post.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<ProjectComponentCreateOrUpdateDto>(cancellationToken: cancellationToken) ?? new();
                    var dto = new ProjectComponentDto { Id = Guid.NewGuid(), Name = body.Name, Description = body.Description, Group = body.Group, Version = body.Version, ProjectId = body.ProjectId, CreatedAt = DateTime.UtcNow };
                    _store.SubProjects.Add(dto);
                    return Created(dto);
                }
                if (method == HttpMethod.Put.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<ProjectComponentCreateOrUpdateDto>(cancellationToken: cancellationToken) ?? new();
                    var it = _store.SubProjects.FirstOrDefault(x => x.Id == body.Id);
                    if (it is null) return new HttpResponseMessage(HttpStatusCode.NotFound);
                    it.Name = body.Name; it.Description = body.Description; it.Group = body.Group; it.Version = body.Version; it.ProjectId = body.ProjectId;
                    return NoContent();
                }
            }
            if (path.StartsWith("/api/v1/ProjectComponent/", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Delete.Method)
            {
                if (Guid.TryParse(path.Split('/').Last(), out var id))
                {
                    _store.SubProjects.RemoveAll(x => x.Id == id);
                    return NoContent();
                }
            }

            if (path.Equals("/api/v1/EtlConnector", StringComparison.OrdinalIgnoreCase))
            {
                if (method == HttpMethod.Post.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<EtlConnectorCreateOrUpdateDto>(cancellationToken: cancellationToken) ?? new();
                    var dto = new EtlConnectorDto { Id = Guid.NewGuid(), Name = body.Name, Description = body.Description, SourceDatabaseConnectionId = body.SourceDatabaseConnectionId, TargetDatabaseConnectionId = body.TargetDatabaseConnectionId, ProjectComponentId = body.ProjectComponentId, DataX = body.DataX, CreatedAt = DateTime.UtcNow };
                    _store.EtlConns.Add(dto);
                    return Created(dto);
                }
                if (method == HttpMethod.Put.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<EtlConnectorCreateOrUpdateDto>(cancellationToken: cancellationToken) ?? new();
                    var it = _store.EtlConns.FirstOrDefault(x => x.Id == body.Id);
                    if (it is null) return new HttpResponseMessage(HttpStatusCode.NotFound);
                    it.Name = body.Name; it.Description = body.Description; it.SourceDatabaseConnectionId = body.SourceDatabaseConnectionId; it.TargetDatabaseConnectionId = body.TargetDatabaseConnectionId; it.ProjectComponentId = body.ProjectComponentId; it.DataX = body.DataX;
                    return NoContent();
                }
            }
            if (path.StartsWith("/api/v1/EtlConnector/", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Delete.Method)
            {
                if (Guid.TryParse(path.Split('/').Last(), out var id))
                {
                    _store.EtlConns.RemoveAll(x => x.Id == id);
                    return NoContent();
                }
            }

            if (path.StartsWith("/api/v1/User/list", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Get.Method)
            {
                var keyword = System.Web.HttpUtility.ParseQueryString(request.RequestUri!.Query).Get("Keyword");
                var list = _store.Users.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(keyword))
                    list = list.Where(p => p.Account.Contains(keyword, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(keyword!, StringComparison.OrdinalIgnoreCase));
                return Ok(MockApiStore.Result(list));
            }
            if (path.Equals("/api/v1/User", StringComparison.OrdinalIgnoreCase))
            {
                if (method == HttpMethod.Post.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<UserCreateDto>(cancellationToken: cancellationToken) ?? new();
                    var dto = new UserDto { Id = Guid.NewGuid(), Account = body.Account, Name = body.Name, Email = body.Email, Role = body.Role, CreatedAt = DateTime.UtcNow };
                    _store.Users.Add(dto);
                    return Created(dto);
                }
                if (method == HttpMethod.Put.Method)
                {
                    var body = await request.Content!.ReadFromJsonAsync<UserUpdateDto>(cancellationToken: cancellationToken) ?? new();
                    var it = _store.Users.FirstOrDefault(x => x.Id == body.Id);
                    if (it is null) return new HttpResponseMessage(HttpStatusCode.NotFound);
                    it.Name = body.Name; it.Email = body.Email; if (!string.IsNullOrWhiteSpace(body.Password)) { /* just ignore */ } it.Role = body.Role;
                    return NoContent();
                }
            }
            if (path.StartsWith("/api/v1/User/", StringComparison.OrdinalIgnoreCase) && method == HttpMethod.Delete.Method)
            {
                if (Guid.TryParse(path.Split('/').Last(), out var id))
                {
                    _store.Users.RemoveAll(x => x.Id == id);
                    return NoContent();
                }
            }
        }
        catch
        {
            // fallthrough to real backend if parsing fails
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static HttpResponseMessage Ok<T>(KiwiResult<T> result) where T : class
        => new(HttpStatusCode.OK) { Content = JsonContent.Create(result) };
    private static HttpResponseMessage Created<T>(T dto) where T : class
        => new(HttpStatusCode.Created) { Content = JsonContent.Create(dto) };
    private static HttpResponseMessage NoContent()
        => new(HttpStatusCode.NoContent);
}
