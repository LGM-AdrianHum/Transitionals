namespace Transitionals
{
	/// <summary>
	///     Describes how null content is supported.
	/// </summary>
	public enum NullContentSupport
	{
		/// <summary>
		///     Transitioning to or from null is not supported.
		/// </summary>
		None,

		/// <summary>
		///     Transitioning from null to non-null is supported.
		/// </summary>
		Old,

		/// <summary>
		///     Transitioning from non-null to null is supported.
		/// </summary>
		New,

		/// <summary>
		///     Transitioning to or from null are both supported.
		/// </summary>
		Both
	}
}