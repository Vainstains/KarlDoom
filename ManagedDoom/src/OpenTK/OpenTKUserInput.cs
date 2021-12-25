using System;
using OpenTK.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ManagedDoom.UserInput;

namespace ManagedDoom.OpenTK
{
    public class OpenTKUserInput : IUserInput, IDisposable
    {
        private Config config;
        private GameWindow window;

        private bool[] weaponKeys;
        private int turnHeld;

        public OpenTKUserInput(Config config, GameWindow window)
        {
            this.config = config;
            this.window = window;

            weaponKeys = new bool[7];
            turnHeld = 0;
        }

        public void BuildTicCmd(TicCmd cmd)
        {
            var keyForward = IsPressed(config.key_forward);
            var keyBackward = IsPressed(config.key_backward);
            var keyStrafeLeft = IsPressed(config.key_strafeleft);
            var keyStrafeRight = IsPressed(config.key_straferight);
            var keyTurnLeft = IsPressed(config.key_turnleft);
            var keyTurnRight = IsPressed(config.key_turnright);
            var keyFire = IsPressed(config.key_fire);
            var keyUse = IsPressed(config.key_use);
            var keyRun = IsPressed(config.key_run);
            var keyStrafe = IsPressed(config.key_strafe);

            weaponKeys[0] = window.IsKeyDown(Keys.D1);
            weaponKeys[1] = window.IsKeyDown(Keys.D2);
            weaponKeys[2] = window.IsKeyDown(Keys.D3);
            weaponKeys[3] = window.IsKeyDown(Keys.D4);
            weaponKeys[4] = window.IsKeyDown(Keys.D5);
            weaponKeys[5] = window.IsKeyDown(Keys.D6);
            weaponKeys[6] = window.IsKeyDown(Keys.D7);

            cmd.Clear();

            var strafe = keyStrafe;
            var speed = keyRun ? 1 : 0;
            var forward = 0;
            var side = 0;

            if (config.game_alwaysrun)
            {
                speed = 1 - speed;
            }

            if (keyTurnLeft || keyTurnRight)
            {
                turnHeld++;
            }
            else
            {
                turnHeld = 0;
            }

            int turnSpeed;
            if (turnHeld < PlayerBehavior.SlowTurnTics)
            {
                turnSpeed = 2;
            }
            else
            {
                turnSpeed = speed;
            }

            if (strafe)
            {
                if (keyTurnRight)
                {
                    side += PlayerBehavior.SideMove[speed];
                }
                if (keyTurnLeft)
                {
                    side -= PlayerBehavior.SideMove[speed];
                }
            }
            else
            {
                if (keyTurnRight)
                {
                    cmd.AngleTurn -= (short)PlayerBehavior.AngleTurn[turnSpeed];
                }
                if (keyTurnLeft)
                {
                    cmd.AngleTurn += (short)PlayerBehavior.AngleTurn[turnSpeed];
                }
            }

            if (keyForward)
            {
                forward += PlayerBehavior.ForwardMove[speed];
            }
            if (keyBackward)
            {
                forward -= PlayerBehavior.ForwardMove[speed];
            }

            if (keyStrafeLeft)
            {
                side -= PlayerBehavior.SideMove[speed];
            }
            if (keyStrafeRight)
            {
                side += PlayerBehavior.SideMove[speed];
            }

            if (keyFire)
            {
                cmd.Buttons |= TicCmdButtons.Attack;
            }

            if (keyUse)
            {
                cmd.Buttons |= TicCmdButtons.Use;
            }

            // Check weapon keys.
            for (var i = 0; i < weaponKeys.Length; i++)
            {
                if (weaponKeys[i])
                {
                    cmd.Buttons |= TicCmdButtons.Change;
                    cmd.Buttons |= (byte)(i << TicCmdButtons.WeaponShift);
                    break;
                }
            }

            if (forward > PlayerBehavior.MaxMove)
            {
                forward = PlayerBehavior.MaxMove;
            }
            else if (forward < -PlayerBehavior.MaxMove)
            {
                forward = -PlayerBehavior.MaxMove;
            }
            if (side > PlayerBehavior.MaxMove)
            {
                side = PlayerBehavior.MaxMove;
            }
            else if (side < -PlayerBehavior.MaxMove)
            {
                side = -PlayerBehavior.MaxMove;
            }

            cmd.ForwardMove += (sbyte)forward;
            cmd.SideMove += (sbyte)side;
        }

