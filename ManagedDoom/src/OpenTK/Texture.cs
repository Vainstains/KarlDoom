using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace ManagedDoom.OpenTK
{
    public class Texture : IDisposable
    {
        private int handle;

        public unsafe Texture(int width, int height)
        {
            handle = GL.GenTexture();

            Use();
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        public unsafe void Update(byte[] pixels, int width, int height, int x, int y)
        {
            fixed (void* data = pixels)
            {
                GL.TexSubImage2D(
                    TextureTarget.Texture2D,
                    0,
                    x, y,
                    width, height,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte, (IntPtr)data);
            }
        }

        public void Use()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(handle);
        }
    }
}
