using Microsoft.Xna.Framework;

namespace Bloodbender.Enemies.Scenario3
{
    public class BadBatSpawner : GraphicObj
    {
        private float _minPeriod;
        private float _maxperiod;
        private float _currentPeriod;
        private float _timer;

        public BadBatSpawner(Vector2 position, float minPeriod, float maxPeriod)
        {
            _minPeriod = minPeriod;
            _maxperiod = maxPeriod;
            _currentPeriod = Bloodbender.ptr.rdn.Next((int)_minPeriod, (int)_maxperiod);
            _timer = 0f; 
            this.position = position;
        }

        public override bool Update(float elapsed)
        {
            if (_timer > _currentPeriod)
            {
                BadBat bat = new BadBat(position, Bloodbender.ptr.player);
                Bloodbender.ptr.listGraphicObj.Add(bat);
                _currentPeriod = Bloodbender.ptr.rdn.Next((int)_minPeriod, (int)_maxperiod);
                _timer = 0f;
            }
            _timer += elapsed;
            return base.Update(elapsed);
        }
    }
}
