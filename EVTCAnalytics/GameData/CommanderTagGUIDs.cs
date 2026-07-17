using System;
using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;

namespace GW2Scratch.EVTCAnalytics.GameData
{
	public class CommanderTagGUIDs
	{
		public static readonly Guid RedCommanderTag = new("4242f370-667c-e54e-b3bf-22be8d06f986");
		public static readonly Guid OrangeCommanderTag = new("e57aae9e-e7fc-5d45-8b0c-f16be4b096bf");
		public static readonly Guid YellowCommanderTag = new("af9442a2-90c6-2145-96e0-b339eb3bde92");
		public static readonly Guid GreenCommanderTag = new("74ad480e-531f-4740-a407-879976c8ca91");
		public static readonly Guid CyanCommanderTag = new("96f4ab5c-dec5-2943-8837-5c7a03ab7614");
		public static readonly Guid BlueCommanderTag = new("ae714fc5-e4ea-464c-8961-cd78e86f9291");
		public static readonly Guid PurpleCommanderTag = new("1993fadb-6fb7-0e43-83a2-23a54d311f7d");
		public static readonly Guid PinkCommanderTag = new("e911d8c0-ef2f-df4d-8d25-2e5fb1283c62");
		public static readonly Guid WhiteCommanderTag = new("a59678cd-fb57-3243-9d7f-cbf58d8bcec3");
		public static readonly Guid RedCatmanderTag = new("ca76ab02-3593-b044-8f69-2fe29df03d17");
		public static readonly Guid OrangeCatmanderTag = new("9fdf03e9-ba09-a245-8c1e-dda4d81bc34d");
		public static readonly Guid YellowCatmanderTag = new("6bce90e9-9016-b448-969e-b317784a8334");
		public static readonly Guid GreenCatmanderTag = new("2ca226e0-7262-c743-ba19-3acf6f9d0af6");
		public static readonly Guid CyanCatmanderTag = new("a8072d65-ce35-924b-abba-c831b12019d7");
		public static readonly Guid BlueCatmanderTag = new("9b94f0fd-616e-7f4a-a58e-fdc8c59fb689");
		public static readonly Guid PurpleCatmanderTag = new("7224a4af-710e-4243-bfe0-32629e17ca6e");
		public static readonly Guid PinkCatmanderTag = new("4387be61-46d4-3246-aa7b-333168ea58ea");
		public static readonly Guid WhiteCatmanderTag = new("a0b0ec07-6bc8-3b40-a293-c1cdec4a7de7");

		public static readonly Dictionary<Guid, CommanderTags> Tags = new()
		{
			{ RedCommanderTag, CommanderTags.RedCommanderTag },
			{ OrangeCommanderTag, CommanderTags.OrangeCommanderTag },
			{ YellowCommanderTag, CommanderTags.YellowCommanderTag },
			{ GreenCommanderTag, CommanderTags.GreenCommanderTag },
			{ CyanCommanderTag, CommanderTags.CyanCommanderTag },
			{ BlueCommanderTag, CommanderTags.BlueCommanderTag },
			{ PurpleCommanderTag, CommanderTags.PurpleCommanderTag },
			{ PinkCommanderTag, CommanderTags.PinkCommanderTag },
			{ WhiteCommanderTag, CommanderTags.WhiteCommanderTag },
			{ RedCatmanderTag, CommanderTags.RedCatmanderTag },
			{ OrangeCatmanderTag, CommanderTags.OrangeCatmanderTag },
			{ YellowCatmanderTag, CommanderTags.YellowCatmanderTag },
			{ GreenCatmanderTag, CommanderTags.GreenCatmanderTag },
			{ CyanCatmanderTag, CommanderTags.CyanCatmanderTag },
			{ BlueCatmanderTag, CommanderTags.BlueCatmanderTag },
			{ PurpleCatmanderTag, CommanderTags.PurpleCatmanderTag },
			{ PinkCatmanderTag, CommanderTags.PinkCatmanderTag },
			{ WhiteCatmanderTag, CommanderTags.WhiteCatmanderTag },
		};
	}
}
