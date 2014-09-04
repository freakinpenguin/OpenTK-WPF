namespace OpenGLviaWinformsHost
{
    using System;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Threading;

    using OpenGLviaFramebuffer;

    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    using Size = System.Drawing.Size;

    public partial class MainWindow : Window
    {
        #region Fields

        private int frames;

        private GLControl glcontrol;

        private DateTime lastMeasureTime;

        private Renderer renderer;

        #endregion

        #region Constructors and Destructors

        public MainWindow()
        {
            this.InitializeComponent();

            this.renderer = new Renderer(new Size(400, 400));

            this.lastMeasureTime = DateTime.Now;
            this.frames = 0;

            this.glcontrol = new GLControl();
            this.glcontrol.Paint += this.GlcontrolOnPaint;
            this.glcontrol.Dock = DockStyle.Fill;
            this.Host.Child = this.glcontrol;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += this.TimerOnTick;
            timer.Start();
        }

        #endregion

        #region Methods

        private void GlcontrolOnPaint(object sender, PaintEventArgs e)
        {
            this.glcontrol.MakeCurrent();

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            float halfWidth = (float)(this.glcontrol.Width / 2);
            float halfHeight = (float)(this.glcontrol.Height / 2);
            GL.Ortho(-halfWidth, halfWidth, halfHeight, -halfHeight, 1000, -1000);
            GL.Viewport(this.glcontrol.Size);

            this.renderer.Render();

            GL.Finish();

            this.glcontrol.SwapBuffers();

            this.frames++;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(this.lastMeasureTime) > TimeSpan.FromSeconds(1))
            {
                this.Title = this.frames + "fps";
                this.frames = 0;
                this.lastMeasureTime = DateTime.Now;
            }

            this.glcontrol.Invalidate();
        }

        #endregion
    }
}