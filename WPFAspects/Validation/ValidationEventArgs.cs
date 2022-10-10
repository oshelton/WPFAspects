using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Validation;
public sealed class ValidationEventArgs : EventArgs
{
	public ValidationEventArgs(Core.Model validatedModel)
	{
		ValidatedModel = validatedModel;
	}

	public Core.Model ValidatedModel { get; init; }
}
