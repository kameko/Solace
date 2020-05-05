
TODO:
 - Make a script to start up all services. Either PowerShell or Deno.
 - Security; Use security tokens/encryption in request/response headers between services.
 
NOTES:
 - OData queries are case-sensitive, use tolower() to remove case sensitivity: ?$filter=contains(tolower(message),%27test%27)
 - Most of the 2000 port range is free. Skip 2370-2390. 61_000 and 62_000 is entirely free.