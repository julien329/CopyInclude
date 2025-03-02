using Community.VisualStudio.Toolkit;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CopyInclude
{
	internal partial class OptionsProvider
	{
		[ComVisible(true)]
		public class GeneralOptions : BaseOptionPage<SettingsPage> { }
	}

	public class SettingsPage : BaseOptionModel<SettingsPage>
	{
		public const string _CategoryName = "Copy Include Settings";
		public const string _PageName = "Options";

		[Category(_CategoryName)]
		[DisplayName("Removed Prefixes")]
		[Description("List of paths prefixes to remove from the copied formatted file path.")]
		public string[] _RemovedPrefixes { get; set; }
	}
}
