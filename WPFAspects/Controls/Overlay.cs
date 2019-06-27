﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFAspects.Controls
{
	/// <summary>
	/// Class for easily adorning a control.
	/// Code from https://www.nickdarnell.com/wpf-overlay-control/.
	/// </summary>
	/// <example>
	/// <aspects:AdornerOverlay>
	///		<ListBox />
	/// 	<aspects:AdornerOverlay.OverlayContent>
	///			<TextBlock VerticalAlignment = "Center" HorizontalAlignment="Center">Loading!</TextBlock>
	///		</aspects:AdornerOverlay.OverlayContent>
	/// </aspects:AdornerOverlay>
	/// </example>
	[TemplatePart(Name = "PART_OverlayAdorner", Type = typeof(AdornerDecorator))]
	public class AdornerOverlay : ContentControl
	{
		public static readonly DependencyProperty OverlayContentProperty =
			DependencyProperty.Register("OverlayContent", typeof(UIElement), typeof(AdornerOverlay),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOverlayContentChanged)));

		public static readonly DependencyProperty IsOverlayContentVisibleProperty =
			DependencyProperty.Register("IsOverlayContentVisible", typeof(bool), typeof(AdornerOverlay),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsOverlayContentVisibleChanged)));

		private UIElementAdorner m_adorner;

		static AdornerOverlay()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AdornerOverlay), new FrameworkPropertyMetadata(typeof(AdornerOverlay)));
		}

		[Category("Overlay")]
		public UIElement OverlayContent
		{
			get { return (UIElement)GetValue(OverlayContentProperty); }
			set { SetValue(OverlayContentProperty, value); }
		}

		[Category("Overlay")]
		public bool IsOverlayContentVisible
		{
			get { return (bool)GetValue(IsOverlayContentVisibleProperty); }
			set { SetValue(IsOverlayContentVisibleProperty, value); }
		}

		private static void OnOverlayContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AdornerOverlay overlay = d as AdornerOverlay;
			if (overlay != null)
			{
				if (overlay.IsOverlayContentVisible)
				{
					overlay.RemoveOverlayContent();
					overlay.AddOverlayContent();
				}
			}
		}

		private static void OnIsOverlayContentVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AdornerOverlay overlay = d as AdornerOverlay;
			if (overlay != null)
			{
				if ((bool)e.NewValue)
				{
					overlay.AddOverlayContent();
				}
				else
				{
					overlay.RemoveOverlayContent();
				}
			}
		}

		private void AddOverlayContent()
		{
			if (OverlayContent != null)
			{
				m_adorner = new UIElementAdorner(this, OverlayContent);
				m_adorner.Add();

				AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(this);
				parentAdorner.Add(m_adorner);
			}
		}

		private void RemoveOverlayContent()
		{
			if (m_adorner != null)
			{
				AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(this);
				parentAdorner.Remove(m_adorner);

				m_adorner.Remove();
				m_adorner = null;
			}
		}

		#region Class UIElementAdorner

		private class UIElementAdorner : Adorner
		{
			private List<UIElement> m_logicalChildren;
			private UIElement m_element;

			public UIElementAdorner(UIElement adornedElement, UIElement element)
				: base(adornedElement)
			{
				m_element = element;
			}

			public void Add()
			{
				base.AddLogicalChild(m_element);
				base.AddVisualChild(m_element);
			}

			public void Remove()
			{
				base.RemoveLogicalChild(m_element);
				base.RemoveVisualChild(m_element);
			}

			protected override Size MeasureOverride(Size constraint)
			{
				m_element.Measure(constraint);
				return m_element.DesiredSize;
			}

			protected override Size ArrangeOverride(Size finalSize)
			{
				Point location = new Point(0, 0);
				Rect rect = new Rect(location, finalSize);
				m_element.Arrange(rect);
				return finalSize;
			}

			protected override int VisualChildrenCount
			{
				get { return 1; }
			}

			protected override Visual GetVisualChild(int index)
			{
				if (index != 0)
					throw new ArgumentOutOfRangeException("index");

				return m_element;
			}

			protected override IEnumerator LogicalChildren
			{
				get
				{
					if (m_logicalChildren == null)
					{
						m_logicalChildren = new List<UIElement>();
						m_logicalChildren.Add(m_element);
					}

					return m_logicalChildren.GetEnumerator();
				}
			}
		}

		#endregion
	}
}
