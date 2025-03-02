using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace CopyInclude
{
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(PackageGuids.CopyIncludeString)]
	[ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), SettingsPage._CategoryName, SettingsPage._PageName, 0, 0, true)]
	[ProvideProfile(typeof(OptionsProvider.GeneralOptions), SettingsPage._CategoryName, SettingsPage._PageName, 0, 0, true)]
	[ProvideBindingPath]
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
	public sealed class ExtensionPackage : ToolkitPackage
	{
		public DTE2 _DTE
		{
			get;
			private set;
		}

		//////////////////////////////////////////////////////////////////////////
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			_DTE = await GetServiceAsync(typeof(DTE)) as DTE2;

			await this.RegisterCommandsAsync();
		}
	}
}
