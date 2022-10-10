using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Utils;

/// <summary>
/// Simple class for representing scoped work; ie, work that needs to do something when it is done.
/// </summary>
public sealed class ScopedWork : IDisposable
{
	public ScopedWork(Action workStartsAction, Action workDoneAction)
	{
		if (workStartsAction is null && workDoneAction is null)
			throw new InvalidOperationException("workStartAction and workDoneAction cannot both be null");

		workStartsAction?.Invoke();

		m_workDoneAction = workDoneAction;
	}

	public void Dispose()
	{
		m_workDoneAction?.Invoke();
	}

	private readonly Action m_workDoneAction;
}
