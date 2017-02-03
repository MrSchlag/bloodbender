using System;
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


namespace Bloodbender
{
    public class DirectionalBlastComponent : IComponent
    {
        PhysicObj owner;

        Random rnd;
        float incTimer = 0.0f;
        float frequency = 2f;
        float shootAngle = (float)Math.PI;

        public DirectionalBlastComponent(PhysicObj obj)
        {
            rnd = new Random();
            owner = obj;
        }

        void IComponent.Update(float elapsed)
        {
            incTimer += elapsed;
            if (incTimer > frequency)
            {
                GenerateDirectionalBlast();
                incTimer = 0.0f;
            }
        }

        void GenerateDirectionalBlast()
        {
            Projectile mainProj = new Projectile(owner.position, 16f, shootAngle, 100f);
            mainProj.addAnimation(new Animation(Bloodbender.ptr.bouleRouge));
            Bloodbender.ptr.listGraphicObj.Add(mainProj);

            int projNbr = rnd.Next(5, 10);
            for (int i = 0; i < projNbr; i++)
            {
                Projectile decorationProj = new Projectile(new Vector2(owner.position.X + rnd.Next(-16, 17), owner.position.Y + rnd.Next(-16, 17)), 3f, (float)Math.PI, 95f);
                int bloodRand = rnd.Next(0, 3);
                if (bloodRand == 0)
                    decorationProj.addAnimation(new Animation(Bloodbender.ptr.blood1));
                else if (bloodRand == 1)
                    decorationProj.addAnimation(new Animation(Bloodbender.ptr.blood2));
                else
                    decorationProj.addAnimation(new Animation(Bloodbender.ptr.blood3));
                decorationProj.setRotation(shootAngle + (float)Math.PI / 2.0f);
                Bloodbender.ptr.listGraphicObj.Add(decorationProj);
            }
        }
    }
}