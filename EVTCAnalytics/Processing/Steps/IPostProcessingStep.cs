namespace GW2Scratch.EVTCAnalytics.Processing.Steps
{
	public interface IPostProcessingStep
	{
		void Process(LogProcessorContext context);
	}
}