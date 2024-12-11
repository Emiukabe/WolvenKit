using static WolvenKit.RED4.Types.Enums;

namespace WolvenKit.RED4.Types
{
	public partial class gameuiIndexedMorphName : RedBaseClass
	{
		[Ordinal(0)] 
		[RED("index")] 
		public CInt32 Index
		{
			get => GetPropertyValue<CInt32>();
			set => SetPropertyValue<CInt32>(value);
		}

		[Ordinal(1)] 
		[RED("morphName")] 
		public CName MorphName
		{
			get => GetPropertyValue<CName>();
			set => SetPropertyValue<CName>(value);
		}

		[Ordinal(2)] 
		[RED("localizedName")] 
		public CString LocalizedName
		{
			get => GetPropertyValue<CString>();
			set => SetPropertyValue<CString>(value);
		}

		[Ordinal(3)] 
		[RED("tags")] 
		public redTagList Tags
		{
			get => GetPropertyValue<redTagList>();
			set => SetPropertyValue<redTagList>(value);
		}

		[Ordinal(4)] 
		[RED("randomizationInfo")] 
		public gameuiCharacterRandomizationInfo RandomizationInfo
		{
			get => GetPropertyValue<gameuiCharacterRandomizationInfo>();
			set => SetPropertyValue<gameuiCharacterRandomizationInfo>(value);
		}

		public gameuiIndexedMorphName()
		{
			Tags = new redTagList { Tags = new() };
			RandomizationInfo = new gameuiCharacterRandomizationInfo { MinRating = 1, MaxRating = 10 };

			PostConstruct();
		}

		partial void PostConstruct();
	}
}
