using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Uploads;

namespace GW2Scratch.ArcdpsLogManager.Configuration
{
	public class DpsReportUploadSettingsPage : SettingsPage
	{
		private readonly RadioButtonList domainList;

		public DpsReportUploadSettingsPage()
		{
			Text = "Uploads - dps.report";

			domainList = new RadioButtonList();
			// The binding has to be set before the data store is as it's only used when the radio buttons are created.
			domainList.ItemTextBinding = new DelegateBinding<DpsReportDomain, string>(x => x.Domain);
			domainList.Orientation = Orientation.Vertical;
			domainList.DataStore = DpsReportUploader.AvailableDomains;

			// It is possible that the domain in settings does not exist anymore,
			// in case we can't match it with a current one, we add it as an extra
			// option to properly reflect the current state. If the user chooses
			// a different option after this, it won't be available anymore.
			var currentDomain = DpsReportUploader.AvailableDomains.FirstOrDefault(x => x.Domain == Settings.DpsReportDomain);
			if (currentDomain == null)
			{
				var savedDomain = new DpsReportDomain(Settings.DpsReportDomain, "");
				domainList.DataStore = DpsReportUploader.AvailableDomains.Append(savedDomain).ToList();
				currentDomain = savedDomain;
			}

			// The following does not work, TODO: Report
			// domainList.SelectedValue = currentDomain;

			// Because setting SelectedValue directly does not work,
			// we fall back to this workaround.
			int domainIndex = domainList.DataStore.TakeWhile(element => element != currentDomain).Count();
			domainList.SelectedIndex = domainIndex;

			var domainDescriptionLabel = new Label
			{
				Wrap = WrapMode.Word,
				Height = 50,
				Text = ((DpsReportDomain) domainList.SelectedValue).Description
			};

			domainList.SelectedValueChanged += (sender, args) =>
			{
				domainDescriptionLabel.Text = ((DpsReportDomain) domainList.SelectedValue).Description;
			};

			var userTokenTextBox = new TextBox
			{
				ReadOnly = true,
				Text = "************",
				Enabled = false
			};

			var showUserTokenButton = new Button {Text = "Show user token"};
			showUserTokenButton.Click += (sender, args) =>
			{
				userTokenTextBox.Text = Settings.DpsReportUserToken;
				userTokenTextBox.Enabled = true;
			};

			var layout = new DynamicLayout();
			layout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				layout.BeginGroup("Upload domain", new Padding(5), new Size(5, 5));
				{
					layout.AddRow(domainList);
					layout.AddRow(domainDescriptionLabel);
				}
				layout.EndGroup();
				layout.BeginGroup("User token", new Padding(5), new Size(5, 5));
				{
					layout.BeginVertical();
					{
						layout.AddRow(new Label
						{
							Text = "The user token used for dps.report uploads. Treat is as a password, " +
							       "it can be used to see all previous uploads.",
							Wrap = WrapMode.Word,
							Height = 50
						});
					}
					layout.EndVertical();
					layout.BeginVertical();
					{
						layout.BeginHorizontal();
						{
							layout.Add(userTokenTextBox, true);
							layout.Add(showUserTokenButton, false);
						}
						layout.EndHorizontal();
					}
					layout.EndVertical();
				}
				layout.EndGroup();
			}
			layout.EndVertical();

			Content = layout;
		}

		public override void SaveSettings()
		{
			Settings.DpsReportDomain = ((DpsReportDomain) domainList.SelectedValue).Domain;
		}
	}
}