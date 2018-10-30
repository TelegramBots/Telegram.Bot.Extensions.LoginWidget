# Telegram bots Login Widget

[![Build Status](https://travis-ci.org/MihaZupan/Telegram.Bot.Extensions.LoginWidget.svg?branch=master)](https://travis-ci.org/MihaZupan/Telegram.Bot.Extensions.LoginWidget)
[![Build status](https://ci.appveyor.com/api/projects/status/720b19vgdhro14o5/branch/master?svg=true)](https://ci.appveyor.com/project/MihaZupan/telegram-bot-extensions-loginwidget/branch/master)

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
