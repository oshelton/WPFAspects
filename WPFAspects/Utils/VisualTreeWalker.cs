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
        /// <param name="recursive">Whether or not the visual tree should be searched recursively.  Defaults to true.</param>
        /// <param name="depthLimit">How deep the search should go. Null results in an unbounded search.</param>
        /// <remarks>
        ///     The depthLimit parameter can be used to make the search end prematurely if a child is not found at the expected depth.
        ///     If recursive is false depthLimit does nothing.
        /// </remarks>
        /// <returns>The child found or null.</returns>
        public static T FindFirstChildOfType<T>(this DependencyObject parent, bool recursive = true, int? depthLimit = null) where T: DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as DependencyObject;

                if (child is T)
                    return child as T;

                if (recursive && (depthLimit == null || depthLimit > 0))
                {
                    var searchChild = child.FindFirstChildOfType<T>(true, depthLimit == null ? null : depthLimit - 1);
                    if (searchChild != null)
                        return searchChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Find all children of a specific type of a passed in object.
        /// </summary>
        /// <param name="recursive">Whether or not the visual tree should be searched recursively.  Defaults to true.</param>
        /// <param name="depthLimit">How deep the search should go. Null results in an unbounded search.</param>
        /// <remarks>
        ///     The depthLimit parameter can be used to make the search end prematurely if a child is not found at the expected depth.
        ///     If recursive is false depthLimit does nothing.
        /// </remarks>
        /// <returns>A collection containing any children found of the appropriate type, or an empty collection.</returns>
        public static IReadOnlyList<T> FindChildrenOfType<T>(this DependencyObject parent, bool recursive = true, int? depthLimit = null) where T: DependencyObject
        {
            List<T> children = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as DependencyObject;

                if (child is T)
                    children.Add(child as T);

                if (recursive && (depthLimit == null || depthLimit > 0))
                    children.AddRange(child.FindChildrenOfType<T>(true, depthLimit == null ? null : depthLimit - 1));
            }

            return children;
        }

        /// <summary>
        /// Find the ancestor of the object with the specified type.
        /// </summary>
        /// <param name="heightLimit">How high the search should go. Null results in an unbounded search.</param>
        /// <remarks>
        ///     The heightLimit parameter can be used to make the search end prematurely if a parent is not found at the expected height.
        /// </remarks>
        /// <returns>The found ancestor or null.</returns>
        public static T FindAncestorOfType<T>(this DependencyObject child, int? heightLimit = null) where T: DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child) as DependencyObject;

            if (parent is T)
                return parent as T;
            else if (parent != null && (heightLimit == null || heightLimit > 0))
                return parent.FindAncestorOfType<T>(heightLimit == null ? null : heightLimit - 1);
            else
                return null;
        }
    }
}
