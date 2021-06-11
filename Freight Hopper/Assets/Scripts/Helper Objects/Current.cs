/// <summary>
/// Think of the value as "health" and the stored value as "max health"
/// Overall, this is used if you have a value you want to reset to
/// </summary>
[System.Serializable]
public struct Current<T>
{
    private T stored;
    public T Stored => stored;

    public T value;

    public Current(T stored)
    {
        this.stored = stored;
        this.value = stored;
    }

    public Current(T stored, T value)
    {
        this.stored = stored;
        this.value = value;
    }

    /// <summary>
    /// Assigns value to the stored value
    /// </summary>
    public void Reset()
    {
        value = stored;
    }
}