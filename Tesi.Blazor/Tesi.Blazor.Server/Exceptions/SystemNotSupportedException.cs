namespace Tesi.Blazor.Server.Exceptions;

public class SystemNotSupportedException(string message) : Exception(message);

public class NoLicenseFoundException(string message) : Exception(message);