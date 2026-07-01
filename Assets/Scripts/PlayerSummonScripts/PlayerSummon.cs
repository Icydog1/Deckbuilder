using UnityEngine;

public class PlayerSummon : AIFigure
{
    public override void Awake()
    {
        team = 0;
        isEnemy = false;
        base.Awake();
    }
}
