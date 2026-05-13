using System;
using System.Collections.Generic;
using System.IO;
using Dock.App.Models;
using Microsoft.Win32;

namespace Dock.App.Services;

public static class DefaultDockItemFactory
{
    public static IReadOnlyList<DockItem> CreateInitialItems(LocalizedText text)
    {
        return
        [
            DockItem.CreateWindowsButton(),
            CreateShortcut(text["ItemWindowsSettings"], ResolveWindowsSettingsTarget()),
            CreateShortcut(text["ItemFileExplorer"], ResolveFileExplorerTarget()),
            CreateShortcut(text["ItemMicrosoftEdge"], ResolveEdgeTarget()),
            DockItem.CreateRecycleBin(text["ItemRecycleBin"])
        ];
    }

    private static DockItem CreateShortcut(string displayName, string targetPath)
    {
        return new DockItem
        {
            Kind = DockItemKind.Link,
            DisplayName = displayName,
            TargetPath = targetPath,
            OriginalSourcePath = targetPath
        };
    }

    private static string ResolveWindowsSettingsTarget()
    {
        var targetPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            "ImmersiveControlPanel",
            "SystemSettings.exe");

        return File.Exists(targetPath) ? targetPath : "ms-settings:";
    }

    private static string ResolveFileExplorerTarget()
    {
        var targetPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            "explorer.exe");

        return File.Exists(targetPath) ? targetPath : "explorer.exe";
    }

    private static string ResolveEdgeTarget()
    {
        foreach (var candidate in GetEdgeCandidates())
        {
            if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
            {
                return candidate;
            }
        }

        return "microsoft-edge:";
    }

    private static IEnumerable<string?> GetEdgeCandidates()
    {
        yield return ReadAppPath(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\msedge.exe");
        yield return ReadAppPath(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\msedge.exe");
        yield return ReadAppPath(Registry.LocalMachine, "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\App Paths\\msedge.exe");
        yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft", "Edge", "Application", "msedge.exe");
        yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft", "Edge", "Application", "msedge.exe");
    }

    private static string? ReadAppPath(RegistryKey root, string subKeyPath)
    {
        try
        {
            using var key = root.OpenSubKey(subKeyPath);
            return key?.GetValue(null) as string;
        }
        catch
        {
            return null;
        }
    }
}
