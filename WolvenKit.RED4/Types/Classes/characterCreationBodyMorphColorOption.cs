using static WolvenKit.RED4.Types.Enums;

namespace WolvenKit.RED4.Types
{
	public partial class characterCreationBodyMorphColorOption : CharacterCreationBodyMorphBaseOption
	{
		[Ordinal(2)] 
		[RED("optionLabel")] 
		public inkTextWidgetReference OptionLabel
		{
			get => GetPropertyValue<inkTextWidgetReference>();
			set => SetPropertyValue<inkTextWidgetReference>(value);
		}

		[Ordinal(3)] 
		[RED("colorPickerBtn")] 
		public inkWidgetReference ColorPickerBtn
		{
			get => GetPropertyValue<inkWidgetReference>();
			set => SetPropertyValue<inkWidgetReference>(value);
		}

		[Ordinal(4)] 
		[RED("selectorNextBtn")] 
		public inkWidgetReference SelectorNextBtn
		{
			get => GetPropertyValue<inkWidgetReference>();
			set => SetPropertyValue<inkWidgetReference>(value);
		}

		[Ordinal(5)] 
		[RED("selectorPrevBtn")] 
		public inkWidgetReference SelectorPrevBtn
		{
			get => GetPropertyValue<inkWidgetReference>();
			set => SetPropertyValue<inkWidgetReference>(value);
		}

		[Ordinal(6)] 
		[RED("selectorTexture")] 
		public inkImageWidgetReference SelectorTexture
		{
			get => GetPropertyValue<inkImageWidgetReference>();
			set => SetPropertyValue<inkImageWidgetReference>(value);
		}

		[Ordinal(7)] 
		[RED("arrowsTexture")] 
		public inkImageWidgetReference ArrowsTexture
		{
			get => GetPropertyValue<inkImageWidgetReference>();
			set => SetPropertyValue<inkImageWidgetReference>(value);
		}

		[Ordinal(8)] 
		[RED("optionSwitchHint")] 
		public inkWidgetReference OptionSwitchHint
		{
			get => GetPropertyValue<inkWidgetReference>();
			set => SetPropertyValue<inkWidgetReference>(value);
		}

		[Ordinal(9)] 
		[RED("colorPickerOption")] 
		public CWeakHandle<gameuiCharacterCustomizationOption> ColorPickerOption
		{
			get => GetPropertyValue<CWeakHandle<gameuiCharacterCustomizationOption>>();
			set => SetPropertyValue<CWeakHandle<gameuiCharacterCustomizationOption>>(value);
		}

		[Ordinal(10)] 
		[RED("appearanceInfo")] 
		public CWeakHandle<gameuiAppearanceInfo> AppearanceInfo
		{
			get => GetPropertyValue<CWeakHandle<gameuiAppearanceInfo>>();
			set => SetPropertyValue<CWeakHandle<gameuiAppearanceInfo>>(value);
		}

		[Ordinal(11)] 
		[RED("currColorIndex")] 
		public CInt32 CurrColorIndex
		{
			get => GetPropertyValue<CInt32>();
			set => SetPropertyValue<CInt32>(value);
		}

		[Ordinal(12)] 
		[RED("selector")] 
		public CWeakHandle<inkWidget> Selector
		{
			get => GetPropertyValue<CWeakHandle<inkWidget>>();
			set => SetPropertyValue<CWeakHandle<inkWidget>>(value);
		}

		[Ordinal(13)] 
		[RED("inputDisabled")] 
		public CBool InputDisabled
		{
			get => GetPropertyValue<CBool>();
			set => SetPropertyValue<CBool>(value);
		}

		[Ordinal(14)] 
		[RED("isVisible")] 
		public CBool IsVisible
		{
			get => GetPropertyValue<CBool>();
			set => SetPropertyValue<CBool>(value);
		}

		public characterCreationBodyMorphColorOption()
		{
			OptionLabel = new inkTextWidgetReference();
			ColorPickerBtn = new inkWidgetReference();
			SelectorNextBtn = new inkWidgetReference();
			SelectorPrevBtn = new inkWidgetReference();
			SelectorTexture = new inkImageWidgetReference();
			ArrowsTexture = new inkImageWidgetReference();
			OptionSwitchHint = new inkWidgetReference();
			CurrColorIndex = -1;

			PostConstruct();
		}

		partial void PostConstruct();
	}
}
