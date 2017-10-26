﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Files.Operations
{
	/// <summary>
	///     Update an existing file with delta patches
	/// </summary>
	public class DeltaPatchOperation : UpdateFileOperation
	{
		/// <summary>
		///     The delta patches that must be applied to the source file to reach the <see cref="UpdateFileOperation.Target" />.
		///     They must be applied in chronological order.
		/// </summary>
		[JsonProperty(PropertyName = "patches")]
		public List<DeltaPatchInfo> Patches { get; set; }
	}

	/// <summary>
	///     Information about a delta patch
	/// </summary>
	public class DeltaPatchInfo
	{
		/// <summary>
		///     The id of the delta patch.
		/// </summary>
		[JsonProperty(PropertyName = "patchId")]
		public int PatchId { get; set; }

		/// <summary>
		///     The length of the delta patch in bytes.
		/// </summary>
		[JsonProperty(PropertyName = "length")]
		public int Length { get; set; }
	}
}