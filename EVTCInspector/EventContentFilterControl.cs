using Eto.Forms;
using GW2Scratch.EVTCAnalytics.Events;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace GW2Scratch.EVTCInspector
{
	public class EventContentFilterControl : Panel
	{
		private uint BuffId { get; set; }
		private bool BuffIdEnabled { get; set; }
		private uint BuffDamageId { get; set; }
		private bool BuffDamageIdEnabled { get; set; }
		private uint CastSkillId { get; set; }
		private bool CastSkillIdEnabled { get; set; }
		private uint DamageSkillId { get; set; }
		private bool DamageSkillIdEnabled { get; set; }

		private readonly DynamicLayout buffLayout;
		private readonly DynamicLayout buffDamageLayout;
		private readonly DynamicLayout castSkillLayout;
		private readonly DynamicLayout damageSkillLayout;

		public EventContentFilterControl()
		{
			buffLayout = ConstructLayout(
				() => BuffId, x => BuffId = x,
				() => BuffIdEnabled, x => BuffIdEnabled = x ?? false,
				"Buff Events",
				"Buff ID"
			);
			buffDamageLayout = ConstructLayout(
				() => BuffDamageId, x => BuffDamageId = x,
				() => BuffDamageIdEnabled, x => BuffDamageIdEnabled = x ?? false,
				"Buff Damage Events",
				"Buff ID"
			);
			castSkillLayout = ConstructLayout(
				() => CastSkillId, x => CastSkillId = x,
				() => CastSkillIdEnabled, x => CastSkillIdEnabled = x ?? false,
				"Skill Cast Events",
				"Skill ID"
			);
			damageSkillLayout = ConstructLayout(
				() => DamageSkillId, x => DamageSkillId = x,
				() => DamageSkillIdEnabled, x => DamageSkillIdEnabled = x ?? false,
				"Damage Events",
				"Skill ID"
			);
			var layout = new DynamicLayout();
			layout.BeginVertical();
			{
				layout.Add(buffLayout);
				layout.Add(buffDamageLayout);
				layout.Add(castSkillLayout);
				layout.Add(damageSkillLayout);
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

			var valueTextBox = new NumericMaskedTextBox<TValue>();
			valueTextBox.ValueBinding.Bind(valueGetter, valueSetter);

			var enabledCheckbox = new CheckBox();
			enabledCheckbox.CheckedBinding.Bind(enabledGetter, enabledSetter);

			var layout = new DynamicLayout();
			layout.BeginHorizontal();
			{
				layout.BeginGroup(groupTitle);
				{
					layout.AddRow(labelText, valueTextBox, enabledCheckbox, null);
				}
				layout.EndGroup();
				layout.AddRow(null);
			}
			layout.EndHorizontal();

			return layout;
		}

		public bool FilterEvent(Event e)
		{
			return e switch
			{
				BuffEvent buffEvent when BuffIdEnabled => buffEvent.Buff.Id == BuffId,
				BuffDamageEvent buffDamage when BuffDamageIdEnabled => buffDamage.Skill.Id == BuffDamageId,
				SkillCastEvent skillEvent when CastSkillIdEnabled => skillEvent.Skill.Id == CastSkillId,
				DamageEvent damageEvent when DamageSkillIdEnabled => damageEvent.Skill.Id == DamageSkillId,
				_ => true,
			};
		}

		private Type GetClosestType(Type a, Type b)
		{
			while (a != null)
			{
				if (a.IsAssignableFrom(b))
					return a;

				a = a.BaseType;
			}

			return null;
		}
	}
}