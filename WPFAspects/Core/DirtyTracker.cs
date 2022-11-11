using System.Collections;
using WPFAspects.Utils;

namespace WPFAspects.Core;

/// <summary>
/// Simple class for tracking changes to models.
/// </summary>
/// <remarks>Cannot be used to track collection based properties.</remarks>
public class DirtyTracker : Model, IDisposable
{
	public DirtyTracker(Model toTrack)
	{
		TrackedObject = toTrack ?? throw new ArgumentNullException(nameof(toTrack));
		TrackedProperties = toTrack.DefaultTrackedProperties;

		AddHandlers();
	}

	public void Dispose()
	{
		RemoveHandlers();
	}

	/// <summary>
	/// Reset the object to its initial state.
	/// </summary>
	public void ResetToInitialState()
	{
		RemoveHandlers();
		if (m_initialValues.Count != 0)
		{
			foreach (var pair in m_initialValues)
				TrackedObject.SetPropertyValue(pair.Key, pair.Value);
		}
		IsDirty = false;

		foreach (var group in m_trackingGroups.Values)
			group.IsDirty = false;
		AddHandlers();
	}

	/// <summary>
	/// Reset the passed in property to its initial value.
	/// </summary>
	public void ResetPropertyToInitialSTate(string propertyName)
	{
		if (string.IsNullOrWhiteSpace(propertyName))
			throw new ArgumentNullException(propertyName);

		if (m_initialValues.TryGetValue(propertyName, out object initial))
			TrackedObject.SetPropertyValue(propertyName, initial);
	}

	/// <summary>
	/// Set the current state of the object as the initial state.
	/// </summary>
	public void SetInitialState()
	{
		m_initialValues.Clear();
		m_newValues.Clear();

		foreach (var group in m_trackingGroups.Values)
			group.IsDirty = false;

		IsDirty = false;
	}

	/// <summary>
	/// Get whether or not the passed in property has changed.
	/// </summary>
	public bool IsPropertyDirty(string propertyName)
	{
		return m_newValues.ContainsKey(propertyName);
	}

	/// <summary>
	/// Get the initial value for the property whose name is passed in.
	/// </summary>
	public object GetInitialValueForProperty(string propertyName)
	{
		if (string.IsNullOrWhiteSpace(propertyName))
			throw new ArgumentNullException(propertyName);

		return m_initialValues.TryGetValue(propertyName, out object initial) ? initial : null;
	}

	/// <summary>
	/// Create a new dirty tracking sub group.
	/// </summary>
	/// <param name="groupName">Name of the group.</param>
	/// <param name="propertiesForGroup">Properties to include in the group.</param>
	/// <returns>The tracking group.</returns>
	public DirtyTrackingGroup CreateDirtyTrackingGroup(string groupName, params string[] propertiesForGroup)
	{
		var newGroup = new DirtyTrackingGroup(groupName, propertiesForGroup);
		m_trackingGroups.Add(groupName, newGroup);

		if (propertiesForGroup.Any(p => m_newValues.ContainsKey(p)))
			newGroup.IsDirty = true;

		return newGroup;
	}

	/// <summary>
	/// The object whose dirty state is to be tracked.
	/// </summary>
	public Model TrackedObject { get; }

	/// <summary>
	/// Get/Set the properties changes to should be ignored.
	/// </summary>
	/// <remarks>Defaults to Model.DefaultTrackedProperties.</remarks>
	public HashSet<string> TrackedProperties
	{
		get => m_trackedProperties;
		set
		{
			if (IsDirty)
				throw new InvalidOperationException("Tracked properties cannot be updated when a DirtyDracker is dirty.");
			m_trackedProperties = value ?? throw new ArgumentNullException(nameof(value));
		}
	}

	/// <summary>
	/// Get whether or not the tracked object is dirty (has changes).
	/// </summary>
	public bool IsDirty
	{
		get => CheckIsOnMainThread(m_isDirty);
		private set => SetPropertyBackingValue(value, ref m_isDirty);
	}

	/// <summary>
	/// Add the necessary event handlers.
	/// </summary>
	private void AddHandlers()
	{
		TrackedObject.PropertyChangingFromValue += OnTrackedObjectPropertyChanging;
		TrackedObject.PropertyChangedToValue += OnTrackedObjectPropertyChanged;
	}

	/// <summary>
	/// Remove the event handlers.
	/// </summary>
	private void RemoveHandlers()
	{
		TrackedObject.PropertyChangingFromValue -= OnTrackedObjectPropertyChanging;
		TrackedObject.PropertyChangedToValue -= OnTrackedObjectPropertyChanged;
	}

	private void OnTrackedObjectPropertyChanging(object sender, PropertyChangingWithValueEventArgs args)
	{
		if ((args.PreviousValue is string || args.PreviousValue is not IEnumerable) && !m_initialValues.ContainsKey(args.PropertyName))
			m_initialValues.Add(args.PropertyName, args.PreviousValue);
	}

	private void OnTrackedObjectPropertyChanged(object sender, PropertyChangedWithValueEventArgs args)
	{
		if ((TrackedProperties.Count == 0 || TrackedProperties.Contains(args.PropertyName)) && (args.NewValue is string || args.NewValue is not IEnumerable))
		{
			if (!Equals(m_initialValues[args.PropertyName], args.NewValue))
			{
				m_newValues[args.PropertyName] = args.NewValue;

				foreach (var group in m_trackingGroups.Values)
				{
					if (group.TracksProperties.Any(p => p == args.PropertyName))
						group.IsDirty = true;
				}
			}
			else
			{
				m_initialValues.Remove(args.PropertyName);
				m_newValues.Remove(args.PropertyName);

				foreach (var group in m_trackingGroups.Values)
				{
					if (!group.TracksProperties.Any(p => m_newValues.ContainsKey(p)))
						group.IsDirty = false;
				}
			}

			if (m_newValues.Count != 0)
				IsDirty = true;
			else
				IsDirty = false;
		}
	}

	/// <summary>
	/// Initial property values of the object, keyed by name.
	/// </summary>
	private readonly Dictionary<string, object> m_initialValues = new Dictionary<string, object>();

	/// <summary>
	/// New values for properties of the object, keyed by name.
	/// </summary>
	private readonly Dictionary<string, object> m_newValues = new Dictionary<string, object>();

	/// <summary>
	/// Dictionary for keeping track of property tracking groups.
	/// </summary>
	private readonly Dictionary<string, DirtyTrackingGroup> m_trackingGroups = new Dictionary<string, DirtyTrackingGroup>();

	private bool m_isDirty;
	private HashSet<string> m_trackedProperties;
}
