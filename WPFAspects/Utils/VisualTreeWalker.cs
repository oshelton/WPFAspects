using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPFAspects.Utils
{
	public static class VisualTreeWalker
	{
		/// <summary>
		/// Find and return the first child visual of a specific type.
		/// </summary>
		/// <param name="parent">Element at which to begin the search.</param>
		/// <param name="recursive">Whether or not the visual tree should be searched recursively.  Defaults to true.</param>
		/// <param name="depthLimit">How deep the search should go. Null results in an unbounded search.</param>
		/// <remarks>
		///     The depthLimit parameter can be used to make the search end prematurely if a child is not found at the expected depth.
		///     If recursive is false depthLimit does nothing.
		/// </remarks>
		/// <returns>The child found or null.</returns>
		public static TChild FindFirstChildOfType<TChild>(DependencyObject parent, bool recursive = true, uint? depthLimit = null)
			where TChild : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
			{
				var child = VisualTreeHelper.GetChild(parent, i) as DependencyObject;

				if (child is TChild)
				{
					return child as TChild;
				}
				else if (recursive && (depthLimit == null || depthLimit > 0))
				{
					var searchChild = FindFirstChildOfType<TChild>(child, true, depthLimit == null ? null : depthLimit - 1);
					if (searchChild != null)
						return searchChild;
				}
			}

			return null;
		}

		/// <summary>
		/// Find all children of a specific type of a passed in object.
		/// </summary>
		/// <param name="parent">Element at which to begin the search.</param>
		/// <param name="recursive">Whether or not the visual tree should be searched recursively.  Defaults to true.</param>
		/// <param name="depthLimit">How deep the search should go. Null results in an unbounded search.</param>
		/// <remarks>
		///     The depthLimit parameter can be used to make the search end prematurely if a child is not found at the expected depth.
		///     If recursive is false depthLimit does nothing.
		/// </remarks>
		/// <returns>A collection containing any children found of the appropriate type, or an empty collection.</returns>
		public static IReadOnlyList<TChild> FindChildrenOfType<TChild>(DependencyObject parent, bool recursive = true, uint? depthLimit = null)
			where TChild : DependencyObject
		{
			var children = new List<TChild>();
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
			{
				var child = VisualTreeHelper.GetChild(parent, i) as DependencyObject;

				if (child is TChild)
					children.Add(child as TChild);
				else if (recursive && (depthLimit == null || depthLimit > 0))
					children.AddRange(FindChildrenOfType<TChild>(child, true, depthLimit == null ? null : depthLimit - 1));
			}

			return children;
		}

		/// <summary>
		/// Find the visual ancestor of the object with the specified type.
		/// </summary>
		/// <param name="child">Element at which to begin the search.</param>
		/// <param name="heightLimit">How high the search should go. Null results in an unbounded search.</param>
		/// <remarks>
		///     The heightLimit parameter can be used to make the search end prematurely if a parent is not found at the expected height.
		/// </remarks>
		/// <returns>The found ancestor or null.</returns>
		public static TAncestor FindFirstAncestorOfType<TAncestor>(DependencyObject child, uint? heightLimit = null)
			where TAncestor : DependencyObject
		{
			var parent = VisualTreeHelper.GetParent(child) as DependencyObject;

			if (parent is TAncestor)
				return parent as TAncestor;
			else if (parent != null && (heightLimit == null || heightLimit > 0))
				return FindFirstAncestorOfType<TAncestor>(child, heightLimit == null ? null : heightLimit - 1);
			else
				return null;
		}
	}
}
