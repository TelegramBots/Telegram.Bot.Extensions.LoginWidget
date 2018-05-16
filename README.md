# Telegram bots Login Widget
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
