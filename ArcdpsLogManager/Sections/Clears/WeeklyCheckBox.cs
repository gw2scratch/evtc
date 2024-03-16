using Eto.Drawing;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class WeeklyCheckBox : StackLayout
{
	private readonly ImageView imageView;
	private readonly Label label;

	public string Text
	{
		get => label.Text;
		set => label.Text = value;
	}

	public Image Image
	{
		get => imageView.Image;
		set => imageView.Image = value;
	}

	public WeeklyCheckBox()
	{
		imageView = new ImageView { Size = new Size(16, 16) };
		label = new Label();
		Orientation = Orientation.Horizontal;
		Spacing = 6;
		Items.Add(imageView);
		Items.Add(label);
	}
}