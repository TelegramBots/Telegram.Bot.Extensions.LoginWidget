using System.Xml.Linq;
using Xunit;

namespace Telegram.Bot.Extensions.TelegramLogin.Tests.Unit;

public class WidgetTests
{
    [Fact]
    public void Test_Generate_Callback_Embed()
    {
        string result = WidgetEmbedCodeGenerator.GenerateCallbackEmbedCode(
            botName: "samplebot",
            callbackFunctionName: "onTelegramAuth",
            callbackParameterName: "user");

        Assert.Contains("data-telegram-login=\"samplebot\"", result);
        Assert.Contains("data-onauth=\"onTelegramAuth(user)\"", result);
    }

    [Fact]
    public void Test_Generate_Redirect_Embed()
    {
        string result = WidgetEmbedCodeGenerator.GenerateRedirectEmbedCode(
            botName: "samplebot",
            redirectUrl: "http://example.com/callback");

        Assert.Contains("data-telegram-login=\"samplebot\"", result);
        Assert.Contains("data-auth-url=\"http://example.com/callback\"", result);
    }
}
