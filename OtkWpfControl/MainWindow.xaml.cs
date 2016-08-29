using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace OtkWpfControl
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OtkWpfControl_Initialized( object sender , EventArgs e )
		{
			mScene.Initialize();

			// for example, get shader error then notify the error by MessageBox
			//var stat = mScene.Initialize();
			//if ( stat.Error )
			//{
			//	// まだウインドウが表示されていないからこうしないと例外が出てしまう
			//  // Sinse the window is not created yet, exceltion will be thrown
			//	// http://stackoverflow.com/questions/7442943/dispatcher-throws-invalidoperationexception-on-messagebox-show-in-textchanged-ev
			//	// https://social.msdn.microsoft.com/Forums/vstudio/en-US/44962927-006e-4629-9aa3-100357861442/dispatcher-processing-has-been-suspended-invalidoperationexception-when-calling-showdialog?forum=wpf
			//	Application.Current.Dispatcher.BeginInvoke
			//	(
			//		System.Windows.Threading.DispatcherPriority.Normal ,
			//		(Action)(() =>
			//		{
			//			MessageBox.Show( stat.ErrorMessage , stat.ErrorComponent );
			//		})
			//	);

			//	Close();
			//}
		}

		private void OtkWpfControl_Resized( object sender , EventArgs e )
		{
			var ctrl = sender as OpenTK.WPF.OtkWpfControl;
			if ( ctrl != null )
			{
				mScene.Resize( ctrl.ActualWidth , ctrl.ActualHeight );
			}
		}

		private void OtkWpfControl_OpenGLDraw( object sender , OpenTK.WPF.OtkWpfControl.OpenGLDrawEventArgs e )
		{
			var ctrl = sender as OpenTK.WPF.OtkWpfControl;
			if ( ctrl != null )
			{
				// RederingTime is actual time from CompositionTarget.Rendering has started, not the interval
				e.Redrawn = mScene.Render( e.RenderingTime );
			}
		}

		private readonly Scene mScene = new Scene();
	}
}
