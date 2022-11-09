using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace WPFAspects.Controls;

/// <summary>
/// Class for addressing airspace issue with WPF's WindowsFormsHost.
/// Originally from https://stackoverflow.com/questions/5978917/render-wpf-control-on-top-of-windowsformshost
/// </summary>
public class AirspaceOverlay : Decorator
{
	public AirspaceOverlay()
	{
		m_transparentInputWindow = CreateTransparentWindow();
		m_transparentInputWindow.PreviewMouseDown += TransparentInputWindow_PreviewMouseDown;
	}

	public object OverlayChild
	{
		get => m_transparentInputWindow.Content;
		set => m_transparentInputWindow.Content = value;
	}

	private static Window CreateTransparentWindow()
	{
		var transparentInputWindow = new Window();

		// Make the window itself transparent, with no style.
		transparentInputWindow.Background = Brushes.Transparent;
		transparentInputWindow.AllowsTransparency = true;
		transparentInputWindow.WindowStyle = WindowStyle.None;

		// Hide from taskbar until it becomes a child
		transparentInputWindow.ShowInTaskbar = false;

		// HACK: This window and it's child controls should never have focus, as window styling of an invisible window
		// will confuse user.
		transparentInputWindow.Focusable = false;

		return transparentInputWindow;
	}

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		base.OnRenderSizeChanged(sizeInfo);
		UpdateOverlaySize();
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		base.OnRender(drawingContext);
		if (m_transparentInputWindow.Visibility != Visibility.Visible)
		{
			UpdateOverlaySize();
			m_transparentInputWindow.Show();
			m_parentWindow = GetParentWindow(this);
			m_transparentInputWindow.Owner = m_parentWindow;
			m_parentWindow.LocationChanged += ParentWindow_LocationChanged;
			m_parentWindow.SizeChanged += ParentWindow_SizeChanged;
		}
	}

	private static Window GetParentWindow(DependencyObject o)
	{
		var parent = VisualTreeHelper.GetParent(o);
		if (parent is not null)
			return GetParentWindow(parent);
		if (o is Window window)
			return window;
		if (o is FrameworkElement element && element.Parent != null)
			return GetParentWindow(element.Parent);
		throw new InvalidOperationException("A window parent could not be found for " + o);
	}

	private void TransparentInputWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e) => m_parentWindow.Focus();

	private void ParentWindow_LocationChanged(object sender, EventArgs e) => UpdateOverlaySize();

	private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateOverlaySize();

	private void UpdateOverlaySize()
	{
		var hostTopLeft = PointToScreen(new Point(0, 0));
		m_transparentInputWindow.Left = hostTopLeft.X;
		m_transparentInputWindow.Top = hostTopLeft.Y;
		m_transparentInputWindow.Width = ActualWidth;
		m_transparentInputWindow.Height = ActualHeight;
	}

	private readonly Window m_transparentInputWindow;
	private Window m_parentWindow;
}
