using System.Collections.Generic;

[System.Serializable]
public class Group<T>
{
    public string label;
    public List<T> items;
}