namespace WPFAspects.Core;

public class DirtyTrackingGroup : Model
{
	internal DirtyTrackingGroup(string name, params string[] propertyNames)
	{
		if (propertyNames is null || propertyNames.Length == 0)
			throw new ArgumentNullException(nameof(propertyNames));

		Name = name;
		TracksProperties = propertyNames.ToHashSet();
	}

	public string Name { get; }
	public IReadOnlyCollection<string> TracksProperties { get; }

	private bool m_isDirty;
	public bool IsDirty
	{
		get => CheckIsOnMainThread(m_isDirty);
		internal set => SetPropertyBackingValue(value, ref m_isDirty);
	}
}
