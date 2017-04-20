# .NET Core C# Authorization Code Flow Tutorial - SKY API

A demonstration of the authorization code flow within SKY API, implemented using [.NET Core](https://www.microsoft.com/net/core/platform).

This code sample is a very basic example of how to interact with the Blackbaud OAuth 2.0 Service.  You are free to choose the client library and method that best suit your needs when creating production-level applications

### Live demo:
View the [demo app](https://dotnetauthcode.azurewebsites.net/) hosted on Microsoft Azure.

### Run locally:

- Download and install [.NET Core SDK](https://www.microsoft.com/net/core/)
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
dotnet restore
```
- On a Mac, type:
```
export ASPNETCORE_ENVIRONMENT=Development && dotnet run
```
- On a PC, type:
```
set ASPNETCORE_ENVIRONMENT=Development && dotnet run
```
- (PowerShell users may type:)
```
$Env:ASPNETCORE_ENVIRONMENT = "Development"
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