        private bool IsPressed(KeyBinding keyBinding)
        {
            foreach (var key in keyBinding.Keys)
            {
                if (window.IsKeyDown(DoomToTK(key)))
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
        }

        public void GrabMouse()
        {
        }

        public void ReleaseMouse()
        {
        }

        public void Dispose()
        {
        }

        public static DoomKey TKToDoom(Keys tkKey)
        {
            switch (tkKey)
            {
                case Keys.Backspace: return DoomKey.Backspace;
                case Keys.Tab: return DoomKey.Tab;
                case Keys.Enter: return DoomKey.Enter;
                case Keys.Pause: return DoomKey.Pause;
                case Keys.Escape: return DoomKey.Escape;
                case Keys.Space: return DoomKey.Space;
                case Keys.PageUp: return DoomKey.PageUp;
                case Keys.PageDown: return DoomKey.PageDown;
                case Keys.End: return DoomKey.End;
                case Keys.Home: return DoomKey.Home;
                case Keys.Left: return DoomKey.Left;
                case Keys.Up: return DoomKey.Up;
                case Keys.Right: return DoomKey.Right;
                case Keys.Down: return DoomKey.Down;
                case Keys.Insert: return DoomKey.Insert;
                case Keys.Delete: return DoomKey.Delete;
                case Keys.D0: return DoomKey.Num0;
                case Keys.D1: return DoomKey.Num1;
                case Keys.D2: return DoomKey.Num2;
                case Keys.D3: return DoomKey.Num3;
                case Keys.D4: return DoomKey.Num4;
                case Keys.D5: return DoomKey.Num5;
                case Keys.D6: return DoomKey.Num6;
                case Keys.D7: return DoomKey.Num7;
                case Keys.D8: return DoomKey.Num8;
                case Keys.D9: return DoomKey.Num9;
                case Keys.A: return DoomKey.A;
                case Keys.B: return DoomKey.B;
                case Keys.C: return DoomKey.C;
                case Keys.D: return DoomKey.D;
                case Keys.E: return DoomKey.E;
                case Keys.F: return DoomKey.F;
                case Keys.G: return DoomKey.G;
                case Keys.H: return DoomKey.H;
                case Keys.I: return DoomKey.I;
                case Keys.J: return DoomKey.J;
                case Keys.K: return DoomKey.K;
                case Keys.L: return DoomKey.L;
                case Keys.M: return DoomKey.M;
                case Keys.N: return DoomKey.N;
                case Keys.O: return DoomKey.O;
                case Keys.P: return DoomKey.P;
                case Keys.Q: return DoomKey.Q;
                case Keys.R: return DoomKey.R;
                case Keys.S: return DoomKey.S;
                case Keys.T: return DoomKey.T;
                case Keys.U: return DoomKey.U;
                case Keys.V: return DoomKey.V;
                case Keys.W: return DoomKey.W;
                case Keys.X: return DoomKey.X;
                case Keys.Y: return DoomKey.Y;
                case Keys.Z: return DoomKey.Z;
                case Keys.KeyPad0: return DoomKey.Numpad0;
                case Keys.KeyPad1: return DoomKey.Numpad1;
                case Keys.KeyPad2: return DoomKey.Numpad2;
                case Keys.KeyPad3: return DoomKey.Numpad3;
                case Keys.KeyPad4: return DoomKey.Numpad4;
                case Keys.KeyPad5: return DoomKey.Numpad5;
                case Keys.KeyPad6: return DoomKey.Numpad6;
                case Keys.KeyPad7: return DoomKey.Numpad7;
                case Keys.KeyPad8: return DoomKey.Numpad8;
                case Keys.KeyPad9: return DoomKey.Numpad9;
                case Keys.KeyPadMultiply: return DoomKey.Multiply;
                case Keys.KeyPadAdd: return DoomKey.Add;
                case Keys.KeyPadSubtract: return DoomKey.Subtract;
                case Keys.KeyPadDivide: return DoomKey.Divide;
                case Keys.F1: return DoomKey.F1;
                case Keys.F2: return DoomKey.F2;
                case Keys.F3: return DoomKey.F3;
                case Keys.F4: return DoomKey.F4;
                case Keys.F5: return DoomKey.F5;
                case Keys.F6: return DoomKey.F6;
                case Keys.F7: return DoomKey.F7;
                case Keys.F8: return DoomKey.F8;
                case Keys.F9: return DoomKey.F9;
                case Keys.F10: return DoomKey.F10;
                case Keys.F11: return DoomKey.F11;
                case Keys.F12: return DoomKey.F12;
                case Keys.F13: return DoomKey.F13;
                case Keys.F14: return DoomKey.F14;
                case Keys.F15: return DoomKey.F15;
                case Keys.LeftShift: return DoomKey.LShift;
                case Keys.RightShift: return DoomKey.RShift;
                case Keys.LeftControl: return DoomKey.LControl;
                case Keys.RightControl: return DoomKey.RControl;
                case Keys.LeftAlt: return DoomKey.LAlt;
                case Keys.RightAlt: return DoomKey.RAlt;
                default: return DoomKey.Unknown;
            }
        }

        public static Keys DoomToTK(DoomKey key)
        {
            switch (key)
            {
                case DoomKey.Backspace: return Keys.Backspace;
                case DoomKey.Tab: return Keys.Tab;
                case DoomKey.Enter: return Keys.Enter;
                case DoomKey.Pause: return Keys.Pause;
                case DoomKey.Escape: return Keys.Escape;
                case DoomKey.Space: return Keys.Space;
                case DoomKey.PageUp: return Keys.PageUp;
                case DoomKey.PageDown: return Keys.PageDown;
                case DoomKey.End: return Keys.End;
                case DoomKey.Home: return Keys.Home;
                case DoomKey.Left: return Keys.Left;
                case DoomKey.Up: return Keys.Up;
                case DoomKey.Right: return Keys.Right;
                case DoomKey.Down: return Keys.Down;
                case DoomKey.Insert: return Keys.Insert;
                case DoomKey.Delete: return Keys.Delete;
                case DoomKey.Num0: return Keys.D0;
                case DoomKey.Num1: return Keys.D1;
                case DoomKey.Num2: return Keys.D2;
                case DoomKey.Num3: return Keys.D3;
                case DoomKey.Num4: return Keys.D4;
                case DoomKey.Num5: return Keys.D5;
                case DoomKey.Num6: return Keys.D6;
                case DoomKey.Num7: return Keys.D7;
                case DoomKey.Num8: return Keys.D8;
                case DoomKey.Num9: return Keys.D9;
                case DoomKey.A: return Keys.A;
                case DoomKey.B: return Keys.B;
                case DoomKey.C: return Keys.C;
                case DoomKey.D: return Keys.D;
                case DoomKey.E: return Keys.E;
                case DoomKey.F: return Keys.F;
                case DoomKey.G: return Keys.G;
                case DoomKey.H: return Keys.H;
                case DoomKey.I: return Keys.I;
                case DoomKey.J: return Keys.J;
                case DoomKey.K: return Keys.K;
                case DoomKey.L: return Keys.L;
                case DoomKey.M: return Keys.M;
                case DoomKey.N: return Keys.N;
                case DoomKey.O: return Keys.O;
                case DoomKey.P: return Keys.P;
                case DoomKey.Q: return Keys.Q;
                case DoomKey.R: return Keys.R;
                case DoomKey.S: return Keys.S;
                case DoomKey.T: return Keys.T;
                case DoomKey.U: return Keys.U;
                case DoomKey.V: return Keys.V;
                case DoomKey.W: return Keys.W;
                case DoomKey.X: return Keys.X;
                case DoomKey.Y: return Keys.Y;
                case DoomKey.Z: return Keys.Z;
                case DoomKey.Numpad0: return Keys.KeyPad0;
                case DoomKey.Numpad1: return Keys.KeyPad1;
                case DoomKey.Numpad2: return Keys.KeyPad2;
                case DoomKey.Numpad3: return Keys.KeyPad3;
                case DoomKey.Numpad4: return Keys.KeyPad4;
                case DoomKey.Numpad5: return Keys.KeyPad5;
                case DoomKey.Numpad6: return Keys.KeyPad6;
                case DoomKey.Numpad7: return Keys.KeyPad7;
                case DoomKey.Numpad8: return Keys.KeyPad8;
                case DoomKey.Numpad9: return Keys.KeyPad9;
                case DoomKey.Multiply: return Keys.KeyPadMultiply;
                case DoomKey.Add: return Keys.KeyPadAdd;
                case DoomKey.Subtract: return Keys.KeyPadSubtract;
                case DoomKey.Divide: return Keys.KeyPadDivide;
                case DoomKey.F1: return Keys.F1;
                case DoomKey.F2: return Keys.F2;
                case DoomKey.F3: return Keys.F3;
                case DoomKey.F4: return Keys.F4;
                case DoomKey.F5: return Keys.F5;
                case DoomKey.F6: return Keys.F6;
                case DoomKey.F7: return Keys.F7;
                case DoomKey.F8: return Keys.F8;
                case DoomKey.F9: return Keys.F9;
                case DoomKey.F10: return Keys.F10;
                case DoomKey.F11: return Keys.F11;
                case DoomKey.F12: return Keys.F12;
                case DoomKey.F13: return Keys.F13;
                case DoomKey.F14: return Keys.F14;
                case DoomKey.F15: return Keys.F15;
                case DoomKey.LShift: return Keys.LeftShift;
                case DoomKey.RShift: return Keys.RightShift;
                case DoomKey.LControl: return Keys.LeftControl;
                case DoomKey.RControl: return Keys.RightControl;
                case DoomKey.LAlt: return Keys.LeftAlt;
                case DoomKey.RAlt: return Keys.RightAlt;
                default: return Keys.Unknown;
            }
        }

        public int MaxMouseSensitivity
        {
            get
            {
                return 9;
            }
        }

        public int MouseSensitivity
        {
            get
            {
                return 3;
            }

            set
            {
            }
        }
    }
}
