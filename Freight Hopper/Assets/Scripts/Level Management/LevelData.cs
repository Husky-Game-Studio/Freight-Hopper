using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Level/Data"), System.Serializable]
public class LevelData : ScriptableObject
{
    public enum NextLevelStatus
    {
        NextLevel,
        NextWorld,
        Menu,
        Credits,
        Custom
    }

    [SerializeField] private NextLevelStatus nextLevelStatus = NextLevelStatus.NextLevel;
    [SerializeField] private string customNextLevelName;
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;
    [SerializeField] private float rotationAngle = 0;
    [SerializeField] private Quaternion velocityDirection;
    [SerializeField] private float speed;

    [Header("Meta Data")]
    [SerializeField] private string title;
    [SerializeField] private Texture2D image;
    [SerializeField] private Optional<string> tutorialSceneName;
    [SerializeField] private float[] medalTimes = new float[4];

    [SerializeField]
    private PlayerAbilities.Name[] defaultAbilities = {
            PlayerAbilities.Name.MovementBehavior,
            PlayerAbilities.Name.JumpBehavior,
            PlayerAbilities.Name.WallRunBehavior,
            PlayerAbilities.Name.GroundPoundBehavior
        };
    [SerializeField]
    private List<PlayerAbilities.Name> activeAbilities = new List<PlayerAbilities.Name>{
            PlayerAbilities.Name.MovementBehavior,
            PlayerAbilities.Name.JumpBehavior,
            PlayerAbilities.Name.WallRunBehavior,
            PlayerAbilities.Name.GroundPoundBehavior,
        };

    public NextLevelStatus NLevelStatus => nextLevelStatus;
    public string CustomNextLevelName => customNextLevelName;
    public Vector3 SpawnPosition => spawnPosition;
    public float RotationAngle => rotationAngle;
    public Quaternion VelocityDirection => velocityDirection;
    public float Speed => speed;
    public string Title => title;
    public Texture2D Image => image;
    public Optional<string> TutorialSceneName => tutorialSceneName;

    
    public IList<float> MedalTimes => Array.AsReadOnly(medalTimes);

    public IList<PlayerAbilities.Name> ActiveAbilities => activeAbilities.AsReadOnly();
    public IList<PlayerAbilities.Name> DefaultAbilites => Array.AsReadOnly(defaultAbilities);

    public void RestartActiveAbilities()
    {
        activeAbilities.Clear();
        for (int i = 0; i < defaultAbilities.Length; i++)
        {
            activeAbilities.Add(defaultAbilities[i]);
        }
    }

    public void AddActiveAbility(PlayerAbilities.Name ability)
    {
        if (activeAbilities.Contains(ability))
        {
            return;
        }
        activeAbilities.Add(ability);
    }

    public void UpdateToLastLevelsAbilities(IList<PlayerAbilities.Name> oldDefaultAbilities, IList<PlayerAbilities.Name> oldActiveAbilities)
    {
        if (EqualAbilitiesList(oldDefaultAbilities, oldActiveAbilities))
        {
            PlayerAbilities.Name[] defaultAbilitiesTemp = new PlayerAbilities.Name[this.DefaultAbilites.Count];
            this.DefaultAbilites.CopyTo(defaultAbilitiesTemp, 0);
            activeAbilities = new List<PlayerAbilities.Name>(defaultAbilitiesTemp);
            return;
        }
        if(EqualAbilitiesList(oldActiveAbilities, this.ActiveAbilities))
        {
            return;
        }

        List<PlayerAbilities.Name> posDif = PosDif(oldDefaultAbilities, defaultAbilities);
        PlayerAbilities.Name[] lastLevelsAbilities = new PlayerAbilities.Name[oldActiveAbilities.Count];
        oldActiveAbilities.CopyTo(lastLevelsAbilities, 0);
        activeAbilities = new List<PlayerAbilities.Name>(lastLevelsAbilities);
        for(int i = 0; i < posDif.Count; i++)
        {
            activeAbilities.Add(posDif[i]);
        }
    }
    
    public bool EqualAbilitiesList(IList<PlayerAbilities.Name> a, IList<PlayerAbilities.Name> b)
    {
        bool equal = true;
        if (a.Count != b.Count)
        {
            equal = false;
        }
        else
        {
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i])
                {
                    equal = false;
                    break;
                }
            }
        }
        return equal;
    }

    // Difference between a and b, but only positive differences (e.g. new ability is added when switching to b)
    public List<PlayerAbilities.Name> PosDif(IList<PlayerAbilities.Name> a, IList<PlayerAbilities.Name> b)
    {
        List<PlayerAbilities.Name> result = new List<PlayerAbilities.Name>();
        for(int i= 0; i < b.Count; i++)
        {
            result.Add(b[i]);
        }
        for(int i = 0; i < a.Count; i++)
        {
            if (result.Contains(a[i]))
            {
                result.Remove(a[i]);
            }
        }

        return result;
    }

    // Difference between a and b, but only negative differences (e.g. ability is removed when switching to b)
    public List<PlayerAbilities.Name> NegDif(IList<PlayerAbilities.Name> a, IList<PlayerAbilities.Name> b)
    {
        return PosDif(b, a);
    }

    // List of common abilities
    public List<PlayerAbilities.Name> SameDif(IList<PlayerAbilities.Name> a, IList<PlayerAbilities.Name> b)
    {
        List<PlayerAbilities.Name> result = new List<PlayerAbilities.Name>();
        for(int i = 0; i < a.Count; i++)
        {
            if (b.Contains(a[i]))
            {
                result.Add(a[i]);
            }
        }

        return result;
    }

    public  bool UsingDefaultAbilities()
    {
        return EqualAbilitiesList(this.ActiveAbilities, this.DefaultAbilites);
    }
    public void RemoveActiveAbility(PlayerAbilities.Name ability)
    {
        if (!activeAbilities.Contains(ability))
        {
            return;
        }
        activeAbilities.Remove(ability);
    }

    public bool ContainsAbility(PlayerAbilities.Name ability)
    {
        return activeAbilities.Contains(ability);
    }

    public void SetSpawnTransform(Transform transform)
    {
        spawnPosition = transform.position;
        rotationAngle = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up);
        velocityDirection = Quaternion.LookRotation(transform.forward, transform.up);
    }
}