using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender.Scene
{
    public class TotemScene : IScene
    {

        private bool _isOn = false;
        private Random _rnd;
        public float SpawnInterval { get; set; }
        private float _intervalTimer;

        public List<Vector2> TotemPositionList { get; private set; }
        public List<Vector2> EnemySpawnList { get; private set; }

        public TotemScene()
        {
            SpawnInterval = 2;
            _intervalTimer = 0;
            _isOn = false;
            _rnd = new Random();
            TotemPositionList = new List<Vector2>();
            EnemySpawnList = new List<Vector2>();
        }

        public void Run()
        {
            foreach (var pos in TotemPositionList)
            {
                Totem totem = new Totem(pos);
                Bloodbender.ptr.listGraphicObj.Add(totem);
                Texture2D tex = Bloodbender.ptr.Content.Load<Texture2D>("Totem");
                totem.addAnimation(new Animation(tex));
            }

            _isOn = true;
        }

        public void Stop()
        {
            _isOn = false;
        }

        public void AddTotemPosition(Vector2 position)
        {
            TotemPositionList.Add(position);
        }

        public void AddEnemySpawnPosition(Vector2 position)
        {
            EnemySpawnList.Add(position);
        }

        public void Update(float elapsed)
        {
            _intervalTimer += elapsed;
            if (_isOn == false || _intervalTimer < SpawnInterval || !EnemySpawnList.Any())
                return;
            _intervalTimer = 0;

            Enemy enemy = new Enemy(EnemySpawnList[_rnd.Next(0, EnemySpawnList.Count - 1)], Bloodbender.ptr.player);
            Bloodbender.ptr.listGraphicObj.Add(enemy);
        }
    }
}
