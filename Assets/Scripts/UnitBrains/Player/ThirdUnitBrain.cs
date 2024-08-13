using System.Collections;
using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnitBrains.Player;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    public override string TargetUnitName => "Ironclad Behemoth";

    private const float _timeMax = 1f;
    private bool Attack = false;
    private bool Move = false;
    private bool hasTargets = false;
    private float _startTime = 0f;

    public override void Update(float deltaTime, float time)
    {
        base.Update(deltaTime, time);

        if (Attack && !hasTargets && !Move || !Attack && hasTargets && !Move)
        {
            Move = true;
            _startTime = time;
        }

        if (Move && time - _startTime >= _timeMax)
        {
            Attack = !Attack;
            Move = false;
        }
    }    
}

