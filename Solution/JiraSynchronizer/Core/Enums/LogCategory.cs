namespace JiraSynchronizer.Core.Enums;

public enum LogCategory
{
    Information = 100,
    LogfileInitialized = 101,
    ApplicationStarted = 102,
    ApplicationStopped = 199,
    Success = 200,
    Warning = 300,
    OvertimeWarning = 301,
    ApplicationAborted = 302,
    Error = 400,
    UserNotFound = 401,
    UserNotAuthorized = 402,
}
