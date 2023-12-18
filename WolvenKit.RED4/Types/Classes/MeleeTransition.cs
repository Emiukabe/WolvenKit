using static WolvenKit.RED4.Types.Enums;

namespace WolvenKit.RED4.Types
{
	public abstract partial class MeleeTransition : DefaultTransition
	{
		[Ordinal(0)] 
		[RED("stateNameString")] 
		public CString StateNameString
		{
			get => GetPropertyValue<CString>();
			set => SetPropertyValue<CString>(value);
		}

		[Ordinal(1)] 
		[RED("driverCombatListener")] 
		public CHandle<DriverCombatListener> DriverCombatListener
		{
			get => GetPropertyValue<CHandle<DriverCombatListener>>();
			set => SetPropertyValue<CHandle<DriverCombatListener>>(value);
		}

		public MeleeTransition()
		{
			PostConstruct();
		}

		partial void PostConstruct();
	}
}
