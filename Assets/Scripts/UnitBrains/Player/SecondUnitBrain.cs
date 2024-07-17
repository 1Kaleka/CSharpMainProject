using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            float temp = GetTemperature();

            if (temp >= overheatTemperature) return;

            for (int i = 0; i <= temp; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            IncreaseTemperature();
        }

        public override Vector2Int GetNextStep()
        {
            var target = _currTarget.Count > 0 ? _currTarget[0] : unit.Pos;
            return IsTargetInRange(target) ? unit.Pos : unit.Pos.CalcNextStepTowards(target); 
        }
        private readonly List<Vector2Int> _currTarget = new();
        

        protected override List<Vector2Int> SelectTargets()
        {
            var result = new List<Vector2Int>();
            var minDistance = float.MaxValue;
            var bestTarget = Vector2Int.zero;
            foreach (var target in GetAllTargets())
            {
                var distance = DistanceToOwnBase(target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestTarget = target;
                }
            }

            _currTarget.Clear();
            if (minDistance < float.MaxValue)
            {
                _currTarget.Add(bestTarget);    
                if (IsTargetInRange(bestTarget))
                {
                    result.Add(bestTarget);
                }
            }
            else
            {
                _currTarget.Add(runtimeModel.RoMap.Bases [
                    IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId]);
            }            
            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}