namespace Telegram.Bot.Extensions.LoginWidget
{
    /// <summary>
    /// Generates JavaScript embed code matching the one found on https://core.telegram.org/widgets/login
    /// </summary>
    public class WidgetEmbedCodeGenerator
    {
        /// <summary>
        /// Defaults to 5
        /// </summary>
        public static int LoginWidgetJsVersion = 5;

        private WidgetEmbedCodeGenerator() { }

        /// <summary>
        /// Generate the embed code that uses a callback function to signal user login
        /// </summary>
        /// <param name="botName">Name of your Telegram bot</param>
        /// <param name="callbackFunctionName">Name of the callback function (ex. onUserLogin)</param>
        /// <param name="callbackParameterName">Name of the parameter in the callback function (ex. user -> onUserLogin(user))</param>
        /// <param name="buttonStyle">Size of the login button</param>
        /// <param name="showUserPhoto">Show to user photo next to the login button</param>
        /// <param name="requestAccess">Request access for your bot to message the user</param>
        /// <returns></returns>
        public static string GenerateCallbackEmbedCode(
            string botName,
            string callbackFunctionName,
            string callbackParameterName,
            ButtonStyle buttonStyle = ButtonStyle.Large,
            bool showUserPhoto = true,
            bool requestAccess = true)
        {
            return GenerateBaseEmbedCode(botName, buttonStyle, showUserPhoto, requestAccess, string.Concat("data-onauth=\"", callbackFunctionName, "(", callbackParameterName, ")\""));
        }

        /// <summary>
        /// Generate the embed code that redirects you to the url you specify with parameters in the query string
        /// </summary>
        /// <param name="botName">Name of your Telegram bot</param>
        /// <param name="redirectUrl">The url to redirect the user to on login</param>
        /// <param name="buttonStyle">Size of the login button</param>
        /// <param name="showUserPhoto">Show to user photo next to the login button</param>
        /// <param name="requestAccess">Request access for your bot to message the user</param>
        /// <returns></returns>
        public static string GenerateRedirectEmbedCode(
            string botName,
            string redirectUrl,
            ButtonStyle buttonStyle = ButtonStyle.Large,
            bool showUserPhoto = true,
            bool requestAccess = true)
        {
            return GenerateBaseEmbedCode(botName, buttonStyle, showUserPhoto, requestAccess, $"data-auth-url=\"{redirectUrl}\"");
        }

        private static string GenerateBaseEmbedCode(string botName, ButtonStyle buttonStyle, bool showUserPhoto, bool requestAccess, string data_auth)
        {
            return string.Concat(
                "<script async src=\"https://telegram.org/js/telegram-widget.js?",
                LoginWidgetJsVersion, "\" data-telegram-login=\"",
                botName, "\" data-size=\"" + buttonStyle.ToString().ToLower(), "\" ",
                (showUserPhoto ? "" : "data-userpic=\"false\" "),
                (requestAccess ? "data-request-access=\"write\" " : ""),
                data_auth, "></script>");
        }
    }
}
