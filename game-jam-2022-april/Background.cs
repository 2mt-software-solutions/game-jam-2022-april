using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace game_jam_2022_april
{
    class BackgroundElement
    {
        private Vector2 position;
        private Texture2D texture;
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        public BackgroundElement(Game game, Vector2 position, string textureName = "projectile-01")
        {
            this.texture = game.Content.Load<Texture2D>(textureName);
            this.position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public void Move(Vector2 direction)
        {
            this.position += direction;
        }

        public void MoveTo(Vector2 position)
        {
            this.position = position;
        }
    }

    class Background
    {
        protected Game game;
        protected const float speed = 0.5f;
        protected List<BackgroundElement> backgroundElements;
        protected Random _rng = new Random();
        protected const int maxBackgroundElements = 250;
        protected int screenWidth;
        protected int screenHeight;

        public Background(Game game, string texture)
        {
            this.game = game;
            screenWidth = game.GraphicsDevice.Viewport.Width;
            screenHeight = game.GraphicsDevice.Viewport.Height;
            this.backgroundElements = new List<BackgroundElement>();
        }

        public void Update()
        {
            int screenWidth = game.GraphicsDevice.Viewport.Width;
            int screenHeight = game.GraphicsDevice.Viewport.Height;

            // fill background until maxElements reached
            if (backgroundElements.Count <= maxBackgroundElements)
            {
                backgroundElements.Add(
                    new BackgroundElement(
                        game,
                        new Vector2(
                            _rng.Next(0, screenWidth),
                            _rng.Next(0, screenHeight)
                        )
                    )
                );
            }

            // when element outside screen, "respawn"/move to top again
            foreach (BackgroundElement element in backgroundElements)
            {
                if (element.Position.Y >= screenHeight)
                {
                    element.MoveTo(
                        new Vector2(
                            _rng.Next(0, screenWidth),
                            0
                        )
                    );
                    continue;
                }
                element.Move(new Vector2(0, speed));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (BackgroundElement element in backgroundElements.ToArray())
            {
                element.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}