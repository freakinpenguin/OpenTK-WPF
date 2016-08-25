namespace OpenGLviaFramebuffer
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using OpenTK;
    using OpenTK.Graphics;

    using FramebufferAttachment = OpenTK.Graphics.OpenGL.FramebufferAttachment;
    using FramebufferErrorCode = OpenTK.Graphics.OpenGL.FramebufferErrorCode;
    using FramebufferTarget = OpenTK.Graphics.OpenGL.FramebufferTarget;
    using GL = OpenTK.Graphics.OpenGL.GL;
    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
    using PixelInternalFormat = OpenTK.Graphics.OpenGL.PixelInternalFormat;
    using PixelType = OpenTK.Graphics.OpenGL.PixelType;
    using RenderbufferStorage = OpenTK.Graphics.OpenGL.RenderbufferStorage;
    using RenderbufferTarget = OpenTK.Graphics.OpenGL.RenderbufferTarget;
    using Size = System.Drawing.Size;
    using TextureMagFilter = OpenTK.Graphics.OpenGL.TextureMagFilter;
    using TextureMinFilter = OpenTK.Graphics.OpenGL.TextureMinFilter;
    using TextureParameterName = OpenTK.Graphics.OpenGL.TextureParameterName;
    using TextureTarget = OpenTK.Graphics.OpenGL.TextureTarget;
    using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;

    internal class FrameBufferHandler
    {
        #region Fields

        private int depthbufferId;

        private int framebufferId;

        private GLControl glControl;

        private bool loaded;

        private Size size;

		private int colorbufferId;

		private byte[] readBuf;

		#endregion

		#region Constructors and Destructors

		public FrameBufferHandler()
        {
            this.loaded = false;
            this.size = Size.Empty;
            this.framebufferId = -1;

            this.glControl = new GLControl(new GraphicsMode(DisplayDevice.Default.BitsPerPixel, 16, 0, 4, 0, 2, false));
            this.glControl.MakeCurrent();
        }

        #endregion

        #region Methods

        internal void Cleanup(ref WriteableBitmap backbuffer)
        {
            if (backbuffer == null || backbuffer.Width != this.size.Width || backbuffer.Height != this.size.Height)
            {
                backbuffer = new WriteableBitmap(
                    this.size.Width, 
                    this.size.Height, 
                    96, 
                    96, 
                    PixelFormats.Pbgra32, 
                    BitmapPalettes.WebPalette);
				readBuf = new byte[this.size.Width*this.size.Height*4];
			}

			GL.ReadPixels( 0 , 0 , this.size.Width , this.size.Height , PixelFormat.Bgra , PixelType.UnsignedByte , readBuf );

			// copy pixels upside down
			backbuffer.Lock();

			var src = new Int32Rect( 0 , 0 , (int)backbuffer.Width , 1 );
			for ( int y = 0; y<(int)backbuffer.Height; y++ )
			{
				src.Y = (int)backbuffer.Height - y - 1;
				backbuffer.WritePixels( src , readBuf , backbuffer.BackBufferStride , 0 , y );
			}

			backbuffer.AddDirtyRect(new Int32Rect(0, 0, (int)backbuffer.Width, (int)backbuffer.Height));
            backbuffer.Unlock();
        }

		internal void Prepare(Size framebuffersize)
        {
            if (GraphicsContext.CurrentContext != this.glControl.Context)
            {
                this.glControl.MakeCurrent();
            }

            if (framebuffersize != this.size || this.loaded == false)
            {
                this.size = framebuffersize;
                this.CreateFramebuffer();
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.framebufferId);
        }

        private void CreateFramebuffer()
        {
            this.glControl.MakeCurrent();

            if (this.framebufferId > 0)
            {
                GL.DeleteFramebuffer(this.framebufferId);
            }

            if (this.depthbufferId > 0)
            {
                GL.DeleteRenderbuffer(this.depthbufferId);
            }

			if ( this.colorbufferId > 0 )
			{
				GL.DeleteRenderbuffer( this.colorbufferId );
			}

            this.framebufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.framebufferId);

			this.colorbufferId = GL.GenRenderbuffer();
			GL.BindRenderbuffer( RenderbufferTarget.Renderbuffer , this.colorbufferId );
			GL.RenderbufferStorage(
				RenderbufferTarget.Renderbuffer ,
				RenderbufferStorage.Rgba8 ,
				this.size.Width ,
				this.size.Height );
			GL.FramebufferRenderbuffer(
				FramebufferTarget.Framebuffer ,
				FramebufferAttachment.ColorAttachment0 ,
				RenderbufferTarget.Renderbuffer ,
				this.colorbufferId );

			this.depthbufferId = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.depthbufferId);
            GL.RenderbufferStorage(
                RenderbufferTarget.Renderbuffer, 
                RenderbufferStorage.DepthComponent24, 
                this.size.Width, 
                this.size.Height);
            GL.FramebufferRenderbuffer(
                FramebufferTarget.Framebuffer, 
                FramebufferAttachment.DepthAttachment, 
                RenderbufferTarget.Renderbuffer, 
                this.depthbufferId);

            FramebufferErrorCode error = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (error != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception();
            }

            this.loaded = true;
        }

		#endregion
	}
}