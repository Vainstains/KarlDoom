using System;
using System.Runtime.ExceptionServices;
using Silk.NET.OpenGL;
using ManagedDoom.Video;

namespace ManagedDoom.Silk
{
    public sealed class SilkVideo : IVideo, IDisposable
    {
        private Renderer renderer;

        private GL gl;

        private int textureWidth;
        private int textureHeight;

        private float[] vertices;
        private uint[] indices;

        private Tutorial.BufferObject<float> vbo;
        private Tutorial.BufferObject<uint> ebo;
        private Tutorial.VertexArrayObject<float, uint> vao;
        private Tutorial.Shader shader;

        private byte[] textureData;
        private Texture texture;

        public SilkVideo(Config config, GameContent content, GL gl)
        {
            try
            {
                Console.Write("Initialize video: ");

                renderer = new Renderer(config, content);

                config.video_gamescreensize = Math.Clamp(config.video_gamescreensize, 0, MaxWindowSize);
                config.video_gammacorrection = Math.Clamp(config.video_gammacorrection, 0, MaxGammaCorrectionLevel);

                this.gl = gl;

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

                vertices = new float[]
                {
                     1F,  1F, 0.0F, 0F,                                    (float)renderer.Width / textureHeight,
                     1F, -1F, 0.0F, (float)renderer.Height / textureWidth, (float)renderer.Width / textureHeight,
                    -1F, -1F, 0.0F, (float)renderer.Height / textureWidth, 0F,
                    -1F,  1F, 0.0F, 0F,                                    0F
                };

                indices = new uint[]
                {
                    0, 1, 3,
                    1, 2, 3
                };

                ebo = new Tutorial.BufferObject<uint>(gl, indices, BufferTargetARB.ElementArrayBuffer);
                vbo = new Tutorial.BufferObject<float>(gl, vertices, BufferTargetARB.ArrayBuffer);
                vao = new Tutorial.VertexArrayObject<float, uint>(gl, vbo, ebo);

                vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
                vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

                shader = new Tutorial.Shader(gl, "shader.vert", "shader.frag");

                textureData = new byte[4 * renderer.Width * renderer.Height];
                texture = new Texture(gl, textureWidth, textureHeight);

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public unsafe void Render(Doom doom)
        {
            renderer.Render(doom, textureData);

            texture.Update(textureData, renderer.Height, renderer.Width, 0, 0);

            gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            vao.Bind();
            shader.Use();
            texture.Bind();
            shader.SetUniform("uTexture0", 0);

            gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);
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

            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }

            if (shader != null)
            {
                shader.Dispose();
                shader = null;
            }

            if (vao != null)
            {
                vao.Dispose();
                vao = null;
            }

            if (vbo != null)
            {
                vbo.Dispose();
                vbo = null;
            }

            if (ebo != null)
            {
                ebo.Dispose();
                ebo = null;
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
