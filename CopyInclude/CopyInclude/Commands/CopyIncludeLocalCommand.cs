using Community.VisualStudio.Toolkit;
using System.Windows.Forms;

namespace CopyInclude
{
	[Command(PackageIds.CopyIncludeLocalCommand)]
	internal sealed class CopyIncludeLocalCommand : CopyIncludeCommandBase<CopyIncludeLocalCommand>
	{
		//////////////////////////////////////////////////////////////////////////
		protected override void FormatToClipboard(string filePath)
		{
			Clipboard.SetText("#include \"" + filePath + "\"");
		}
	}
}
