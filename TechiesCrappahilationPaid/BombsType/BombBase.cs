﻿using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Divine;
using Divine.SDK.Extensions;
using SharpDX;
using TechiesCrappahilationPaid.BombsType.BombBehaviour;
using TechiesCrappahilationPaid.BombsType.DrawBehaviour;
using TechiesCrappahilationPaid.BombsType.Enums;

namespace TechiesCrappahilationPaid.BombsType
{
    public abstract class BombBase
    {
        public BombEnums.BombStatus BombStatus;
        public IDetonateType DetonateType;
        public bool IsActive;
        public Unit Owner;
        public float Range;
        public IRangeSystem RangeSystem;
        public Vector3 Position;

        protected BombBase(Unit owner)
        {
            Owner = owner;
            Init();
        }

        protected BombBase(Unit owner, IDetonateType detonateType)
        {
            Owner = owner;
            DetonateType = detonateType;
            Init();
        }

        private void Init()
        {
            IsActive = false;
            Position = Owner.Position;
        }

        public void StartUpdatingPosition()
        {
            UpdateManager.BeginInvoke(500, async () =>
            {
                while (Owner != null)
                {
                    try
                    {
                        Position = Owner.Position;
                        await Task.Delay(50);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            });
        }

        public float Damage { get; set; }

        public void ChangeDrawType(bool draw, Color clr)
        {
            if (draw)
                RangeSystem = new CanDrawRange(Owner, Range, clr);
            else
                RangeSystem = new CantDrawRange(Owner);
        }

        public void ChangeDrawType(IRangeSystem range)
        {
            RangeSystem = range;
        }

        public void ChangeDetonateType(bool detonate)
        {
            if (detonate)
                DetonateType = new CanDetonate(Owner.Spellbook.Spell1);
            else
                DetonateType = new CantDetonate();
        }

        public void ChangeDetonateType(IDetonateType type)
        {
            DetonateType = type;
        }

        public BombBase SetDamage(float setDamage, bool onInit = false)
        {
            Damage = setDamage;
            if (onInit)
                if (EntityManager.LocalHero.HasAghanimsScepter())
                {
                    Damage += 150;
                    ((RemoteMine) this).HasAghBuff = true;
                }

            // TechiesCrappahilationPaid.Log.Warn($"SetDamage: {setDamage} Total: {Damage}");
            return this;
        }
    }
}