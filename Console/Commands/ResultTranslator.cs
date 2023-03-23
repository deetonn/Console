namespace Console.Commands;

public static class Result
{
    public static string Translate(int result)
    {
        return result switch
        {
            CommandReturnValues.DontShowText => string.Empty,
            PathFileCommand.FailedToStartProcess => $"failed to start process ({result})",
            PathFileCommand.FileMovedOrDeleted => $"the file no longer exists ({result})",
            CommandReturnValues.SafeExit => $"the application is about to exit ({result})",
            CommandReturnValues.BadArguments => $"the previous command was supplied invalid arguments.",
            CommandReturnValues.NoSuchCommand => $"that command does not exist.",
            CommandReturnValues.CQueueBadType => $"a command of that type cannot be queued.",
            CommandReturnValues.CQueueNotFound => $"cannot dequeue that command, we could not resolve it.",
            CommandReturnValues.FailedToStartProcess => $"failed to start that process.",
            int.MinValue => $"the command was not found",
            >= 0 => $"the command was successful ({result})",
            < 0 => $"the command failed ({result})"
        };
    }
}