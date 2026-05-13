using System;
using System.Diagnostics;
using System.IO;
using Dock.App.Models;

namespace Dock.App.Services;

public static class DockLauncher
{
    public static bool Open(DockItem item)
    {
        if (item.Kind == DockItemKind.WindowsButton)
        {
            WindowsButtonService.OpenStartMenu();
            return true;
        }

        if (item.Kind == DockItemKind.RecycleBin)
        {
            RecycleBinService.Open();
            return true;
        }

        if (item.Kind == DockItemKind.Window)
        {
            return WindowMinimizeMonitor.RestoreWindow(item.NativeWindowHandle);
        }

        if (string.IsNullOrWhiteSpace(item.TargetPath))
        {
            return false;
        }

        if (IsWindowsSettingsTarget(item.TargetPath))
        {
            StartShellTarget("ms-settings:");
            return true;
        }

        if (IsFileExplorerTarget(item.TargetPath))
        {
            StartShellTarget("explorer.exe");
            return true;
        }

        StartShellTarget(item.TargetPath);
        return true;
    }

    public static bool IsWindowsShellCommand(DockItem item)
    {
        return !string.IsNullOrWhiteSpace(item.TargetPath) &&
               (IsWindowsSettingsTarget(item.TargetPath) || IsFileExplorerTarget(item.TargetPath));
    }

    private static void StartShellTarget(string target)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = target,
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }

    private static bool IsWindowsSettingsTarget(string targetPath)
    {
        if (targetPath.StartsWith("ms-settings:", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return Path.GetFileName(targetPath).Equals("SystemSettings.exe", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsFileExplorerTarget(string targetPath)
    {
        if (targetPath.Equals("explorer.exe", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return Path.GetFileName(targetPath).Equals("explorer.exe", StringComparison.OrdinalIgnoreCase);
    }
}
