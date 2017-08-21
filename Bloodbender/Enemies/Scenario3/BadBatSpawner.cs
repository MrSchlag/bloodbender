using System;
using Microsoft.Xna.Framework;

namespace Bloodbender.Enemies.Scenario3
{
    public class BadBatSpawner : GraphicObj
    {
        private float _minPeriod;
        private float _maxperiod;
        private float _currentPeriod;
        private float _timer;
        private bool _active;
        private int _batNbr;
        private int _batSpawnedNbr;

        public BadBatSpawner(Vector2 position, float minPeriod, float maxPeriod, int batNbr)
        {
            _minPeriod = minPeriod;
            _maxperiod = maxPeriod;
            _currentPeriod = Bloodbender.ptr.rdn.Next((int)_minPeriod, (int)_maxperiod);
            _timer = 0f;
            _active = false;
            _batSpawnedNbr = 0;
            _batNbr = batNbr;
            this.position = position;
        }

        public override bool Update(float elapsed)
        {
            if (_timer > _currentPeriod)
            {
                _active = CanSeePlayer();
                if (_active && _batSpawnedNbr < _batNbr)
                {
                    Bloodbender.ptr.listGraphicObj.Add(new BadBat(position, Bloodbender.ptr.player));
                    _batSpawnedNbr++;
                }
                _currentPeriod = Bloodbender.ptr.rdn.Next((int)_minPeriod, (int)_maxperiod);
                _timer = 0f;
            }
            _timer += elapsed;
            return base.Update(elapsed);
        }

        private bool CanSeePlayer()
        {
            if (_active)
                return _active;
            bool canSee = false;
            Bloodbender.ptr.world.RayCast((fixture, point, normal, fraction) =>
            {
                if (fixture.UserData == null)
                    return -1;
                if (((AdditionalFixtureData)fixture.UserData).physicParent is Player)
                {
                    canSee = true;
                    return 0;
                }
                return 0;
            }, position * Bloodbender.pixelToMeter, Bloodbender.ptr.player.body.Position);
            return canSee;
        }
    }
}
