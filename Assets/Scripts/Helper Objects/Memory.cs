/// <summary>
/// Inspired from autosplitting language, just holds the previous value and the current value of type T
/// This is great when you want to check if the last value is different from the current
/// </summary>
[System.Serializable]
public struct Memory<T>
{
    public T old;

    public T current;

    public Memory(T old, T current)
    {
        this.old = old;
        this.current = current;
    }

    /// <summary>
    /// Just sets old to current
    /// </summary>
    public void UpdateOld()
    {
        old = current;
    }

    /// <summary>
    /// Assigns current value to parameter
    /// </summary>
    /// <param name="value"></param>
    public void SetCurrent(T value)
    {
        current = value;
    }

    /// <summary>
    /// Assigns current to old
    /// </summary>
    public void RevertCurrent()
    {
        current = old;
    }
}