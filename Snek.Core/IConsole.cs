namespace Snek.Core;

public interface IConsole
{
    public ConsoleColor BackgroundColor { get; set; }
    public ConsoleColor ForegroundColor { get; set; }
    public bool CursorVisible { set; }
    public int WindowWidth { get; }
    public int WindowHeight { get; }
    public bool KeyAvailable { get; }
    public Task SetWindowSize(int width, int height);
    public Task SetBufferSize(int width, int height);
    public Task SetCursorPosition(int left, int top);
    public Task Write(char value);
    public Task Clear();
    public Task<ConsoleKeyInfo> ReadKey(bool intercept);
}