using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoyalCompanion : Card
{
    public LoyalCompanion() : base(0, 1, 1) { }

    [SerializeField]
    private GameObject buttleBot;

    public override void Awake()
    {
        //clockworkOwl = Resources.Load<GameObject>("Prefabs/AllFigures/PlayerSummons/ClockworkOwl");
        base.Awake();
    }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Summon(buttleBot)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Strength(-2))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Strength(4,1), "friendly",5)));
    }
}