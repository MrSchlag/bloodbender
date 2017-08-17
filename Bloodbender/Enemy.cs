﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Bloodbender.Projectiles;

namespace Bloodbender
{
    public class Enemy : PhysicObj
    {
        protected Fixture fixture;
        public PhysicObj target;
        protected float distanceAttackWithTarget = 40;
        protected float attackRate = 1.5f;
        protected float timerAttack = 0;
        protected bool canGenerateProjectile = true;
        protected bool canBeHitByPlayer = true;
        protected bool canBeHitByProjectile = true;
        protected bool canAttack = true;
        protected float lifePoints = 3;

        public Enemy(Vector2 position, PhysicObj player) : base(position, PathFinderNodeType.CENTER)
        {
            target = player;
        }

        public override bool Update(float elapsed)
        {
            if (canAttack)
            {
                if (timerAttack > 0)
                    timerAttack -= elapsed;

                if (distanceWith(target.position) < distanceAttackWithTarget)
                {
                    if (timerAttack <= 0)
                    {
                        Attack();
                    }
                    else
                    {
                        if (!getAnimation(1).isRunning)
                            runAnimation(0);
                    }
                }
                else
                    runAnimation(0);
            } else
                runAnimation(0);


            if (lifePoints <= 0)
                shouldDie = true;

            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        protected virtual void Attack()
        {
            runAnimation(1);
            timerAttack = attackRate;
        }
        public void takeHit(float angle)
        {
            if (canBeHitByPlayer)
                lifePoints -= 1;
            if (canGenerateProjectile)
            {
                //System.Diagnostics.Debug.WriteLine("Totem touched by playerattacksensor");
                Projectile proj = new Blood(body.Position * Bloodbender.meterToPixel, 10, angle, 400f);
                body.FixtureList[0].IgnoreCollisionWith(proj.body.FixtureList[0]);
                Bloodbender.ptr.listGraphicObj.Add(proj);
            }
        }
    }
}
