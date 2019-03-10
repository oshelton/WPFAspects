using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFAspects.Utils
{
    public static class LogicalTreeWalker
    {
        /// <summary>
        /// Find a child logical element with the given name.
        /// </summary>
        /// <param name="childName">The name of the child element to search for.</param>
        /// <returns>The found element, or null if one is not found.</returns>
        public static T FindChild<T>(FrameworkElement parent, string childName) where T:FrameworkElement
        {
            return LogicalTreeHelper.FindLogicalNode(parent, childName) as T;
        }

        /// <summary>
        /// Find a parent logical element with the given name.
        /// </summary>
        /// <param name="childName">Name of the parent to search for.</param>
        /// <remarks>This method is subject to WPF's name scopes https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/wpf-xaml-namescopes?view=netframework-4.7.2.</remarks>
        /// <returns>Parent with the given name.</returns>
        public static T FindAncestor<T>(FrameworkElement child, string childName) where T:FrameworkElement
        {
            return child.FindName(childName) as T;
        }

        /// <summary>
        /// Find the first child logical element of a given type.
        /// </summary>
        /// <param name="recursive">Whether or not the logical tree should be searched recursively.  Defaults to true.</param>
        /// <param name="depthLimit">How deep the search should go. Null results in an unbounded search.</param>
        /// <remarks>
        ///     The depthLimit parameter can be used to make the search end prematurely if a child is not found at the expected depth.
        ///     If recursive is false depthLimit does nothing.
        /// </remarks>
        /// <returns>The child found or null.</returns>
        public static T FindFirstChildOfType<T>(FrameworkElement parent, bool recursive = true, uint? depthLimit = null) where T:FrameworkElement
        {
            foreach (var child in LogicalTreeHelper.GetChildren(parent).Cast<FrameworkElement>())
            {
                if (child is T)
                    return child as T;
                else if (recursive && (depthLimit == null || depthLimit > 0))
                {
                    var searchChild = FindFirstChildOfType<T>(child, true, depthLimit == null ? null : depthLimit - 1);
                    if (searchChild != null)
                        return searchChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Find all logical children of a given type.
        /// </summary>
        /// <param name="recursive">Whether or not the logical tree should be searched recursively.  Defaults to true.</param>
        /// <param name="depthLimit">How deep the search should go. Null results in an unbounded search.</param>
        /// <remarks>
        ///     The depthLimit parameter can be used to make the search end prematurely if a child is not found at the expected depth.
        ///     If recursive is false depthLimit does nothing.
        /// </remarks>
        /// <returns>A collection containing any children found of the appropriate type, or an empty collection.</returns>
        public static IReadOnlyList<T> FindChildrenOfType<T>(FrameworkElement parent, bool recursive = true, uint? depthLimit = null) where T : FrameworkElement
        {
            List<T> children = new List<T>();
            foreach (var child in LogicalTreeHelper.GetChildren(parent).Cast<FrameworkElement>())
            {
                if (child is T)
                    children.Add(child as T);
                else if (recursive && (depthLimit == null || depthLimit > 0))
                    children.AddRange(FindChildrenOfType<T>(child, true, depthLimit == null ? null : depthLimit - 1));
            }

            return children;
        }

        /// <summary>
        /// Find the logical ancestor of the object with the specified type.
        /// </summary>
        /// <param name="heightLimit">How high the search should go. Null results in an unbounded search.</param>
        /// <remarks>
        ///     The heightLimit parameter can be used to make the search end prematurely if a parent is not found at the expected height.
        /// </remarks>
        /// <returns>The found ancestor or null.</returns>
        public static T FindFirstAncestorOfType<T>(FrameworkElement child, uint? heightLimit = null) where T : FrameworkElement
        {
            var parent = LogicalTreeHelper.GetParent(child) as FrameworkElement;

            if (parent is T)
                return parent as T;
            else if (parent != null && (heightLimit == null || heightLimit > 0))
                return FindFirstAncestorOfType<T>(child, heightLimit == null ? null : heightLimit - 1);
            else
                return null;
        }
    }
}
