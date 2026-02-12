namespace Snek.Core.Extensions;

public static class EnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource> source)
    {
        public bool IsEmpty => !source.Any();
    }
}