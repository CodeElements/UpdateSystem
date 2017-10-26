using System;
using System.Reflection;

namespace CodeElements.UpdateSystem.Client
{
	/// <summary>
	///     Retrieve the version from the core assembly. The priority order is <see cref="UpdateSystemVersionAttribute" /> >
	///     <see cref="AssemblyFileVersionAttribute" /> > <see cref="AssemblyVersionAttribute" />
	/// </summary>
	public class AssemblyVersionProvider : IVersionProvider
	{
		private readonly Assembly _assembly;

		/// <summary>
		///     Initialize a new instance of <see cref="AssemblyVersionProvider" />
		/// </summary>
		/// <param name="assembly">The core assembly which provides the version</param>
		public AssemblyVersionProvider(Assembly assembly)
		{
			_assembly = assembly;
		}

		/// <summary>
		///     Initialize a new instance of <see cref="AssemblyVersionProvider" /> with the entry assembly as version providing
		///     assembly
		/// </summary>
		public AssemblyVersionProvider()
		{
			_assembly = Assembly.GetEntryAssembly();
		}

		/// <summary>
		///     Get the current version from the given assembly
		/// </summary>
		/// <returns>Return the current assembly version</returns>
		public SemVersion GetVersion()
		{
			var version = _assembly.GetCustomAttribute<UpdateSystemVersionAttribute>()?.Version;
			if (version != null)
				return version;

			var fileVersionAttribute = _assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
			if (fileVersionAttribute != null)
				return (SemVersion) new Version(fileVersionAttribute.Version);

			return (SemVersion) _assembly.GetName().Version;
		}

		/// <summary>
		///     Get a string representing the current <see cref="AssemblyVersionProvider" />
		/// </summary>
		/// <returns>Return a string</returns>
		public override string ToString()
		{
			return "ApplicationVersion: " + GetVersion();
		}
	}
}