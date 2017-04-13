/*
 * Copyright 2017 Jack owen Shelton
 * Licensed under the terms of the MIT license.
 * Part of the WPFAspects project. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Validation
{
    /// <summary>
    /// Base class for all object validators.  Implements just the common validation properties, events, and methods.
    /// </summary>
    /// <remarks>
    /// This class should not be inherited from in client code.  Instead use the generic subclass of this class.
    /// </remarks>
    public abstract class Validator
    {
        ///Get whether or not the validated object has any validation errors.
        public bool HasErrors { get; protected set; }
    }

    /// <summary>
    /// Base class for object validators.  This is what you want to subclass in client code.
    /// </summary>
    /// <typeparam name="T">The type this validator validates.</typeparam>
    public abstract class Validator<T> : Validator
    {

    }
}
