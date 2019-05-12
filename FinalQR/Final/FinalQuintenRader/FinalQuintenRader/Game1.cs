 using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace FinalQuintenRader
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont textfont;
        Vector2 textPos1;
        Vector2 textPos2;
        Vector2 textPos3;

        Model player1;
        Model squProj;

        Vector3 player1Pos;
        float player1Rot;
        Vector3 player1Dir;
        int player1Score;

        Model player2;
        Model sphProj;

        Vector3 player2Pos;
        float player2Rot;
        Vector3 player2Dir;
        int player2Score;

        Model wall;
        Model turntwall;

        public float StartTime { get; set; }   //start time in milliseconds
        public float MaxTime { get; set; }
        private float elapsedTime = 0;

        Model plane;

        List<Projectile> squProjectiles;
        List<Projectile> sphProjectiles;

        GamePadState state = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.None);

        public Game1()
        {
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            player1Rot = 0;
            player1Pos = Vector3.Zero;
            player1Score = 0;
            textPos1 = new Vector2(25, 25);

            player2Rot = 0;
            player2Pos = new Vector3(0f, 0f, 0f);
            player2Score = 0;
            textPos2 = new Vector2(700, 25);

            textPos3 = new Vector2(400, 25);

            squProjectiles = new List<Projectile>();
            sphProjectiles = new List<Projectile>();

            MaxTime = 1200000;
            StartTime = 0;
            elapsedTime = 0;

            base.Initialize();
        }
        private void reset(GameTime gameTime)
        {
            player1Rot = 0;
            player1Pos = Vector3.Zero;
            player1Score = 0;

            player2Rot = 0;
            player2Pos = new Vector3(0f, 0f, 0f);
            player2Score = 0;

            squProjectiles = new List<Projectile>();
            sphProjectiles = new List<Projectile>();

            MaxTime = 1200000;
            StartTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            elapsedTime = 0;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            textfont = Content.Load <SpriteFont>("textfont");

            player1 = Content.Load<Model>("Cube");
            squProj = Content.Load<Model>("Cube");
            player2 = Content.Load<Model>("Sphere");
            sphProj = Content.Load<Model>("Sphere");
            textfont = Content.Load<SpriteFont>("textFont");

            plane = Content.Load<Model>("Plane");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            //Player one controls
            if (elapsedTime / 1000 < 120)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    player1Rot -= 0.05f;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    player1Rot += 0.05f;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    player1Pos += Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(player1Rot));
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    player1Pos += Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(player1Rot));
                }
                //player one shooting
                if (Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightTrigger))
                {
                    player1Dir = Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(player1Rot));
                    squProjectiles.Add(new Projectile(player1Pos, player1Dir));
                    GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                    //player1Score++;
                }
                //player one controller movement
                var gamePadState1 = GamePad.GetState(PlayerIndex.One);
                player1Pos.X -= gamePadState1.ThumbSticks.Left.X;
                player1Pos.Z += gamePadState1.ThumbSticks.Left.Y;
                player1Rot = (float)Math.Acos(Vector2.Dot(new Vector2(0, 1), gamePadState1.ThumbSticks.Right)) * -((gamePadState1.ThumbSticks.Right.X == 0) ? -1 : (gamePadState1.ThumbSticks.Right.X / Math.Abs(gamePadState1.ThumbSticks.Right.X)));

                //player 1 projectile collision w/ player 2 detection
                for (int p = 0; p < squProjectiles.Count; p++)
                {
                    squProjectiles[p].Update(gameTime);
                    if (Vector3.Distance(player2Pos, squProjectiles[p].Pos) < 1.0f)
                    {
                        squProjectiles.RemoveAt(p);
                        p--;
                        player1Score++;
                        break;
                    }
                }

                //player 1 end of screen looping
                if (player1Pos.X >= 59)
                    player1Pos = new Vector3(-58, player1Pos.Y, player1Pos.Z);
                if (player1Pos.X <= -59)
                    player1Pos = new Vector3(58, player1Pos.Y, player1Pos.Z);
                if (player1Pos.Z >= 59)
                    player1Pos = new Vector3(player1Pos.X, player1Pos.Y, -58);
                if (player1Pos.Z <= -59)
                    player1Pos = new Vector3(player1Pos.X, player1Pos.Y, 58);

                //player 2 controls
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    player2Rot -= 0.05f;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    player2Rot += 0.05f;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    player2Pos += Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(player2Rot));
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    player2Pos += Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(player2Rot));
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.RightTrigger))
                {
                    player2Dir = Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(player2Rot));
                    sphProjectiles.Add(new Projectile(player2Pos, player2Dir));
                    GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                    //player2Score++;
                }
                //player two controller movement
                var gamePadState2 = GamePad.GetState(PlayerIndex.Two);
                player2Pos.X -= gamePadState2.ThumbSticks.Left.X;
                player2Pos.Z += gamePadState2.ThumbSticks.Left.Y;
                player2Rot = (float)Math.Acos(Vector2.Dot(new Vector2(0, 1), gamePadState2.ThumbSticks.Right)) * -((gamePadState2.ThumbSticks.Right.X == 0) ? -1 : (gamePadState2.ThumbSticks.Right.X / Math.Abs(gamePadState2.ThumbSticks.Right.X)));

                //player 2 projectile collision w/ player 1 detection
                for (int p = 0; p < sphProjectiles.Count; p++)
                {
                    sphProjectiles[p].Update(gameTime);
                    if (Vector3.Distance(player1Pos, sphProjectiles[p].Pos) < 1.0f)
                    {
                        sphProjectiles.RemoveAt(p);
                        p--;
                        player2Score++;
                        break;
                    }
                }

                //player 2 projectile collision w/ enemy detection
                if (player2Pos.X >= 59)
                    player2Pos = new Vector3(-58, player2Pos.Y, player2Pos.Z);
                if (player2Pos.X <= -59)
                    player2Pos = new Vector3(58, player2Pos.Y, player2Pos.Z);
                if (player2Pos.Z >= 59)
                    player2Pos = new Vector3(player2Pos.X, player2Pos.Y, -58);
                if (player2Pos.Z <= -59)
                    player2Pos = new Vector3(player2Pos.X, player2Pos.Y, 58);

                //deletes projectiles after they go off screen
                for (int p = 0; p < sphProjectiles.Count; p++)
                {
                    if (Vector3.Distance(new Vector3(0, 0, 0), sphProjectiles[p].Pos) > 60.0f)
                    {
                        sphProjectiles.RemoveAt(p);
                        p--;
                        break;
                    }
                }
                for (int p = 0; p < squProjectiles.Count; p++)
                {
                    if (Vector3.Distance(new Vector3(0, 0, 0), squProjectiles[p].Pos) > 60.0f)
                    {
                        squProjectiles.RemoveAt(p);
                        p--;
                        break;
                    }
                }
                elapsedTime = (float)gameTime.TotalGameTime.TotalMilliseconds - StartTime;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    reset(gameTime);
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            //player1 world
            Matrix proj = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(60),
                1,
                0.001f,
                1000);
            Matrix view = Matrix.CreateLookAt(
                new Vector3(0,100,0),
                Vector3.Zero,
                new Vector3(0,0,1));
            //player1 world
            Matrix world1 = Matrix.CreateScale(2f)
                * Matrix.CreateRotationY(player1Rot)
                * Matrix.CreateTranslation(player1Pos);
            //player2 world
            Matrix world2 = Matrix.CreateScale(1f)
                * Matrix.CreateRotationY(player2Rot)
                * Matrix.CreateTranslation(player2Pos);
            //world
            Matrix world = Matrix.CreateScale(5f)
                * Matrix.CreateRotationY(0f)
                * Matrix.CreateTranslation(new Vector3(0f, -1f, 0f));
            if (elapsedTime / 1000 < 120)
            {
                player1.Draw(world1, view, proj);
                player2.Draw(world2, view, proj);

                foreach (Projectile p in squProjectiles)
                {
                    world1 = Matrix.CreateScale(0.25f) * Matrix.CreateTranslation(p.Pos);
                    squProj.Draw(world1, view, proj);
                }
                foreach (Projectile p in sphProjectiles)
                {
                    world2 = Matrix.CreateScale(0.25f) * Matrix.CreateTranslation(p.Pos);
                    squProj.Draw(world2, view, proj);
                }
                plane.Draw(world, view, proj);

                spriteBatch.Begin();
                spriteBatch.DrawString(textfont, "Score: " + (player1Score.ToString()), textPos1, Color.White);
                spriteBatch.DrawString(textfont, "Score: " + (player2Score.ToString()), textPos2, Color.White);
                spriteBatch.DrawString(textfont, ((elapsedTime / 1000).ToString("0.00")), textPos3, Color.White);
                
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(textfont, "Score: " + (player1Score.ToString()), textPos1, Color.White);
                spriteBatch.DrawString(textfont, "Score: " + (player2Score.ToString()), textPos2, Color.White);
                spriteBatch.DrawString(textfont, ((elapsedTime / 1000).ToString("0.00")), textPos3, Color.White);
                spriteBatch.DrawString(textfont, "Press 'Enter' to play again!", new Vector2(300, 200), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}