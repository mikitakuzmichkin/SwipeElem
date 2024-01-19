namespace SO
{
    public interface ILevels
    {
        int[,] this[int index] { get; }
        int Count { get; }
    }
}