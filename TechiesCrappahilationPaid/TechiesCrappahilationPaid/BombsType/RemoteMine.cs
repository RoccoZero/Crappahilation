﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common.Objects.UtilityObjects;
using Ensage.SDK.Geometry;
using Ensage.SDK.Helpers;
using SharpDX;
using TechiesCrappahilationPaid.BombsType.DrawBehaviour;

namespace TechiesCrappahilationPaid.BombsType
{
    public class RemoteMine : BombBase
    {
        public bool HasAghBuff;

        public RemoteMine(Unit owner) : base(owner)
        {
            Range = 425;
            RangeSystem = new CanDrawRange(Owner, Range, Color.Gray);
            Stacker = new Stacker();
            
            //TODO: delete after update
            UpdateManager.BeginInvoke(async () =>
            {
                try
                {
                    while (owner.Health <= 1)
                    {
                        await Task.Delay(100);
                    }

                    IsActive = true;
                    DisposeSpawnRange();
                    var isVisible = Owner.IsVisibleToEnemies;
                    ChangeDrawType(true,
                        isVisible ? Color.Red : Color.White);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            },500);
        }

        public Stacker Stacker;
        public RemoteMine StackerMain;

        public RemoteMine UpdateStacker(IEnumerable<RemoteMine> bombs)
        {
            var closest =
                bombs.Where(
                        x =>
                            !x.Equals(this) && x.Stacker.IsActive &&
                            x.Position.Distance2D(Position) <= 200)
                    .OrderBy(y => y.Position.Distance2D(Position))
                    .FirstOrDefault();
            if (closest != null)
            {
                closest.Stacker.Counter++;
                StackerMain = closest;
            }
            else
            {
                Stacker.Counter++;
            }

            return this;
        }

        public void UnStacker(List<RemoteMine> bombs)
        {
            var closest =
                bombs.Where(
                        x =>
                            !x.Equals(this) && x.Stacker.IsActive &&
                            x.Position.Distance2D(Position) <= 200)
                    .OrderBy(y => y.Position.Distance2D(Position))
                    .FirstOrDefault();
            if (closest != null)
            {
                closest.Stacker.Counter--;
                StackerMain = null;
            }
            else if (Stacker.IsActive)
            {
                Stacker.Counter--;
                if (Stacker.IsActive)
                {
                    closest =
                        bombs.Where(
                                x =>
                                    !x.Equals(this) && !x.Stacker.IsActive &&
                                    x.Position.Distance2D(Position) <= 200)
                            .OrderBy(y => y.Position.Distance2D(Position))
                            .FirstOrDefault();
                    if (closest != null)
                    {
                        RefreshStacker(bombs);
                    }
                }
            }
        }

        private void RefreshStacker(List<RemoteMine> bombs)
        {
            foreach (
                var bomb in
                bombs.Where(
                    manager =>
                        !manager.Equals(this) /*&&
                            manager.BombPosition.Distance2D(BombPosition) <= StackerRange*2 + 50*/))
            {
                bomb.Stacker.Counter = 0;
                bomb.InitNewStacker(bomb, bombs);
            }
        }

        public void InitNewStacker(RemoteMine target, List<RemoteMine> bombs)
        {
            var closest =
                bombs.Where(
                        x =>
                            !x.Equals(this) && x.Stacker.IsActive &&
                            x.Position.Distance2D(target.Position) <= 200)
                    .OrderBy(y => y.Position.Distance2D(target.Position))
                    .FirstOrDefault();
            if (closest != null)
            {
                closest.Stacker.Counter++;
            }
            else
            {
                Stacker.Counter++;
            }
        }

        public ParticleEffect RangeEffect { get; set; }
        public bool UnderTrueSight { get; set; }

        public void DrawSpawnRange()
        {
            RangeEffect = new ParticleEffect("materials/ensage_ui/particles/range_display_mod.vpcf", Owner.Position);
            RangeEffect.SetControlPoint(1, new Vector3(Range, 255, 0));
            RangeEffect.SetControlPoint(2, new Vector3(100, 100, 100));
        }

        public void DisposeSpawnRange()
        {
            RangeEffect?.Dispose();
        }
    }
}