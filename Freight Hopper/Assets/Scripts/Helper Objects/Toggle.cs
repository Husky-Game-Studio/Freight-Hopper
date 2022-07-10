[System.Serializable]
public class Toggle
{
    public bool value;

    public void Reset() => value = false;

    public void Trigger() => value = true;
}