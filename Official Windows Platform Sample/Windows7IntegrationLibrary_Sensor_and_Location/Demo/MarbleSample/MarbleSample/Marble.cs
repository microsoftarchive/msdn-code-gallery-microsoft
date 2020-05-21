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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Windows7.Sensors;

namespace Microsoft.Sensors.Samples
{
    public class ColorUtils
    {
        public static Color ColorFromHSV(float h, float s, float v)
        {
            int i;
            float f, p, q, t;
            float r, g, b;
            if (s == 0)
            {
                // achromatic (grey)
                r = g = b = v;
                return new Color(r, g, b);
            }

            h /= 60.0F;			// sector 0 to 5
            i = (int)h;
            f = h - i;			// fractional part of h
            p = v * (1.0F - s);
            q = v * (1.0F - s * f);
            t = v * (1.0F - s * (1.0F - f));
            switch (i)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                default:		// case 5:
                    r = v;
                    g = p;
                    b = q;
                    break;
            }

            return new Color(r, g, b);
        }
    }

    public class Marble
    {
        float _illuminanceLuxNorm;
        Texture2D _circleTexture;
        Rectangle _bounds;
        Vector2 _position;
        Vector2 _velocity;
        int _width;
        int _height;
        const float VelocityModifier = 0.025f;
        const float MaxVelocity = 100.0f;
        const float BounceFactor = 0.6f;
        // Convert G to units in pixel space. G is 9.8 meters per second but we are in pixel space so we
        // just pick a value that feels good.
        const float GravitationalConstant = 10;


        /// <summary>
        /// Construct a new marble
        /// </summary>
        /// <param name="circleTexture">The texture of the marble</param>
        /// <param name="bounds">The bounds of the screen off which the marble will bounce</param>
        public Marble(Texture2D circleTexture, Rectangle bounds)
        {
            this._circleTexture = circleTexture;
            this._bounds = bounds;
            _velocity = Vector2.Zero;

            // Start the marble at the center of the screen.
            _position = new Vector2(bounds.Width / 2, bounds.Height / 2);
            // Without input the marble should not move.
            VectorGravity = Vector3.Zero;
            _width = circleTexture.Width;
            _height = circleTexture.Height;
        }

        /// <summary>
        /// The direction of gravity
        /// </summary>
        public Vector3 VectorGravity
        {
            get;
            set;
        }

        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
        }

        public void UpdateIlluminance(float luxNorm)
        {
            _illuminanceLuxNorm = luxNorm;
        }

        /// <summary>
        /// Draw the marble to the screen
        /// </summary>
        /// <param name="spriteBatch">The sprite to draw on.</param>
        public void Draw(SpriteBatch spriteBatch)
        {            
            spriteBatch.Draw(_circleTexture, _position, ColorUtils.ColorFromHSV(_illuminanceLuxNorm * 360.0f, 1.0f, 0.8f));
        }

        /// <summary>
        /// Update the position of the marble based on accelerometer input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="vectorGravity">The direction of gravity.</param>
        public void UpdatePosition(GameTime gameTime, Vector3? vectorGravity)
        {
            if (vectorGravity == null)
            {
                VectorGravity = Vector3.Zero;
                _velocity = Vector2.Zero;
                return;
            }
            VectorGravity = vectorGravity.Value;

            // We map the accelerometer axes to the game axis.
            // We don't care about the Z direction since it points into the screen.
            // The accelerometer is oriented such that +X is right, +Y is forward, and +Z is up
            // so to convert it into screen space we invert the Y value.
            Vector2 vectorGravityDirection = new Vector2();
            vectorGravityDirection.X = vectorGravity.Value.X;
            vectorGravityDirection.Y = -vectorGravity.Value.Y;

            float deltaTimeInMilliseconds = gameTime.ElapsedGameTime.Milliseconds * VelocityModifier;
            // Add the gravity from the accelerometer to the velocity
            // to make the marble appear to fall toward the ground.
            // Multiply it by game time to make sure the speed is the same for all frame rates.
            Vector2 acceleration = vectorGravityDirection * deltaTimeInMilliseconds * GravitationalConstant;
            _velocity += acceleration;

            CalulateNewVelocityForWallBounce(deltaTimeInMilliseconds, acceleration);

            // Cap the velocity at a maximum of 50 pixels per millisecond
            if (_velocity.Length() > MaxVelocity)
            {
                _velocity *= MaxVelocity / _velocity.Length();
            }

            // Move the marble to the new position based on its velocity.
            // Multiply it by game time to make sure the speed is the same for all frame rates.
            _position += _velocity * deltaTimeInMilliseconds;
        }

        private void CalulateNewVelocityForWallBounce(float delta, Vector2 acceleration)
        {
            // If the next position based on this velocity would bring it out of bounds,
            // apply a normal force to prevent it from wobbling on the wall and
            // reverse the velocity to bounce off the wall and dampen to the velocity
            // to make it bounce realistically.
            Vector2 nextPosition = _position + _velocity * delta;
            if (nextPosition.X < 0)
            {
                //_velocity.X -= acceleration.X;
                //_velocity.X = -_velocity.X * BounceFactor;
                //nextPosition.X = 0;
                _velocity.X = 0;
            }
            else if (nextPosition.X + _width > _bounds.Right)
            {
                //_velocity.X -= acceleration.X;
                //_velocity.X = -_velocity.X * BounceFactor;
                //nextPosition.X = _bounds.Right - _width;
                _velocity.X = 0;
            }

            if (nextPosition.Y < 0)
            {
                //_velocity.Y -= acceleration.Y;
                //_velocity.Y = -_velocity.Y * BounceFactor;
                //nextPosition.Y = 0;
                _velocity.Y = 0;
            }
            else if (nextPosition.Y + _height > _bounds.Bottom)
            {
                //_velocity.Y -= acceleration.Y;
                //_velocity.Y = -_velocity.Y * BounceFactor;
                //nextPosition.Y = _bounds.Bottom - _height;
                _velocity.Y = 0;
            }
        }
    }
}