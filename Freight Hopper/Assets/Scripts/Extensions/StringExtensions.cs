public static class StringExtensions
{
    /// <summary>
    /// Incrementable string is in the format 'name-name-name #'
    /// Only space should be inbetween number and name
    /// </summary>
    public static string Increment(this string incrementableString, int arraySize)
    {
        string[] splitText = incrementableString.Split(' ');
        int currentNum = int.Parse(splitText[1]);
        return splitText[0] + ((currentNum + 1) % arraySize);
    }

    /// <summary>
    /// SetNumber string is in the format 'name-name-name #'
    /// Swaps # with provided number
    /// </summary>
    public static string SetNumber(this string incrementableString, int number)
    {
        string[] splitText = incrementableString.Split(' ');
        return splitText[0] + number;
    }
}