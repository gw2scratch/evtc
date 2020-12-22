namespace GW2Scratch.EVTCAnalytics.Processing.Steps
{
	/// <summary>
	/// Represents a post-processing step that occurs at the end of processing by the <see cref="LogProcessor"/>.
	/// </summary>
	public interface IPostProcessingStep
	{
		/// <summary>
		/// Applies changes to the log processor context.
		/// </summary>
		/// <param name="context">The log processor context.</param>
		void Process(LogProcessorContext context);
	}
}