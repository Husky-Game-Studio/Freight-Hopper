/// <summary>
/// Inspired from autosplitting language, just holds the previous value and the current value of type T
/// This is great when you want to check if the last value is different from the current
/// </summary>
[System.Serializable]
public struct Var<T>
{
    public T old;

    public T current;

    public Var(T old, T current)
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
}