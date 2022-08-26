using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace ConfigurableOptionAmountLimit; 

[Serializable]
public class UserConfigurationException : Exception {
    public UserConfigurationException() { }
    public UserConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    protected UserConfigurationException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) { }
    public UserConfigurationException(string message) : base(message) { }
}