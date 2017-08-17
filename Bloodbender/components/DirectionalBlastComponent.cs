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
using Bloodbender.Projectiles;

namespace Bloodbender
{
    public class DirectionalBlastComponent : IComponent
    {
        /* entité détentrice du component */
        PhysicObj owner;

        Random rnd;
        private float incTimer = 0.0f;
        /* fréquence de tire des rafales */
        private float frequency;
        /* angle de tire */
        private float shootAngle; //(float)-0.349066 - (float)Math.PI / 2;
        /* définit le décalage par rapport à la position de owner */
        private Vector2 spawnPositionOffset;
        private float mainProjRadius;
        float decorationProjRadius;
        float projSpeed;

        public DirectionalBlastComponent(PhysicObj obj, float projSpeed, float shootAngle, 
            float mainProjRadius, float decorationProjRadius, Vector2 spawnPositionOffset, float frequency = 0.0f)
        {
            this.spawnPositionOffset = spawnPositionOffset;// new Vector2(50, 0);
            this.mainProjRadius = mainProjRadius;
            this.decorationProjRadius = decorationProjRadius;
            this.projSpeed = projSpeed;
            this.shootAngle = shootAngle;
            this.frequency = frequency;

            rnd = new Random();
            owner = obj;
        }

        bool IComponent.Update(float elapsed)
        {
            if (frequency != 0.0f)
            {
                incTimer += elapsed;
                if (incTimer > frequency)
                {
                    GenerateDirectionalBlast();
                    incTimer = 0.0f;
                }
            }

            return true;
        }

        void GenerateDirectionalBlast()
        {
            Vector2 mainProjPosition = spawnPositionOffset.Rotate(shootAngle) + owner.position;

            Projectile mainProj = new Blood(mainProjPosition, mainProjRadius, shootAngle, projSpeed);
            mainProj.body.IgnoreCollisionWith(owner.body);
            Bloodbender.ptr.listGraphicObj.Add(mainProj);

            int projNbr = rnd.Next(5, 10);
            
            for (int i = 0; i < projNbr; i++)
            {
                int bloodTexRand = rnd.Next(0, 3);
                int spawnPositionInLineRand = rnd.Next(-20, 21);

                Vector2 decorationProjPositionOffset = new Vector2(0 + spawnPositionOffset.X - mainProjRadius + rnd.Next(-15, 6), spawnPositionInLineRand + spawnPositionOffset.Y);
                Vector2 decorationProjPosition = decorationProjPositionOffset.Rotate(shootAngle) + owner.position;

                Projectile decorationProj = new Blood(decorationProjPosition, decorationProjRadius, shootAngle, projSpeed);
                
                if (bloodTexRand == 0)
                    decorationProj.addAnimation(new Animation(Bloodbender.ptr.blood1));
                else if (bloodTexRand == 1)
                    decorationProj.addAnimation(new Animation(Bloodbender.ptr.blood2));
                else
                    decorationProj.addAnimation(new Animation(Bloodbender.ptr.blood3));
                decorationProj.setRotation(shootAngle + (float)Math.PI / 2.0f);
                decorationProj.body.IgnoreCollisionWith(owner.body);
                Bloodbender.ptr.listGraphicObj.Add(decorationProj);
            }
        }

        public void Remove()
        {
        }
    }
}