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
    public class Totem : PhysicObj
    {
        public Totem(Vector2 position) : base(position, PathFinderNodeType.OUTLINE)
        {
            Bloodbender.ptr.shadowsRendering.addShadow(new Shadow(this));

            //Fixture totemBoundsFix = createRectangleFixture(100, 100, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            Fixture totemBoundsFix = createOctogoneFixture(32, 32, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            addFixtureToCheckedCollision(totemBoundsFix);
            body.BodyType = BodyType.Static;
            //height = 10;
            IComponent comp = new GenerateProjectileComponent(this);
            //addComponent(comp);
            length = 10;
            createOutlinePathNodes();

            addAnimation(new Animation(Bloodbender.ptr.Content.Load<Texture2D>("Totem")));
        }

        public override bool Update(float elapsed)
        {
            return base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        
        public void generateProjectile(float angle)
        {
            //System.Diagnostics.Debug.WriteLine("Totem touched by playerattacksensor");
            Projectile proj = new Blood(body.Position * Bloodbender.meterToPixel, 10, angle, 400f);
            body.FixtureList[0].IgnoreCollisionWith(proj.body.FixtureList[0]);
            Bloodbender.ptr.listGraphicObj.Add(proj);
        }
    }
}
