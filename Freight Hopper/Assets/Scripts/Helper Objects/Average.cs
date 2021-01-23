using System.Collections.Generic;

/// <summary>
/// Uses a queue and a duration of time to determine the average of a value T
/// </summary>
public class Average
{
    private Queue<float> values;
    private float total;
    private float defaultValue;
    private int resettedBlockCount;
    private int n;

    /// <summary>
    /// Creates average object
    /// </summary>
    /// <param name="n">Duration average uses for calculation (e.g. 20 frames)</param>
    /// <param name="intializeValue">Fills queue with this value n times</param>
    public Average(int n, float intializeValue = 0)
    {
        this.n = n;
        defaultValue = intializeValue;
        values = new Queue<float>(n);
        Fill();
    }

    // Resets block count to N and causes total to not use the queue for N updates
    private void Fill()
    {
        total = defaultValue * n;
        resettedBlockCount = n;
    }

    /// <summary>
    /// dequeues last value in average queue and adds in value
    /// </summary>
    /// <param name="value">Adds value to the total</param>
    public void Update(float value)
    {
        if (resettedBlockCount > 0)
        {
            total -= defaultValue;
            resettedBlockCount--;
            if (values.Count == n)
            {
                values.Dequeue();
            }
        }
        else
        {
            total -= values.Dequeue();
        }

        values.Enqueue(value);
        total += value;
    }

    /// <summary>
    /// Resets average as if it were just initialized again
    /// </summary>
    public void Reset()
    {
        Fill();
    }

    /// <summary>
    /// Entire point of class
    /// </summary>
    /// <returns>returns average over n time steps</returns>
    public float GetAverage()
    {
        return total / n;
    }
}