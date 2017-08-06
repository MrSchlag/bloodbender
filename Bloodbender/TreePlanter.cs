using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class TreePlanter
    {
        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

        private Vector2 _center;

        private List<TreeSeed> seeds;

        public TreePlanter(float minX, float maxX, float minY, float maxY)
        {
            seeds = new List<TreeSeed>();

            _minX = minX - 5;
            _maxX = maxX + 5;
            _minY = minY - 5;
            _maxY = maxY + 5;

            _center.X = _maxX - _minX;
            _center.Y = _maxY - _minY;
            SetInitialSeedPosition(0.4f);
            Run();
        }

        private void SetInitialSeedPosition(float step)
        {
            for (float y = _minY; y < _maxY; y += step)
            {
                var seed = new TreeSeed(new Vector2(_minX, y), new Vector2(1, 0), 0);
                seeds.Add(seed);
                seed = new TreeSeed(new Vector2(_maxX, y), new Vector2(-1, 0), 0);
                seeds.Add(seed);
            }
            
            for (float x = _minX + step; x < _maxX - step; x += step)
            {
                var seed = new TreeSeed(new Vector2(x, _minY), new Vector2(0, 1), 0);
                seeds.Add(seed);
                seed = new TreeSeed(new Vector2(x, _maxY), new Vector2(0, -1), 50);
                seeds.Add(seed);
            }

        }

        private void Run()
        {
            foreach (var seed in seeds)
            {
                seed.Run();
            }
        }
    }
}