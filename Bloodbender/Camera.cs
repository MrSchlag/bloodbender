using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloodbender
{
    public class Camera // la camera est un objet dont la position doit absolument etre defini apres que la position de l'objet qu'elle suit soit defini
    {
        public int width = 1280; //determine la largeur et hauteur de la vue
        public int height = 720;

        GraphicObj gameobj = null; // obj que la camera peut suivre
        Vector2 offset = Vector2.Zero;

        public Vector2 zoom; // Camera Zoom
        public Matrix transform; // Matrix Transform
        public Vector2 position; // Camera Position
        protected float rotation; // Camera Rotation

        public Camera()
        {
            zoom = Vector2.One;
            rotation = 0.0f;
            position = Vector2.Zero;
        }

        public Vector2 Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public void Move(Vector2 amount)
        {
            position += amount;
        }
        public Vector2 Pos
        {
            get { return position; }
            set { position = value; }
        }

        public void requestFocus(GraphicObj gameobj) // permet a la camera de suivre l'objet passer en parametre
        { requestFocus(gameobj, Vector2.Zero); }

        public void requestFocus(GraphicObj gameobj, Vector2 offset)
        {
            this.gameobj = gameobj;
            this.offset = offset;
        }

        public bool isInView(GraphicObj obj) // à optimiser et faux si la taille de la camera change au runtime
        {
            Rectangle box1 = new Rectangle((int)Math.Round(position.X - width / 2), (int)Math.Round(position.Y - height / 2), width, height);
            Rectangle box2 = new Rectangle((int)Math.Round(obj.position.X), (int)Math.Round(obj.position.Y), (int)Math.Round(obj.getSize().X), (int)Math.Round(obj.getSize().Y));
            if ((box2.X > box1.X + box1.Width)
                || (box2.X + box2.Width < box1.X)
                || (box2.Y > box1.Y + box1.Height)
                || (box2.Y + box2.Height < box1.Y))
                return false;
            else
                return true;
        }

        public bool Update()
        {
            if (gameobj != null)
                position = gameobj.getCenter() + offset;

            return true;
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(zoom.X, zoom.Y, 1)) *
                 Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return transform;
        }
    }
}
