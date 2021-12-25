using System;
using System.Runtime.ExceptionServices;
using OpenTK.Graphics.OpenGL4;
using LearnOpenTK.Common;
using ManagedDoom.Video;

namespace ManagedDoom.OpenTK
{
    public sealed class OpenTKVideo : IVideo, IDisposable
    {
        private Renderer renderer;

        private int textureWidth;
        private int textureHeight;

        private float[] vertices;
        private uint[] indices;

        private int elementBufferObject;
        private int vertexBufferObject;
        private int vertexArrayObject;
        private Shader shader;


        private byte[] textureData;
        private Texture texture;

        public OpenTKVideo(Config config, GameContent content)
        {
            try
            {
                Console.Write("Initialize video: ");

                renderer = new Renderer(config, content);

                config.video_gamescreensize = Math.Clamp(config.video_gamescreensize, 0, MaxWindowSize);
                config.video_gammacorrection = Math.Clamp(config.video_gammacorrection, 0, MaxGammaCorrectionLevel);

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

                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

                vertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(vertexArrayObject);

                vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

                shader = new Shader("shader.vert", "shader.frag");
                shader.Use();

                var vertexLocation = shader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                var texCoordLocation = shader.GetAttribLocation("aTexCoord");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

                textureData = new byte[4 * renderer.Width * renderer.Height];
                texture = new Texture(textureWidth, textureHeight);
                texture.Use();

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

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindVertexArray(vertexArrayObject);
            texture.Use();
            shader.Use();
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
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
