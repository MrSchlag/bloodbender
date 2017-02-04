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
        /* entité détentrice du component */
        PhysicObj owner;

        Random rnd;
        float incTimer = 0.0f;
        /* fréquence de tire des rafales */
        float frequency = 2f;
        /* angle de tire */
        float shootAngle = (float)-0.349066 - (float)Math.PI / 2;
        /* définit le décalage par rapport à la position de owner */
        Vector2 spawnPositionOffset;
        float mainProjRadius;
        float decorationProjRadius;
        float projSpeed;

        public DirectionalBlastComponent(PhysicObj obj)
        {
            spawnPositionOffset = new Vector2(50, 0);
            mainProjRadius = 16f;
            decorationProjRadius = 3f;
            projSpeed = 300f;
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
            Vector2 mainProjPosition = spawnPositionOffset.Rotate(shootAngle) + owner.position;

            Projectile mainProj = new Projectile(mainProjPosition, mainProjRadius, shootAngle, projSpeed);
            mainProj.addAnimation(new Animation(Bloodbender.ptr.bouleRouge));
            mainProj.body.IgnoreCollisionWith(owner.body);
            Bloodbender.ptr.listGraphicObj.Add(mainProj);

            int projNbr = rnd.Next(5, 10);
            
            for (int i = 0; i < projNbr; i++)
            {
                int bloodTexRand = rnd.Next(0, 3);
                int spawnPositionInLineRand = rnd.Next(-20, 21);

                Vector2 decorationProjPositionOffset = new Vector2(0 + spawnPositionOffset.X - mainProjRadius + rnd.Next(-15, 6), spawnPositionInLineRand + spawnPositionOffset.Y);
                Vector2 decorationProjPosition = decorationProjPositionOffset.Rotate(shootAngle) + owner.position;

                Projectile decorationProj = new Projectile(decorationProjPosition, decorationProjRadius, shootAngle, projSpeed);
                
                if (bloodTexRand == 0)
                    decorationProj.addAnimation(new Animation(Bloodbender.ptr.blood1));
                else if (bloodTexRand == 1)
                    decorationProj.addAnimation(new Animation(Bloodbender.ptr.blood2));
                else
                    decorationProj.addAnimation(new Animation(Bloodbender.ptr.blood3));
                decorationProj.setRotation(shootAngle + (float)Math.PI / 2.0f);
                decorationProj.body.IgnoreCollisionWith(owner.body);
                decorationProj.addComponent(new TextureHeadingToDirectionComponent(decorationProj));
                Bloodbender.ptr.listGraphicObj.Add(decorationProj);
            }
        }
    }
}