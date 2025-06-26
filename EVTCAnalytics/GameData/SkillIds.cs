namespace GW2Scratch.EVTCAnalytics.GameData
{
	/// <summary>
	/// Provides ids of common skills.
	/// </summary>
	public static class SkillIds
	{
		/* Boons and Conditions */
		public const int Protection = 717;
		public const int Regeneration = 718;
		public const int Swiftness = 719;
		public const int Blinded = 720;
		public const int Crippled = 721;
		public const int Chilled = 722;
		public const int Poisoned = 723;
		public const int Fury = 725;
		public const int Vigor = 726;
		public const int Immobile = 727;
		public const int Bleeding = 736;
		public const int Burning = 737;
		public const int Vulnerability = 738;
		public const int Might = 740;
		public const int Weakness = 742;
		public const int Aegis = 743;
		public const int Fear = 791;
		public const int Confusion = 861;
		public const int Retaliation = 873;
		public const int Stability = 1122;
		public const int Quickness = 1187;
		public const int Torment = 19426;
		public const int Alacrity = 30328;

		/* Buffs */
		public const int Invulnerability = 757;
		public const int Determined = 762;
		public const int Determined895 = 895;
		public const int GorsevalInvulnerability = 31790;
		public const int QadimFlameArmor = 52568;
		public const int SoullessHorrorDetermined = 895;

		public const int Superspeed = 5974;

		/* Buffs - Mesmer */
		public const int TimeAnchored = 30136;
		public const int FencersFinesse = 30426;
		public const int IllusionaryDefense = 49099;
		public const int HealingPrism = 29997;
		public const int SignetOfTheEtherPassive = 21751;
		public const int SignetOfDominationPassive = 10231;
		public const int SignetOfIllusionsPassive = 10246;
		public const int SignetOfInspirationPassive = 10235;
		public const int SignetOfMidnightPassive = 10233;
		public const int SignetOfHumilityPassive = 30739;

		/* Buffs - Thief */
		public const int SignetOfMalicePassive = 13049;
		public const int SignetOfAgilityPassive = 13061;
		public const int AssassinsSignetPassive = 13047;
		public const int InfiltratorsSignetPassive = 13063;
		public const int SignetOfShadowsPassive = 13059;

		/* Skills */
		public const int Revive = 1066;
		/// <summary>
		/// This is not a real skill.
		/// arcdps, however, encodes dodging as a skill.
		/// </summary>
		public const int ArcdpsDodge = 65001;

		/* Skills - Mesmer */
		public const int LesserChaosStorm = 13733;
		public const int MindWrackAmmo = 49068;
		public const int MirageMirror = 44677;
		public const int SignetOfTheEther = 21750;
		public const int SignetOfDomination = 10232;
		public const int SignetOfIllusions = 10247;
		public const int SignetOfInspiration = 10236;
		public const int SignetOfMidnight = 10234;
		public const int SignetOfHumility = 29519;

		/* Skills - Thief */
		public const int SignetOfMalice = 13050;
		public const int SignetOfAgility = 13062;
		public const int AssassinsSignet = 13046;
		public const int InfiltratorsSignet = 13064;
		public const int SignetOfShadows = 13060;
		public const int PalmStrike = 30693;

		/* Raids - Wing 4 */
		// Cairn CM - counts down and triggers the special action skill as it reaches 0
		public const int CairnCountdown = 38098;

		/* Raids - Wing 5 */
		// Soulless Horror - Stacking debuff that increases incoming damage, applied to fixated players
		public const int Necrosis = 47414;

		/* Fractals */
		// Mistlock Instabilities
		public const int Afflicted = 22228;
		public const int NoPainNoGain = 22277;
		public const int LastLaugh = 22293;
		public const int SocialAwkwardness = 32942;
		public const int ToxicTrail = 36204;
		public const int AdrenalineRush = 36341;
		public const int FluxBomb = 36386;
		public const int Vengeance = 46865;
		public const int ToxicSickness = 47288;
		public const int Hamstrung = 47323;
		public const int FractalVindicators = 48296;
		public const int BoonOverload = 53673;
		public const int StickTogether = 53932;
		public const int Outflanked = 54084;
		public const int SugarRush = 54237;
		public const int Frailty = 54477;
		public const int WeBleedFire = 54719;
		public const int SlipperySlope = 54817;
		public const int Birds = 54131;
		public const int MistsConvergence = 36224;
		
		// Ai, the Keeper of the Peak, a skill without a name that she casts early into the dark phase.
		public const int AiDarkEarlySkill = 61356;

		public const int HarvestTempleLiftOff = 63896;

		
		// The first attack of Cardinal Adina
		public const int AdinaChargeUp = 56648;
		// The first autoattack of Cardinal Sabir
		public const int SabirFirstAutoattack = 56620;

		public const int Emboldened = 68087;

		public const int LifeFireCircleCM = 76339;
	}
}