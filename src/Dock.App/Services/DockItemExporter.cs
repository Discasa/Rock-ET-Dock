using System;
using System.IO;
using Dock.App.Models;

namespace Dock.App.Services;

public sealed class DockItemExporter
{
    public string MoveToDesktop(DockItem item)
    {
        if (item.Kind is DockItemKind.WindowsButton or DockItemKind.RecycleBin ||
            item.IsRuntime ||
            string.IsNullOrWhiteSpace(item.TargetPath))
        {
            throw new InvalidOperationException("This item cannot be moved to the desktop.");
        }

        var sourcePath = Path.GetFullPath(item.TargetPath);
        if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath))
        {
            throw new FileNotFoundException("The backing item no longer exists.", sourcePath);
        }

        var desktop = UserPaths.DesktopDirectory;
        Directory.CreateDirectory(desktop);

        if (ManagedPathService.IsDirectChildOfDirectory(sourcePath, desktop))
        {
            return sourcePath;
        }

        return ManagedPathService.MoveFileSystemEntry(sourcePath, desktop);
    }
}
