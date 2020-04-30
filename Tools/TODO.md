
 - Implement sending requests: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0 
 - Implement a system for sending a confirmation response when fetching data, and resending data if a service didn't get a confirmation response.
 - Security; Use security tokens/encryption in request/response headers between services.
 - In one of the frontends, implement moving one or all services to another desktop: https://stackoverflow.com/questions/31801402/api-for-windows-10-virtual-desktops . Note that this API only works for windows a process owns, so we'll have to make some conditional build steps for hosting on Windows.
 
NOTES:
 - OData queries are case-sensitive, use tolower() to remove case sensitivity: ?$filter=contains(tolower(message),%27test%27)
