using GW2Scratch.EVTCAnalytics.GameData.Detections;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.GameData
{
	public class SpecializationDetection
	{
		public IDetection Detection { get; }
		public CoreSpecialization Specialization { get; }

		public SpecializationDetection(IDetection detection, CoreSpecialization specialization)
		{
			Detection = detection;
			Specialization = specialization;
		}
	}
}