using Eto.Forms;
using GW2Scratch.EVTCAnalytics.Events;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace GW2Scratch.EVTCInspector
{
	public class EventContentFilterControl : Panel
	{
		private uint IdFilter { get; set; }
		private bool IdFilterEnabled { get; set; }
		private string NameFilter { get; set; }
		private bool NameFilterEnabled { get; set; }

		private readonly DynamicLayout idLayout;
		private readonly DynamicLayout nameLayout;

		public EventContentFilterControl()
		{
			idLayout = ConstructLayout(
				() => IdFilter, x => IdFilter = x,
				() => IdFilterEnabled, x => IdFilterEnabled = x ?? false,
				"Filter events by Skill ID",
				"Skill ID"
			);
			nameLayout = ConstructLayout(
				() => NameFilter, x => NameFilter = x,
				() => NameFilterEnabled, x => NameFilterEnabled = x ?? false,
				"Filter events by Skill Name",
				"Skill Name"
			);

			var layout = new DynamicLayout();
			layout.BeginVertical();
			{
				layout.Add(idLayout);
				layout.Add(nameLayout);
			}
			layout.EndVertical();
			Content = layout;
		}

		private static DynamicLayout ConstructLayout<TValue>(
			Expression<Func<TValue>> valueGetterExpr, Action<TValue> valueSetter,
			Expression<Func<bool?>> enabledGetterExpr, Action<bool?> enabledSetter,
			string groupTitle, string labelText)
		{
			var valueGetter = valueGetterExpr.Compile();
			var enabledGetter = enabledGetterExpr.Compile();

			Control valueControl;

			if (typeof(TValue) == typeof(uint))
			{
				// Numberic filter
				var numericTextBox = new NumericMaskedTextBox<TValue>();
				numericTextBox.ValueBinding.Bind(valueGetter, valueSetter);

				valueControl = numericTextBox;
			}
			else
			{
				// Text filter
				var textBox = new TextBox();

				textBox.TextBinding.Bind(
					() => (string) (object) valueGetter(),
					s => valueSetter((TValue) (object) s)
				);

				valueControl = textBox;
			}

			var enabledCheckbox = new CheckBox();
			enabledCheckbox.CheckedBinding.Bind(enabledGetter, enabledSetter);

			var layout = new DynamicLayout();
			layout.BeginHorizontal();
			{
				layout.BeginGroup(groupTitle);
				{
					layout.AddRow(labelText, valueControl, enabledCheckbox, null);
				}
				layout.EndGroup();
				layout.AddRow(null);
			}
			layout.EndHorizontal();

			return layout;
		}

		public bool FilterEvent(Event e)
		{
			switch (e)
			{
				case BuffEvent buff:
					if (IdFilterEnabled && buff.Buff.Id == IdFilter) return true;
					if (NameFilterEnabled && buff.Buff.Name.Contains(NameFilter, StringComparison.CurrentCultureIgnoreCase)) return true;
					return !(IdFilterEnabled || NameFilterEnabled);
				case SkillCastEvent skillEvent:
					if (IdFilterEnabled && skillEvent.Skill.Id == IdFilter) return true;
					if (NameFilterEnabled && skillEvent.Skill.Name.Contains(NameFilter, StringComparison.CurrentCultureIgnoreCase)) return true;
					return !(IdFilterEnabled || NameFilterEnabled);
				case DamageEvent damageEvent:
					if (IdFilterEnabled && damageEvent.Skill.Id == IdFilter) return true;
					if (NameFilterEnabled && damageEvent.Skill.Name.Contains(NameFilter, StringComparison.CurrentCultureIgnoreCase)) return true;
					return !(IdFilterEnabled || NameFilterEnabled);

				default:
					return true;
			}
		}
	}
}