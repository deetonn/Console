namespace Console.Commands;

public static class CommandReturnValues
{
    private const int Crv = 9000;

    /// <summary>
    /// The command requests that Terminal doesn't
    /// write the default translation message.
    /// </summary>
    public const int DontShowText = Crv - 1;

    /// <summary>
    /// Some command request that the application exit safely.
    /// </summary>
    public const int SafeExit = Crv + 1;

    /// <summary>
    /// The quested command does not exist.
    /// </summary>
    public const int NoSuchCommand = Crv + 2;

    /// <summary>
    /// A <see cref="PathFileCommand"/> failed to open the process.
    /// </summary>
    public const int FailedToStartProcess = Crv + 3;

    public const int BadArguments = Crv + 0xF;


    /// <summary>
    /// The command is an invalid type to queue.
    /// </summary>
    public const int CQueueBadType = Crv + 400;

    public const int CQueueNotFound = Crv + 401;
}