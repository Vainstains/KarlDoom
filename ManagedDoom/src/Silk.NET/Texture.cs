using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace ManagedDoom.Silk
{
    public class Texture : IDisposable
    {
        private GL gl;

        private uint handle;

        public unsafe Texture(GL gl, int width, int height)
        {
            this.gl = gl;

            handle = gl.GenTexture();

            Bind();
            gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
        }

        public unsafe void Update(byte[] pixels, int width, int height, int x, int y)
        {
            fixed (void* data = pixels)
            {
                gl.TexSubImage2D(
                    TextureTarget.Texture2D,
                    0,
                    x, y,
                    (uint)width, (uint)height,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte, data);
            }
        }

        public void Bind()
        {
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, handle);
        }

        public void Dispose()
        {
            gl.DeleteTexture(handle);
        }
    }
}
