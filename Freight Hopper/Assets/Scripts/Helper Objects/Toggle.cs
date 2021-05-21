[System.Serializable]
public class Toggle
{
    public bool value = false;

    public void Reset() => value = false;

    public void Trigger() => value = true;
}