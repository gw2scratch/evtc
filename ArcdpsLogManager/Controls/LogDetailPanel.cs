using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
			private IEnumerable<Player> players;

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

			public IEnumerable<Player> Players
			{
				get => players;
				set
				{
					if (Equals(value, players)) return;
					players = value;
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
		private LogValues Values { get; } = new LogValues();

		public LogData LogData
		{
			get => logData;
			set
			{
				logData = value;

				// TODO: REMOVE
				if (logData.ParseTime == default)
				{
					logData.ParseData();
				}

				Values.EncounterName = logData.EncounterName;

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

				Values.EncounterTime = logData.EncounterStartTime.ToLocalTime().DateTime
					.ToString(CultureInfo.CurrentCulture);
				var seconds = logData.EncounterDuration.TotalSeconds;
				Values.EncounterDuration = $"{seconds / 60:0}m {seconds % 60:0.0}s";

				Values.Result = $"{result} in {Values.EncounterDuration}";

				Values.ParseTimeMilliseconds = $"{logData.ParseMilliseconds} ms";

				Values.Players = logData.Players;
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
			nameLabel.TextBinding.Bind(Values, x => x.EncounterName);
			var resultLabel = new Label()
			{
				Font = Fonts.Sans(12)
			};
			resultLabel.TextBinding.Bind(Values, x => x.Result);

			var timeLabel = new Label();
			timeLabel.TextBinding.Bind(Values, x => x.EncounterTime);
			var durationLabel = new Label();
			durationLabel.TextBinding.Bind(Values, x => x.EncounterDuration);

			var groupComposition = new GroupCompositionControl(imageProvider);
			groupComposition.Bind(x => x.Players,
				new ObjectBinding<IEnumerable<Player>>(Values, nameof(Values.Players)));

			var debugDataVisibleBinding = new DelegateBinding<bool>(
				() => Settings.ShowDebugData,
				null,
				ev => Settings.ShowDebugDataChanged += ev,
				ev => Settings.ShowDebugDataChanged -= ev
			);

            var parseTimeExplanationLabel = new Label {Text = "Time spent parsing"};
            parseTimeExplanationLabel.Bind(x => x.Visible, debugDataVisibleBinding);

			var parseTimeLabel = new Label();
			parseTimeLabel.TextBinding.Bind(Values, x => x.ParseTimeMilliseconds);
			parseTimeLabel.Bind(
				x => x.Visible,
				debugDataVisibleBinding
			);

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
			BeginVertical();
			AddRow(parseTimeExplanationLabel, parseTimeLabel);
			BeginHorizontal();
			Add(new Button {Text = "Upload to dps.report (EI)", Enabled = false});
			Add(new Button {Text = "Upload to gw2raidar (EI)", Enabled = false});
			EndHorizontal();
			EndVertical();

			EndVertical();
		}
	}
}