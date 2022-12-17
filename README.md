# Telegram bots Login Widget

![Build Status](https://github.com/karb0f0s/Telegram.Bot.Extensions.LoginWidget/actions/workflows/ci.yml/badge.svg)


Makes it simple to validate login widget authorization hashes

Built according to specifications published on [Telegram's website](https://core.telegram.org/widgets/login)

## Usage

```c#
// Parsed from the query string / from the callback object
Dictionary<string, string> fields = QueryStringFields;

LoginWidget loginWidget = new LoginWidget("your API access Token");
if (loginWidget.CheckAuthorization(fields) == Authorization.Valid)
{
    // ...
}
```

## Installation

Install as [NuGet package](https://www.nuget.org/packages/Telegram.Bot.Extensions.LoginWidget/):

Package manager:

```powershell
Install-Package Telegram.Bot.Extensions.LoginWidget
```
