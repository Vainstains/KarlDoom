//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//



using System;
using System.Runtime.ExceptionServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ManagedDoom.Video;

namespace ManagedDoom.Xna
{
    public sealed class XnaVideo : IVideo, IDisposable
    {
        private XnaDoom xnaDoom;

        private Renderer renderer;

        private int windowWidth;
        private int windowHeight;

        private int textureWidth;
        private int textureHeight;

        private byte[] textureData;
        private Texture2D texture;
        private SpriteBatch sprite;

        public XnaVideo(XnaDoom xnaDoom, Config config, GameContent content)
        {
            try
            {
                Console.Write("Initialize video: ");

                this.xnaDoom = xnaDoom;

                renderer = new Renderer(config, content);

                config.video_gamescreensize = Math.Clamp(config.video_gamescreensize, 0, MaxWindowSize);
                config.video_gammacorrection = Math.Clamp(config.video_gammacorrection, 0, MaxGammaCorrectionLevel);

                windowWidth = xnaDoom.GraphicsDevice.PresentationParameters.BackBufferWidth;
                windowHeight = xnaDoom.GraphicsDevice.PresentationParameters.BackBufferHeight;

                if (config.video_highresolution)
                {
                    textureWidth = 512;
                    textureHeight = 1024;
                }
                else
                {
                    textureWidth = 256;
                    textureHeight = 512;
                }

                textureData = new byte[4 * renderer.Width * renderer.Height];

                texture = new Texture2D(xnaDoom.GraphicsDevice, textureWidth, textureHeight);
                sprite = new SpriteBatch(xnaDoom.GraphicsDevice);

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public void Render(Doom doom)
        {
            renderer.Render(doom, textureData);

            texture.SetData(
                0,
                new Rectangle(0, 0, renderer.Height, renderer.Width),
                textureData,
                0,
                textureData.Length);

            sprite.Begin(
                SpriteSortMode.Immediate,
                BlendState.Opaque,
                SamplerState.PointClamp,
                null,
                null,
                null,
                Matrix.Identity);

            sprite.Draw(
                texture,
                new Rectangle(0, 0, windowHeight, windowWidth),
                new Rectangle(0, 0, renderer.Height, renderer.Width),
                Color.White,
                -MathF.PI / 2,
                new Vector2(renderer.Height, 0),
                SpriteEffects.FlipHorizontally,
                0F);

            sprite.End();
        }

        public void InitializeWipe()
        {
            renderer.InitializeWipe();
        }

        public bool HasFocus()
        {
            return true;
        }

        public void Dispose()
        {
            Console.WriteLine("Shutdown renderer.");

            if (sprite != null)
            {
                sprite.Dispose();
                sprite = null;
            }

            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
        }

        public int WipeBandCount => renderer.WipeBandCount;
        public int WipeHeight => renderer.WipeHeight;

        public int MaxWindowSize => renderer.MaxWindowSize;

        public int WindowSize
        {
            get => renderer.WindowSize;
            set => renderer.WindowSize = value;
        }

        public bool DisplayMessage
        {
            get => renderer.DisplayMessage;
            set => renderer.DisplayMessage = value;
        }

        public int MaxGammaCorrectionLevel => renderer.MaxGammaCorrectionLevel;

        public int GammaCorrectionLevel
        {
            get => renderer.GammaCorrectionLevel;
            set => renderer.GammaCorrectionLevel = value;
        }
    }
}
