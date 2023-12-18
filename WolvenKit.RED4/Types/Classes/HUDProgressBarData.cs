using static WolvenKit.RED4.Types.Enums;

namespace WolvenKit.RED4.Types
{
	public partial class HUDProgressBarData : RedBaseClass
	{
		[Ordinal(0)] 
		[RED("header")] 
		public CString Header
		{
			get => GetPropertyValue<CString>();
			set => SetPropertyValue<CString>(value);
		}

		[Ordinal(1)] 
		[RED("bottomText")] 
		public CString BottomText
		{
			get => GetPropertyValue<CString>();
			set => SetPropertyValue<CString>(value);
		}

		[Ordinal(2)] 
		[RED("completedText")] 
		public CString CompletedText
		{
			get => GetPropertyValue<CString>();
			set => SetPropertyValue<CString>(value);
		}

		[Ordinal(3)] 
		[RED("failedText")] 
		public CString FailedText
		{
			get => GetPropertyValue<CString>();
			set => SetPropertyValue<CString>(value);
		}

		[Ordinal(4)] 
		[RED("active")] 
		public CBool Active
		{
			get => GetPropertyValue<CBool>();
			set => SetPropertyValue<CBool>(value);
		}

		[Ordinal(5)] 
		[RED("progress")] 
		public CFloat Progress
		{
			get => GetPropertyValue<CFloat>();
			set => SetPropertyValue<CFloat>(value);
		}

		[Ordinal(6)] 
		[RED("type")] 
		public CEnum<gameSimpleMessageType> Type
		{
			get => GetPropertyValue<CEnum<gameSimpleMessageType>>();
			set => SetPropertyValue<CEnum<gameSimpleMessageType>>(value);
		}

		public HUDProgressBarData()
		{
			BottomText = "LocKey#22169";
			CompletedText = "LocKey#15455";
			FailedText = "LocKey#15353";

			PostConstruct();
		}

		partial void PostConstruct();
	}
}
