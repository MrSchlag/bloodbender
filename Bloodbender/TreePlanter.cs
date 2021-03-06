﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static Bloodbender.GraphicObj;

namespace Bloodbender
{
    public class TreePlanter
    {
        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

        private Vector2 _center;

        private Random rnd;

        public TreePlanter(float minX, float maxX, float minY, float maxY, Random rand)
        {
            rnd = rand;

            _minX = minX - 5;
            _maxX = maxX + 5;
            _minY = minY - 5;
            _maxY = maxY + 5;

            _center.X = _maxX - _minX;
            _center.Y = _maxY - _minY;
            SetInitialSeedPosition(1.3f);
        }

        private void SetInitialSeedPosition(float step)
        {
            for (float y = _minY; y < _maxY; y += step)
            {
                for (float x = _minX; x < _maxX; x += step)
                {
                    AddTree(x + rnd.Next(-300000, 300000) / 100000f, y + rnd.Next(-500000, 500000) / 100000f);
                }
            }
        }

        private void AddTree(float x, float y)
        {
            if (IsPointOutside(x, y))
            {
                var tree = new GraphicObj(OffSet.BottomCenterHorizontal);
                tree.position = new Vector2(x, y) * Bloodbender.meterToPixel;
                tree.addAnimation(new Animation(Bloodbender.ptr.Content.Load<Texture2D>("tree1")));
                int treeScale = rnd.Next(600, 1800);
                tree.scale = new Vector2(treeScale / 1000.0f, treeScale / 1000.0f);
                Bloodbender.ptr.listGraphicObj.Add(tree);
            }
        }

        public static bool IsPointOutside(float x, float y)
        {
            var pt1 = new Vector2(x, y);
            int intersectionCount = 0;
            Bloodbender.ptr.world.RayCast((fixture, point, normal, fraction) =>
            {
                if (fixture.UserData == null)
                    return -1;
                if (((AdditionalFixtureData)fixture.UserData).physicParent is MapBound)
                    intersectionCount++;
                return -1;
            }, pt1, new Vector2(4000, pt1.Y));

            if (intersectionCount % 2 == 0)
                return true;
            intersectionCount = 0;
            Bloodbender.ptr.world.RayCast((fixture, point, normal, fraction) =>
            {
                if (fixture.UserData == null)
                    return -1;
                if (((AdditionalFixtureData)fixture.UserData).physicParent is MapBound)
                    intersectionCount++;
                return -1;
            }, pt1, new Vector2(pt1.X, 4000));

            if (intersectionCount % 2 == 0)
                return true;

            return false;
        }

    }
}