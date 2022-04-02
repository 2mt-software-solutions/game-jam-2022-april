using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace game_jam_2022_april
{
    class Enemy
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed;

        // constructor
        public Enemy(Vector2 position)
        {
            this.position = position;
            texture = Game1.instance.Content.Load<Texture2D>("enemy-01");
            speed = 50f;
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return texture;
            }
        }

        public Rectangle Rect
        {
            get
            {
                var rectangle = texture.Bounds;
                rectangle.Offset(position);
                return rectangle;
            }
        }

        public void Move(double elapsedGameTime)
        {
            position.Y += speed * (float)elapsedGameTime;
        }

        public bool IsHit(List<Projectile> projectiles)
        {
            foreach (Projectile projectile in projectiles)
            {
                if (projectile.Rect.Intersects(Rect))
                {
                    return true;
                }
            }
            return false;
               
        }
    }
}
