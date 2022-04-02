using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace game_jam_2022_april
{
    public class Game1 : Game
    {
        public static Game1 instance;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TimeSpan lastShotTime;

        // Player
        Texture2D playerTexture;
        Vector2 playerPosition;
        float playerSpeed;

        // Enemies
        List<Enemy> enemies;

        // Projectiles
        List<Projectile> projectiles;

        // Background
        private Background _background;

        // Music
        private Song _backgroundMusic;
        private SoundEffect playerShot;
        private SoundEffect enemyDeath;
        private SoundEffect playerDeath;

        public Game1()
        {
            instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected void ResetGame()
        {
            playerSpeed = 400f;
            projectiles = new List<Projectile>();
            enemies = new List<Enemy>();

            // set enemy positions
            for (var i = 0; i < 23; i++)
            {
                enemies.Add(new Enemy(new Vector2(0 + i * 35, 0)));
            }

            // set player position
            playerPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 - playerTexture.Width / 2, _graphics.PreferredBackBufferHeight - playerTexture.Height);
        }

        protected override void Initialize()
        {
            _background = new Background(this, "projectile-01");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("player");

            ResetGame();
            
            // music
            this._backgroundMusic = Content.Load<Song>("music");
            MediaPlayer.Play(_backgroundMusic);
            MediaPlayer.IsRepeating = true;

            // sounds
            playerShot = Content.Load<SoundEffect>("shoot-02");
            enemyDeath = Content.Load<SoundEffect>("enemy-death-02");
            playerDeath = Content.Load<SoundEffect>("player-death");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // background
            _background.Update();

            // enemy movement
            foreach (var enemy in enemies.ToArray())
            {

                if (enemy.Position.Y > _graphics.PreferredBackBufferHeight)
                {
                    playerDeath.Play();
                    ResetGame();
                    return;
                }
                if (enemy.IsHit(projectiles))
                {
                    enemies.Remove(enemy);
                    enemyDeath.Play();
                    continue;
                }
                enemy.Move(gameTime.ElapsedGameTime.TotalSeconds);
            }

            // get input
            var kstate = Keyboard.GetState();

            // move player
            if (kstate.IsKeyDown(Keys.Left))
                playerPosition.X -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Right))
                playerPosition.X += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // prevent player from moving outside of screen
            playerPosition.X = MathHelper.Clamp(playerPosition.X, 0, _graphics.PreferredBackBufferWidth - playerTexture.Width);

            // spawn projectile
            var timeSinceLastShot = gameTime.TotalGameTime - lastShotTime;
            if (kstate.IsKeyDown(Keys.Space) && new TimeSpan(0, 0, 0, 0, 300) < timeSinceLastShot)
            {
                projectiles.Add(new Projectile(playerPosition));
                playerShot.Play();
                lastShotTime = gameTime.TotalGameTime;
            }

            // move projectiles
            foreach (Projectile projectile in projectiles.ToArray())
            {
                if (projectile.Position.Y < 0)
                {
                    projectiles.Remove(projectile);
                    continue;
                }
                projectile.Move(gameTime.ElapsedGameTime.TotalSeconds);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _background.Draw(_spriteBatch);

            _spriteBatch.Begin();
            _spriteBatch.Draw(playerTexture, playerPosition, Color.White);
            foreach (Projectile projectile in projectiles)
                _spriteBatch.Draw(projectile.Texture, projectile.Position, Color.White);
            foreach (Enemy enemy in enemies)
                _spriteBatch.Draw(enemy.Texture, enemy.Position, Color.White);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
