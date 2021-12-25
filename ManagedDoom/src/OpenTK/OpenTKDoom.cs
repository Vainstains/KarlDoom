using System;
using System.ComponentModel;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace ManagedDoom.OpenTK
{
    public class OpenTKDoom : IDisposable
    {
        private CommandLineArgs args;

        private Config config;

        private GameWindow window;

        private GameContent content;

        private OpenTKVideo video;
        private OpenTKUserInput userInput;

        private Doom doom;

        public OpenTKDoom(CommandLineArgs args)
        {
            this.args = args;

            config = new Config(ConfigUtilities.GetConfigPath());

            var gameWindowSettings = new GameWindowSettings
            {
                UpdateFrequency = 35,
                RenderFrequency = 35
            };

            var nativeWindowSettings = new NativeWindowSettings
            {
                Size = new Vector2i(2 * 640, 2 * 400)
            };

            window = new GameWindow(gameWindowSettings, nativeWindowSettings);

            window.Load += OnLoad;
            window.UpdateFrame += OnUpdate;
            window.RenderFrame += OnRender;
            window.KeyDown += KeyDown;
            window.KeyUp += KeyUp;
        }

        public void Run()
        {
            window.Run();
        }

        public void KeyDown(KeyboardKeyEventArgs obj)
        {
            doom.PostEvent(new DoomEvent(EventType.KeyDown, OpenTKUserInput.TKToDoom(obj.Key)));
        }

        public void KeyUp(KeyboardKeyEventArgs obj)
        {
            doom.PostEvent(new DoomEvent(EventType.KeyUp, OpenTKUserInput.TKToDoom(obj.Key)));
        }

        private void OnLoad()
        {
            content = new GameContent(ConfigUtilities.GetWadPaths(args));

            video = new OpenTKVideo(config, content);
            userInput = new OpenTKUserInput(config, window);

            doom = new Doom(args, config, content, video, null, null, userInput);
        }

        private void OnUpdate(FrameEventArgs obj)
        {
            if (doom.Update() == UpdateResult.Completed)
            {
                window.Close();
            }
        }

        private void OnRender(FrameEventArgs obj)
        {
            video.Render(doom);

            window.SwapBuffers();
        }

        public void Dispose()
        {
            if (userInput != null)
            {
                userInput.Dispose();
                userInput = null;
            }

            if (video != null)
            {
                video.Dispose();
                video = null;
            }
        }

        public string QuitMessage => doom.QuitMessage;
    }
}
