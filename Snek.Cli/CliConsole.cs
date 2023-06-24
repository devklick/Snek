using System.Runtime.Versioning;
using Snek.Core;

namespace Snek.Cli;

public class CliConsole : IConsole
{
    public ConsoleColor BackgroundColor
    {
        get => Console.BackgroundColor;
        set => Console.BackgroundColor = value;
    }
    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    public bool CursorVisible
    {
        set => Console.CursorVisible = value;
    }
    public int WindowWidth
    {
        get => Console.WindowWidth;
    }

    public int WindowHeight
    {
        get => Console.WindowHeight;
    }
    public bool KeyAvailable
    {
        get => Console.KeyAvailable;
    }

    public Task Clear()
    {
        Console.Clear();
        return Task.CompletedTask;
    }

    public Task<ConsoleKeyInfo> ReadKey(bool intercept)
    {
        var key = Console.ReadKey(intercept);
        return Task.FromResult(key);
    }

    [SupportedOSPlatform("windows")]
    public Task SetBufferSize(int width, int height)
    {
        Console.SetBufferSize(width, height);
        return Task.CompletedTask;
    }

    public Task SetCursorPosition(int left, int top)
    {
        Console.SetCursorPosition(left, top);
        return Task.CompletedTask;
    }

    [SupportedOSPlatform("windows")]
    public Task SetWindowSize(int width, int height)
    {
        Console.SetWindowSize(width, height);
        return Task.CompletedTask;
    }

    public Task Write(char value)
    {
        Console.Write(value);
        return Task.CompletedTask;
    }
}