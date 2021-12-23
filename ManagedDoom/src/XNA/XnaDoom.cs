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
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ManagedDoom.Xna
{
    public sealed class XnaDoom : Game
    {
        private CommandLineArgs args;

        private GraphicsDeviceManager graphics;

        private Config config;

        private GameContent content;

        private XnaVideo video;
        private XnaSound sound;
        private XnaMusic music;
        private XnaUserInput userInput;

        private Doom doom;

        public XnaDoom(CommandLineArgs args)
        {
            this.args = args;

            graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            var displayMode = graphics.GraphicsDevice.DisplayMode;

            config = XnaConfigUtilities.GetConfig(displayMode);

            config.video_screenwidth = Math.Clamp(config.video_screenwidth, 320, 3200);
            config.video_screenheight = Math.Clamp(config.video_screenheight, 200, 2000);

            graphics.PreferredBackBufferWidth = config.video_screenwidth;
            graphics.PreferredBackBufferHeight = config.video_screenheight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            try
            {
                content = new GameContent(ConfigUtilities.GetWadPaths(args));

                video = new XnaVideo(this, config, content);

                if (!args.nosound.Present && !args.nosfx.Present)
                {
                    sound = new XnaSound(config, content.Wad);
                }

                if (!args.nosound.Present && !args.nomusic.Present)
                {
                    music = XnaConfigUtilities.GetMusicInstance(config, content.Wad);
                }

                userInput = new XnaUserInput(this, config, !args.nomouse.Present);

                Window.KeyDown += KeyPressed;
                Window.KeyUp += KeyReleased;

                if (!args.timedemo.Present)
                {
                    TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 35);
                }

                doom = new Doom(args, config, content, video, sound, music, userInput);
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            if (userInput != null)
            {
                userInput.Dispose();
                userInput = null;
            }

            if (music != null)
            {
                music.Dispose();
                music = null;
            }

            if (sound != null)
            {
                sound.Dispose();
                sound = null;
            }

            if (video != null)
            {
                video.Dispose();
                video = null;
            }

            if (content != null)
            {
                content.Dispose();
                content = null;
            }

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (doom.Update() == UpdateResult.Completed)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            video.Render(doom);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            config.Save(ConfigUtilities.GetConfigPath());

            base.OnExiting(sender, args);
        }

        private void KeyPressed(object sender, InputKeyEventArgs e)
        {
            doom.PostEvent(new DoomEvent(EventType.KeyDown, XnaUserInput.XnaToDoom(e.Key)));
        }

        private void KeyReleased(object sender, InputKeyEventArgs e)
        {
            doom.PostEvent(new DoomEvent(EventType.KeyUp, XnaUserInput.XnaToDoom(e.Key)));
        }

        public string QuitMessage => doom.QuitMessage;
    }
}
