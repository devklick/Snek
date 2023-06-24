using System.Text;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Snek.Core;

namespace Snek.Web;

public class WebConsole : IConsole
{
    public ConsoleColor BackgroundColor { get; set; }
    public ConsoleColor ForegroundColor { get; set; }
    public bool CursorVisible { set { } }
    public int WindowWidth { get; set; } = 30;
    public int WindowHeight { get; set; } = 30;
    private const int _delay = 1; // milliseconds

    public bool KeyAvailable => _inputs.Any();
    public Action? TriggerRefresh;
    public bool StateHasChanged { get; set; } = true;
    private Position _cursorPosition = Position.Default;

    private Cell[,] View;

    private readonly Queue<ConsoleKeyInfo> _inputs = new();

    public WebConsole()
    {
        View = new Cell[WindowHeight, WindowWidth];
    }
    public async Task Clear()
    {
        for (int row = 0; row < View.GetLength(0); row++)
        {
            for (int column = 0; column < View.GetLength(1); column++)
            {
                View[row, column] = new Cell(row, column, BackgroundColor, ForegroundColor, ' ');
            }
        }
        await Task.CompletedTask;
    }

    public async Task<ConsoleKeyInfo> ReadKey(bool intercept)
    {
        while (!KeyAvailable)
        {
            await Refresh();
        }
        return ReadKeyNoRefresh(intercept);
    }

    public async Task SetBufferSize(int width, int height)
    {
        await Task.CompletedTask;
    }

    public async Task SetCursorPosition(int left, int top)
    {
        _cursorPosition = new Position(left, top);
        await Task.CompletedTask;
    }

    public async Task SetWindowSize(int width, int height)
    {
        await Task.CompletedTask;
    }

    public async Task Write(char value)
    {
        View[_cursorPosition.Y, _cursorPosition.X] = new Cell(_cursorPosition.X, _cursorPosition.Y, BackgroundColor, ForegroundColor, value);
        StateHasChanged = true;
        await Refresh();
    }

    private ConsoleKeyInfo ReadKeyNoRefresh(bool capture)
    {
        if (!KeyAvailable)
        {
            throw new InvalidOperationException("attempting a no refresh ReadKey with an empty input buffer");
        }
        var keyInfo = _inputs.Dequeue();

        return keyInfo;
    }
    public async Task Refresh()
    {
        if (StateHasChanged)
        {
            TriggerRefresh?.Invoke();
        }
        await Task.Delay(_delay);
    }

    public void EnqueueInput(ConsoleKey key, bool shift = false, bool alt = false, bool control = false)
    {
        char c = key switch
        {
            >= ConsoleKey.A and <= ConsoleKey.Z => (char)(key - ConsoleKey.A + 'a'),
            >= ConsoleKey.D0 and <= ConsoleKey.D9 => (char)(key - ConsoleKey.D0 + '0'),
            ConsoleKey.Enter => '\n',
            ConsoleKey.Backspace => '\b',
            ConsoleKey.OemPeriod => '.',
            ConsoleKey.OemMinus => '-',
            _ => '\0',
        };
        _inputs.Enqueue(new(shift ? char.ToUpper(c) : c, key, shift, alt, control));
    }

    public void OnKeyDown(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "Home": EnqueueInput(ConsoleKey.Home); break;
            case "End": EnqueueInput(ConsoleKey.End); break;
            case "Backspace": EnqueueInput(ConsoleKey.Backspace); break;
            case " ": EnqueueInput(ConsoleKey.Spacebar); break;
            case "Delete": EnqueueInput(ConsoleKey.Delete); break;
            case "Enter": EnqueueInput(ConsoleKey.Enter); break;
            case "Escape": EnqueueInput(ConsoleKey.Escape); break;
            case "ArrowLeft": EnqueueInput(ConsoleKey.LeftArrow); break;
            case "ArrowRight": EnqueueInput(ConsoleKey.RightArrow); break;
            case "ArrowUp": EnqueueInput(ConsoleKey.UpArrow); break;
            case "ArrowDown": EnqueueInput(ConsoleKey.DownArrow); break;
            case ".": EnqueueInput(ConsoleKey.OemPeriod); break;
            case "-": EnqueueInput(ConsoleKey.OemMinus); break;
            default:
                if (e.Key.Length is 1)
                {
                    char c = e.Key[0];
                    switch (c)
                    {
                        case >= '0' and <= '9': EnqueueInput(ConsoleKey.D0 + (c - '0')); break;
                        case >= 'a' and <= 'z': EnqueueInput(ConsoleKey.A + (c - 'a')); break;
                        case >= 'A' and <= 'Z': EnqueueInput(ConsoleKey.A + (c - 'A'), shift: true); break;
                    }
                }
                break;
        }
    }

    public MarkupString State
    {
        get
        {
            StringBuilder stateBuilder = new();
            for (int row = 0; row < View.GetLength(0); row++)
            {
                for (int column = 0; column < View.GetLength(1); column++)
                {
                    if (View[row, column].BackgroundColor is not ConsoleColor.Black)
                    {
                        stateBuilder.Append($@"<span style=""background-color:{HtmlEncode(View[row, column].BackgroundColor)}"">");
                    }
                    if (View[row, column].SpriteColor is not ConsoleColor.White)
                    {
                        stateBuilder.Append($@"<span style=""color:{HtmlEncode(View[row, column].SpriteColor)}"">");
                    }
                    stateBuilder.Append(HttpUtility.HtmlEncode(View[row, column].Sprite));
                    if (View[row, column].SpriteColor is not ConsoleColor.White)
                    {
                        stateBuilder.Append("</span>");
                    }
                    if (View[row, column].BackgroundColor is not ConsoleColor.Black)
                    {
                        stateBuilder.Append("</span>");
                    }
                    if ((_cursorPosition.X, _cursorPosition.Y) == (column, row))
                    {
                        stateBuilder.Append("</span>");
                    }
                }
                stateBuilder.Append("<br />");
            }
            string state = stateBuilder.ToString();
            StateHasChanged = false;
            return (MarkupString)state;
        }
    }

    private static string HtmlEncode(ConsoleColor color) => color switch
    {
        ConsoleColor.Black => "#000000",
        ConsoleColor.White => "#ffffff",
        ConsoleColor.Blue => "#0000ff",
        ConsoleColor.Red => "#ff0000",
        ConsoleColor.Green => "#00ff00",
        ConsoleColor.Yellow => "#ffff00",
        ConsoleColor.Cyan => "#00ffff",
        ConsoleColor.Magenta => "#ff00ff",
        ConsoleColor.Gray => "#808080",
        ConsoleColor.DarkBlue => "#00008b",
        ConsoleColor.DarkRed => "#8b0000",
        ConsoleColor.DarkGreen => "#006400",
        ConsoleColor.DarkYellow => "#8b8000",
        ConsoleColor.DarkCyan => "#008b8b",
        ConsoleColor.DarkMagenta => "#8b008b",
        ConsoleColor.DarkGray => "#a9a9a9",
        _ => throw new NotImplementedException(),
    };
}