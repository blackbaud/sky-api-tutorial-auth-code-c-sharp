# SKY API Authorization Code Flow Tutorial

### Run locally:

- Download and install [.NET Core SDK](https://www.microsoft.com/net/core)
- Open Terminal/Command Prompt and type:
```
$  git clone https://github.com/blackbaud/sky-api-auth-tutorial-c-sharp.git
$  cd sky-api-auth-tutorial-c-sharp
```
- Duplicate **appsettings.json-sample** as **appsettings.Development.json** and fill in the missing values (all required).
```
{
    "AppSettings": {
        "AuthClientId": "<Your developer app ID>",
        "AuthClientSecret": "<Your developer app secret>",
        "AuthRedirectUri": "http://localhost:5000/auth/callback",
        "AuthSubscriptionKey": "<Your developer subscription key>"
    }
}
```
- Open Terminal/Command Prompt and type:
```
$  dotnet restore
```
- On a Mac, type:
```
$  export ASPNETCORE_ENVIRONMENT=Development && dotnet run
```
- On a PC, type:
```
$  set ASPNETCORE_ENVIRONMENT=Development && dotnet run
```

Visit [http://localhost:5000/](http://localhost:5000/).

### Deploy to Azure App Services:
1. Login to Azure Portal and create a new App Service
1. Under the app's Application Settings, add the following settings:
    <table>
        <tr>
            <td>`AppSettings:AuthClientId`</td>
            <td>Your registered application's Application ID</td>
        </tr>
        <tr>
            <td>`AppSettings:AuthClientSecret`</td>
            <td>Your registered application's Application Secret</td>
        </tr>
        <tr>
            <td>`AppSettings:AuthRedirectUri`</td>
            <td>The respective URI listed under your registered application's Redirect URIs</td>
        </tr>
        <tr>
            <td>`AppSettings:AuthSubscriptionKey`</td>
            <td>Your Blackbaud Developer Subscription Key</td>
        </tr>
    </table>
1. Add a new deployment to point to a cloned version of this repository and sync.

### Additional Reading
- [Azure App Service and ASP.NET Core RC2](https://blogs.msdn.microsoft.com/appserviceteam/2016/05/24/azure-app-service-and-asp-net-core-rc2/)
- [Your First ASP.NET Core Application on a Mac Using Visual Studio Code](https://docs.asp.net/en/latest/tutorials/your-first-mac-aspnet.html#publishing-to-azure)