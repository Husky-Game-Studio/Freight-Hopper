using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelTimeSaveData
{
    // This is all temp really, this isn't really that fast as we want
    [SerializeField] private List<AbilityTimes> bestTimes;
    [SerializeField] private int medalIndex = -1;
    public int MedalIndex => medalIndex;

    public void SetNewMedalIndex(int value)
    {
        medalIndex = value;
    }

    public LevelTimeSaveData(List<AbilityTimes> bestTimes)
    {
        this.bestTimes = bestTimes;
    }

    [System.Serializable]
    public struct AbilityTimes
    {
        [SerializeField] public List<PlayerAbilities.Name> abilitiesUsed;
        [SerializeField] public float time;

        public AbilityTimes(List<PlayerAbilities.Name> names, float time)
        {
            abilitiesUsed = names;
            this.time = time;
        }

        public AbilityTimes(PlayerAbilities.Name[] names, float time)
        {
            abilitiesUsed = new List<PlayerAbilities.Name>(names);
            this.time = time;
        }
    }

    // First returns best time, next returns index to get the best time
    public (float, int) GetTime(PlayerAbilities.Name[] abilitesUsed)
    {
        for (int i = 0; i < bestTimes.Count; i++)
        {
            if (bestTimes[i].abilitiesUsed.Count != abilitesUsed.Length)
            {
                continue;
            }
            bool equals = true;
            for (int j = 0; j < abilitesUsed.Length; j++)
            {
                if (bestTimes[i].abilitiesUsed[j] != abilitesUsed[j])
                {
                    equals = false;
                    break;
                }
            }
            if (equals)
            {
                return (bestTimes[i].time, i);
            }
        }
        return (float.PositiveInfinity, -1);
    }

    // returns the best time too, checks if time provided is the new best time first
    public float SetNewBestTime(PlayerAbilities.Name[] abilitesUsed, float time)
    {
        (float, int) currentBest = GetTime(abilitesUsed);
        if (currentBest.Item1 < time)
        {
            return currentBest.Item1;
        }
        if (currentBest.Item2 == -1)
        {
            bestTimes.Add(new AbilityTimes(abilitesUsed, time));
        }
        else
        {
            bestTimes[currentBest.Item2] = new AbilityTimes(abilitesUsed, time);
        }

        return time;
    }
}