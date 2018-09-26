namespace ScratchEVTCParser.Model.Encounters
{
	public class ConstantEncounterNameProvider : IEncounterNameProvider
	{
		private readonly string encounterName;

		public ConstantEncounterNameProvider(string encounterName)
		{
			this.encounterName = encounterName;
		}

		public string GetEncounterName()
		{
			return encounterName;
		}
	}
}