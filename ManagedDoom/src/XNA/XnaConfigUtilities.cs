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
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ManagedDoom.Xna
{
    public static class XnaConfigUtilities
    {
        public static Config GetConfig(DisplayMode displayMode)
        {
            var config = new Config(ConfigUtilities.GetConfigPath());

            if (!config.IsRestoredFromFile)
            {
                var windowSize = GetDefaultWindowSize(displayMode);
                config.video_screenwidth = windowSize.width;
                config.video_screenheight = windowSize.height;
            }

            return config;
        }

        public static (int width, int height) GetDefaultWindowSize(DisplayMode displayMode)
        {
            var baseWidth = 640;
            var baseHeight = 400;

            var currentWidth = baseWidth;
            var currentHeight = baseHeight;

            while (true)
            {
                var nextWidth = currentWidth + baseWidth;
                var nextHeight = currentHeight + baseHeight;

                if (nextWidth >= 0.9 * displayMode.Width ||
                    nextHeight >= 0.9 * displayMode.Height)
                {
                    break;
                }

                currentWidth = nextWidth;
                currentHeight = nextHeight;
            }

            return (currentWidth, currentHeight);
        }

        public static XnaMusic GetMusicInstance(Config config, Wad wad)
        {
            var sfPath = Path.Combine(ConfigUtilities.GetExeDirectory(), config.audio_soundfont);
            if (File.Exists(sfPath))
            {
                return new XnaMusic(config, wad, sfPath);
            }
            else
            {
                Console.WriteLine("SoundFont '" + config.audio_soundfont + "' was not found!");
                return null;
            }
        }
    }
}
