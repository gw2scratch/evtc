using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using ArcdpsLogManager.Annotations;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using Eto.Forms;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;

namespace ArcdpsLogManager.Controls
{
	public class LogDetailPanel : DynamicLayout
	{
		private class LogValues : INotifyPropertyChanged
		{
			private string encounterName;
			private string result;
			private string encounterTime;
			private string encounterDuration;
			private string parseTimeMilliseconds;
			private LogPlayer[] players;
			private string parsingStatus;

			public string EncounterName
			{
				get => encounterName;
				set
				{
					if (value == encounterName) return;
					encounterName = value;
					OnPropertyChanged();
				}
			}

			public string Result
			{
				get => result;
				set
				{
					if (value == result) return;
					result = value;
					OnPropertyChanged();
				}
			}

			public string EncounterTime
			{
				get => encounterTime;
				set
				{
					if (value == encounterTime) return;
					encounterTime = value;
					OnPropertyChanged();
				}
			}

			public string EncounterDuration
			{
				get => encounterDuration;
				set
				{
					if (value == encounterDuration) return;
					encounterDuration = value;
					OnPropertyChanged();
				}
			}

			public string ParseTimeMilliseconds
			{
				get => parseTimeMilliseconds;
				set
				{
					if (value == parseTimeMilliseconds) return;
					parseTimeMilliseconds = value;
					OnPropertyChanged();
				}
			}

			public string ParsingStatus
			{
				get => parsingStatus;
				set
				{
					if (value == parsingStatus) return;
					parsingStatus = value;
					OnPropertyChanged();
				}
			}

			public IEnumerable<LogPlayer> Players
			{
				get => players;
				set
				{
					if (Equals(value, players)) return;
					players = value as LogPlayer[] ?? value.ToArray();
					OnPropertyChanged();
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			[NotifyPropertyChangedInvocator]
			protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public ImageProvider ImageProvider { get; }

		private LogData logData;
		private LogValues Model { get; } = new LogValues();

		public LogData LogData
		{
			get => logData;
			set
			{
				SuspendLayout();
				logData = value;

				Model.EncounterName = logData.EncounterName;

				string result;
				switch (logData.EncounterResult)
				{
					case EncounterResult.Success:
						result = "Success";
						break;
					case EncounterResult.Failure:
						result = "Failure";
						break;
					case EncounterResult.Unknown:
						result = "Unknown";
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				Model.EncounterTime = logData.EncounterStartTime.ToLocalTime().DateTime
					.ToString(CultureInfo.CurrentCulture);
				var seconds = logData.EncounterDuration.TotalSeconds;
				Model.EncounterDuration = $"{seconds / 60:0}m {seconds % 60:0.0}s";

				Model.Result = $"{result} in {Model.EncounterDuration}";

				Model.ParseTimeMilliseconds = $"{logData.ParseMilliseconds} ms";
				Model.ParsingStatus = logData.ParsingStatus.ToString();

				Model.Players = logData.Players;
				ResumeLayout();
			}
		}


		public LogDetailPanel(ImageProvider imageProvider)
		{
			ImageProvider = imageProvider;

			Padding = new Padding(10);
			Width = 300;

			var nameLabel = new Label()
			{
				Font = Fonts.Sans(16, FontStyle.Bold)
			};
			nameLabel.TextBinding.Bind(Model, x => x.EncounterName);
			var resultLabel = new Label()
			{
				Font = Fonts.Sans(12)
			};
			resultLabel.TextBinding.Bind(Model, x => x.Result);

			var timeLabel = new Label();
			timeLabel.TextBinding.Bind(Model, x => x.EncounterTime);
			var durationLabel = new Label();
			durationLabel.TextBinding.Bind(Model, x => x.EncounterDuration);

			var groupComposition = new GroupCompositionControl(imageProvider);
			groupComposition.Bind(x => x.Players,
				new ObjectBinding<IEnumerable<LogPlayer>>(Model, nameof(Model.Players)));

			var parseTimeLabel = new Label();
			parseTimeLabel.TextBinding.Bind(Model, x => x.ParseTimeMilliseconds);

			var parseStatusLabel = new Label();
			parseStatusLabel.TextBinding.Bind(Model, x => x.ParsingStatus);

			BeginVertical(spacing: new Size(0, 30));

			BeginVertical();
			Add(nameLabel);
			Add(resultLabel);
			EndVertical();
			BeginVertical(spacing: new Size(5, 5));
			AddRow("Encounter start", timeLabel);
			AddRow("Encounter duration", durationLabel);
			AddRow();
			EndVertical();
			BeginVertical();
			BeginHorizontal(true);
			Add(groupComposition);
			EndHorizontal();

			var debugSection = BeginVertical(spacing: new Size(5, 5));
			AddRow("Time spent parsing", parseTimeLabel);
			AddRow("Parsing status", parseStatusLabel);
			EndVertical();

			BeginVertical();
			BeginHorizontal();
			Add(new Button {Text = "Upload to dps.report (EI)", Enabled = false}); // TODO: Implement
			Add(new Button {Text = "Upload to gw2raidar", Enabled = false}); // TODO: Implement
			EndHorizontal();
			EndVertical();

			EndVertical();

			debugSection.Visible = Settings.ShowDebugData;
			Settings.ShowDebugDataChanged += (sender, args) => { debugSection.Visible = Settings.ShowDebugData; };
		}
	}
}