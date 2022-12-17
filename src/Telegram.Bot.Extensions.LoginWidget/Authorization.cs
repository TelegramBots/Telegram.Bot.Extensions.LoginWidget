namespace Telegram.Bot.Extensions.TelegramLogin;

/// <summary>
/// Authorization result
/// </summary>
public enum Authorization
{
    /// <summary>
    /// Error: Invalid hash
    /// </summary>
    InvalidHash,

    /// <summary>
    /// Error: Missing fields
    /// </summary>
    MissingFields,

    /// <summary>
    /// Error: Invalid date format
    /// </summary>
    InvalidAuthDateFormat,

    /// <summary>
    /// Error: Too old
    /// </summary>
    TooOld,

    /// <summary>
    /// Valid
    /// </summary>
    Valid,
}
