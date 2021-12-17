using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.EVTCAnalytics.Model;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Controls;

public class MistlockInstabilityList : DynamicLayout
{
	private readonly ImageProvider imageProvider;

	private List<MistlockInstability> mistlockInstabilities;

	public IEnumerable<MistlockInstability> MistlockInstabilities
	{
		get => mistlockInstabilities;
		set
		{
			mistlockInstabilities = value?.ToList();
			RecreateLayout();
		}
	}

	public MistlockInstabilityList(ImageProvider imageProvider)
	{
		this.imageProvider = imageProvider;
	}


	private void RecreateLayout()
	{
		if (mistlockInstabilities == null)
		{
			SuspendLayout();
			Clear();
			Create();
			ResumeLayout();
			return;
		}

		Spacing = new Size(5, 0);
		Padding = new Padding(5, 0);
		SuspendLayout();
		Clear();
		BeginHorizontal();
		{
			foreach (var instability in mistlockInstabilities.OrderBy(GameNames.GetInstabilityName))
			{
				Add(new ImageView
				{
					Image = imageProvider.GetMistlockInstabilityIcon(instability),
					Size = new Size(24, 24),
					ToolTip = GameNames.GetInstabilityName(instability),
				});
			}

			Add(null, true);
		}
		EndHorizontal();
		Create();
		ResumeLayout();
	}
}