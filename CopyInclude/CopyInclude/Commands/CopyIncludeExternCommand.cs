using Community.VisualStudio.Toolkit;
using System.Windows.Forms;

namespace CopyInclude
{
	[Command(PackageIds.CopyIncludeExternCommand)]
	internal sealed class CopyIncludeExternCommand : CopyIncludeCommandBase<CopyIncludeExternCommand>
	{
		//////////////////////////////////////////////////////////////////////////
		protected override void FormatToClipboard(string filePath)
		{
			Clipboard.SetText("#include <" + filePath + ">");
		}
	}
}
