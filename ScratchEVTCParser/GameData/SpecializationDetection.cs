using ScratchEVTCParser.GameData.Detections;
using ScratchEVTCParser.Model;

namespace ScratchEVTCParser.GameData
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