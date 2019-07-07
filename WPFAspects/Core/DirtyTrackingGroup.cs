using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Core
{
    public class DirtyTrackingGroup : Model
    {
        internal DirtyTrackingGroup(string name, params string[] propertyNames)
        {
            Name = name;
            TracksProperties = new HashSet<string>(propertyNames);
        }

        public string Name { get; }
        public IReadOnlyCollection<string> TracksProperties { get; }

        private bool _IsDirty;
        public bool IsDirty
        {
            get => CheckIsOnMainThread(_IsDirty);
            internal set => SetPropertyBackingValue(value, ref _IsDirty);
        }
    }
}
