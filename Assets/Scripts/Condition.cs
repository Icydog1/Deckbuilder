using UnityEngine;

public class Condition : MonoBehaviour
{
    private string conditionName;
    public string Name { get { return conditionName; } }
    private int amount;
    public int Value { get { return amount; } set { amount = value; } }

    private int duration;
    public int Duration { get { return duration; } set { duration = value; } }

    protected int addType;
    public int AddType { get { return addType; }}
    protected bool isVisible;
    public bool IsVisible { get { return isVisible; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Condition (string name, int conditionValue, int conditionDuration, int conditionAddType, bool isShown)
    {
        conditionName = name;
        amount = conditionValue;
        duration = conditionDuration;
        addType = conditionAddType;
        isVisible = isShown;
    }
    
}

public class Strength : Condition
{
    public Strength(int conditionValue, int conditionDuration = -1, int addType = 1) : base("strength", conditionValue, conditionDuration, addType, true) {}
}

public class Dexterity : Condition
{
    public Dexterity(int conditionValue, int conditionDuration = -1, int addType = 1) : base("dexterity", conditionValue, conditionDuration, addType, true) {}
}

public class Speed : Condition
{
    public Speed(int conditionValue, int conditionDuration = -1, int addType = 1) : base("speed", conditionValue, conditionDuration, addType, true) { }
}

public class  Finesse: Condition
{
    public Finesse(int conditionValue, int conditionDuration = -1, int addType = 1) : base("finesse", conditionValue, conditionDuration, addType, true) { }
}

public class NaturalScaling: Condition
{
    public NaturalScaling(int conditionValue, int conditionDuration = -1, int addType = 3) : base("naturalScaling", conditionValue, conditionDuration, addType, false) { }
}