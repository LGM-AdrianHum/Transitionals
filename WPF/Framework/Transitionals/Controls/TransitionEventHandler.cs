namespace Transitionals.Controls
{
	/// <summary>
	/// The signature for a handler of events involving transitions.
	/// </summary>
	/// <param name="sender">
	/// The sender of the event.
	/// </param>
	/// <param name="e">
	/// A <see cref="TransitionEventArgs"/> containing the event data.
	/// </param>
	public delegate void TransitionEventHandler(object sender, TransitionEventArgs e);
}