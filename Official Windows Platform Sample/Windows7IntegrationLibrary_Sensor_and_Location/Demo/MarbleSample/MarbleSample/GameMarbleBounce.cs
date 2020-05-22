// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Globalization;
using Windows7.Sensors;
using Windows7.Sensors.Sensors.Light;

namespace Microsoft.Sensors.Samples
{
    /// <summary>
    /// GameMarbleBounce moves the marble around the screen based on accelerometer input
    /// and changes the background color based on ambient light.
    /// </summary>
    public class GameMarbleBounce : Microsoft.Xna.Framework.Game
    {
        AmbientLightSensor _lightSensor;
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Rectangle _viewportRect;
        Color _backColor = Color.DimGray;
        float _illuminanceLux;

        string _displayText;
        SpriteFont _font;
        Vector2 _fontPosition;
        double _fontRotationInRadians;
        const int TextMarginX = 10;
        const int TextMarginY = 10;

        Marble _marble;

        OrientationSourceProvider _orientationSourceProvider;

        /// <summary>
        /// Constructs a new GameMarbleBounce
        /// </summary>
        public GameMarbleBounce()
        {
            AmbientLightSensor[] lightSensors = SensorManager.GetSensorsByType<AmbientLightSensor>();
            if (lightSensors.Length > 0)
            {
                _lightSensor = lightSensors[0];
                _lightSensor.ReportInterval = _lightSensor.MinReportInterval;
            }

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _orientationSourceProvider = new OrientationSourceProvider();
            _orientationSourceProvider.OrientationChanged += new EventHandler(OnOrientationChanged);
            base.Initialize();
        }

        /// <summary>
        /// Move and rotate the display text when the screen orientation changes.
        /// The text is positioned 10 pixels from the top left corner based on the
        /// orientation of the screen.
        /// </summary>
        /// <param name="oldOrientation">The previous orientation.</param>
        /// <param name="newOrientation">The new orientation.</param>
        void OnOrientationChanged(object sender, EventArgs e)
        {
            Orientation? orientation = _orientationSourceProvider.Orientation;

            int viewportWidth = _graphics.GraphicsDevice.Viewport.Width;
            int viewportHeight = _graphics.GraphicsDevice.Viewport.Height;

            switch (orientation)
            {
                case Orientation.Angle0:
                    _fontRotationInRadians = 0;
                    _fontPosition = new Vector2(TextMarginX, TextMarginY);
                    break;
                case Orientation.Angle90:
                    _fontRotationInRadians = Math.PI / 2;
                    _fontPosition = new Vector2(viewportWidth - TextMarginX, TextMarginY);
                    break;
                case Orientation.Angle180:
                    _fontRotationInRadians = Math.PI;
                    _fontPosition = new Vector2(viewportWidth - TextMarginX, viewportHeight - TextMarginY);
                    break;
                case Orientation.Angle270:
                    _fontRotationInRadians = -Math.PI / 2;
                    _fontPosition = new Vector2(TextMarginX, viewportHeight - TextMarginY);
                    break;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create the texture for the marble
            Texture2D circleTexture = Content.Load<Texture2D>("marble");

            // Load the font for the display text and set defaults for position and rotation.
            _font = Content.Load<SpriteFont>("DebugText");
            _fontPosition = new Vector2(TextMarginX, TextMarginY);
            _fontRotationInRadians = 0;

            // viewportRect stores the size of the screen 
            _viewportRect = new Rectangle(0,
                                          0,
                                          _graphics.GraphicsDevice.Viewport.Width,
                                          _graphics.GraphicsDevice.Viewport.Height);


            // Create the marble and pass in the screen size so the marble can 
            // bounce off the sides of the display area.
            _marble = new Marble(circleTexture, _viewportRect);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Microsoft.Xna.Framework.Input.Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            AmbientLightSensorDataReport lightReport = (AmbientLightSensorDataReport)_lightSensor.GetDataReport();
            _illuminanceLux = lightReport.IlluminanceLux;

            // If the accelerometer is providing a value, we treat it as if it is measuring
            // the direction of gravity and pass it into the marble so it can move around the screen.
            // If the accelerometer is providing no value, the marble will not move.
            _marble.UpdatePosition(gameTime, _orientationSourceProvider.Vector);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            float normLux = (float) Math.Log(_illuminanceLux, 100000.0);
            _graphics.GraphicsDevice.Clear(new Color(normLux, normLux, normLux));
            _marble.UpdateIlluminance(normLux);

            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            // Draw the marble.
            _marble.Draw(_spriteBatch);

            // Set the displayText to the current values of the sensors.
            _displayText = String.Format(CultureInfo.CurrentCulture, "X: {0:0.0000}\nY: {1:0.0000}\nZ: {2:0.0000}\n\nVelocity: {3:0.0000}",
                            _marble.VectorGravity.X,
                            _marble.VectorGravity.Y,
                            _marble.VectorGravity.Z,
                            _marble.Velocity.Length());

            // Draw the displayText to the screen based on the orientation of the screen.
            _spriteBatch.DrawString(_font, _displayText, _fontPosition, Color.White,
                (float)_fontRotationInRadians, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}