namespace OpenGLviaFramebuffer
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using OpenTK.Graphics.OpenGL;

    using Size = System.Drawing.Size;

    public partial class MainWindow : Window
    {
        #region Fields

        private WriteableBitmap backbuffer;

        private FrameBufferHandler framebufferHandler;

        private int frames;

        private DateTime lastMeasureTime;

        private Renderer renderer;

        #endregion

        #region Constructors and Destructors

        public MainWindow()
        {
            this.InitializeComponent();

            this.renderer = new Renderer(new Size(400, 400));
            this.framebufferHandler = new FrameBufferHandler();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += this.TimerOnTick;
            timer.Start();
        }

        #endregion

        #region Methods

        private void Render()
        {
            if (this.image.ActualWidth <= 0 || this.image.ActualHeight <= 0)
            {
                return;
            }

			this.framebufferHandler.Prepare( new Size( (int)this.imageContainer.ActualWidth , (int)this.imageContainer.ActualHeight ) );

			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();
			float halfWidth = (float)(this.imageContainer.ActualWidth / 2);
			float halfHeight = (float)(this.imageContainer.ActualHeight / 2);
			GL.Ortho( -halfWidth , halfWidth , -halfHeight , halfHeight , 1000 , -1000 );
			GL.Viewport( 0 , 0 , (int)this.imageContainer.ActualWidth , (int)this.imageContainer.ActualHeight );

			this.renderer.Render();

            GL.Finish();

            this.framebufferHandler.Cleanup(ref this.backbuffer);

            if (this.backbuffer != null)
            {
                this.image.Source = this.backbuffer;
            }

            this.frames++;
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (DateTime.Now.Subtract(this.lastMeasureTime) > TimeSpan.FromSeconds(1))
            {
                this.Title = this.frames + "fps";
                this.frames = 0;
                this.lastMeasureTime = DateTime.Now;
            }

            this.Render();
        }

        #endregion
    }
}