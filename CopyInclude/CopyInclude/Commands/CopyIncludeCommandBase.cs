using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CopyInclude
{
	public abstract class CopyIncludeCommandBase<T> : BaseCommand<T> where T : class, new()
	{
		private DTE2 _DTE
		{
			get => (Package as ExtensionPackage)._DTE;
		}

		private SettingsPage _UserSettings
		{
			get => SettingsPage.Instance;
		}

		//////////////////////////////////////////////////////////////////////////
		protected abstract void FormatToClipboard(string filePath);

		//////////////////////////////////////////////////////////////////////////
		protected override void Execute(object sender, EventArgs e)
		{
			string filePath = GetRelPath();

			if (!string.IsNullOrEmpty(filePath))
			{
				if (_UserSettings._RemovedPrefixes != null)
				{
					foreach (string prefix in _UserSettings._RemovedPrefixes)
					{
						if (filePath.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
						{
							filePath = filePath.Remove(0, prefix.Length);
							filePath = filePath.TrimStart('/', '\\');

							break;
						}
					}
				}

				FormatToClipboard(filePath);
			}
		}

		//////////////////////////////////////////////////////////////////////////
		private string GetRelPath()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			string fileName = "";
			if (_DTE.ActiveWindow.Type == vsWindowType.vsWindowTypeDocument)
			{
				// Document context menu.
				Document activeDoc = _DTE.ActiveDocument;
				if (activeDoc != null)
				{
					fileName = activeDoc.FullName;
				}
			}

			if (string.IsNullOrEmpty(fileName) || _DTE.ActiveWindow.Type == vsWindowType.vsWindowTypeSolutionExplorer)
			{
				// Solution explorer context menu.
				fileName = GetPathFromSelectedProjectItem();
				if (string.IsNullOrEmpty(fileName))
				{
					// Probably, selected item is not part of the project.
					fileName = GetPathFromHierarchy();
				}
			}

			// Do nothing if failed to obtain the correct file name.
			if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
			{
				return null;
			}

			string basePath = Path.GetDirectoryName(_DTE.Solution.FullName);

			// Compare path components ignoring case (assume NTFS).
			if (fileName.StartsWith(basePath, StringComparison.CurrentCultureIgnoreCase))
			{
				fileName = fileName.Remove(0, basePath.Length);
			}
			else
			{
				// Trim common prefix
				string[] basePathComponents = basePath.Split(Path.DirectorySeparatorChar);
				string[] fileNameComponents = fileName.Split(Path.DirectorySeparatorChar);
				int minLen = Math.Min(basePathComponents.Length, fileNameComponents.Length);

				int i = 0;
				for (; i < minLen; ++i)
				{
					if (!fileNameComponents[i].Equals(basePathComponents[i], StringComparison.CurrentCultureIgnoreCase))
					{
						break;
					}
				}
				var subPathComponents = fileNameComponents.Skip(i).ToArray();
				fileName = Path.Combine(subPathComponents);
			}

			fileName = fileName.TrimStart(Path.DirectorySeparatorChar);
			fileName = fileName.Replace(Path.DirectorySeparatorChar, '/');

			return fileName;
		}

		//////////////////////////////////////////////////////////////////////////
		private string GetPathFromSelectedProjectItem()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			SelectedItems selItems = _DTE.SelectedItems;
			if (selItems.Count > 0)
			{
				SelectedItem item = selItems.Item(1);
				if (item != null)
				{
					ProjectItem projItem = item.ProjectItem;
					if (projItem != null)
					{
						// Selected item is a part of the project.
						if (projItem.FileCount > 0)
						{
							return projItem.FileNames[0];
						}
					}
				}
			}

			return "";
		}

		//////////////////////////////////////////////////////////////////////////
		private string GetPathFromHierarchy()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			UIHierarchy slnExplorer = _DTE.ToolWindows.SolutionExplorer;
			if (slnExplorer.SelectedItems is object[] hierItems && hierItems.Length > 0)
			{
				// Get all the parent items up to the solution item (excluding it).
				List<string> hierPath = new List<string>();
				UIHierarchyItem hierItem = hierItems[0] as UIHierarchyItem;
				if (hierItem != null && hierItem.IsSelected)
				{
					while (hierItem != null)
					{
						object parent = hierItem.Collection.Parent;
						if (parent.Equals(slnExplorer) || parent is EnvDTE.Solution)
						{
							break;
						}
						hierPath.Insert(0, hierItem.Name);
						hierItem = parent as UIHierarchyItem;
					}
				}
				if (hierPath.Count != 0)
				{
					string slnDir = Path.GetDirectoryName(_DTE.Solution.FileName);
					if (hierPath.Count > 1)
					{
						// Remove project item (which comes 1st) if there is no directory for it.
						string filePath = Path.Combine(slnDir, hierPath[0]);
						if (!Directory.Exists(filePath))
						{
							hierPath.RemoveAt(0);
						}
					}
					return Path.Combine(slnDir, string.Join("\\", hierPath));
				}
			}

			return "";
		}
	}
}
