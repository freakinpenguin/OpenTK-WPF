using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OtkWpfControl
{
    /// <summary>
    /// The renderer.
    /// </summary>
    public class Scene
    {
		/// <summary>
		/// initialize the scene
		/// shaders, buffers, basic OpenGL settings
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// re-initialize size related staffs
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void Resize( double width , double height )
        {
            this.size = new Size( (int)width , (int)height );

			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();
			float halfWidth = (float)(this.size.Width / 2);
			float halfHeight = (float)(this.size.Height / 2);
			GL.Ortho( -halfWidth , halfWidth , -halfHeight , halfHeight , 1000 , -1000 );
			GL.Viewport( 0 , 0 , (int)this.size.Width , (int)this.size.Height );
		}

		/// <summary>
		/// render a scene , same as OpenGLviaFramebuffer
		/// it's better to make animation depending on the time, not the interval nor assuming frame rate
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public bool Render( TimeSpan time )
        {
			if ( this.displayList <= 0 )
			{
				this.displayList = GL.GenLists( 1 );
				GL.NewList( this.displayList , ListMode.Compile );

				GL.Color3( Color.Red );

				GL.Begin( PrimitiveType.Points );

				Random rnd = new Random();
				for ( int i = 0; i < 1000000; i++ )
				{
					float factor = 0.2f;
					Vector3 position = new Vector3(
						rnd.Next(-1000, 1000) * factor,
						rnd.Next(-1000, 1000) * factor,
						rnd.Next(-1000, 1000) * factor);
					GL.Vertex3( position );

					position.Normalize();
					GL.Normal3( position );
				}

				GL.End();

				GL.EndList();
			}

			GL.Enable( EnableCap.Lighting );
			GL.Enable( EnableCap.Light0 );
			GL.Enable( EnableCap.Blend );
			GL.BlendFunc( BlendingFactorSrc.SrcAlpha , BlendingFactorDest.OneMinusSrcAlpha );
			GL.Enable( EnableCap.DepthTest );

			GL.ClearColor( Color.FromArgb( 200 , Color.LightBlue ) );
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

			float halfWidth = this.size.Width * 0.5f;
			this.angle += 1f;
			GL.Rotate( this.angle , Vector3.UnitZ );
			GL.Rotate( this.angle , Vector3.UnitY );
			GL.Rotate( this.angle , Vector3.UnitX );
			GL.Translate( 0.5f , 0 , 0 );

			GL.CallList( this.displayList );

			GL.MatrixMode( MatrixMode.Modelview );
			GL.LoadIdentity();

			GL.Begin( PrimitiveType.Triangles );

			GL.Color4( Color.Green );
			GL.Vertex3( 0 , 300 , 0 );  // top of the triangle
			GL.Vertex3( 0 , 0 , 0 );  // left-bottom of the triangle
			GL.Vertex3( 300 , 0 , 0 );  // right-bottom of the triangle

			GL.End();

			return true; // continue animation
		}

        #region Fields

        private float angle;

        private int displayList;

        private Size size;

        #endregion
	}
}
