using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetCoreDownloader
{
    internal class FxChecker
    {
        private static string ProgramFiles64Bit
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).Replace("(x86)", "").Replace("(X86)", "").Trim();
            }
        }

        private static string ProgramFilesX86
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }
        }

        public static bool IsArm64
        {
            get
            {
                bool? isArm = FxChecker._isArm64;
                if (isArm == null)
                {
                    string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine);
                    bool? flag = FxChecker._isArm64 = new bool?(((environmentVariable != null) ? environmentVariable.ToUpperInvariant() : null) == "ARM64");
                    return flag.Value;
                }
                return isArm.GetValueOrDefault();
            }
        }

        public static bool IsX86
        {
            get
            {
                return !FxChecker.IsArm64 && !Environment.Is64BitOperatingSystem;
            }
        }

        public static bool Is64Bit
        {
            get
            {
                return FxChecker.IsArm64 || Environment.Is64BitOperatingSystem;
            }
        }
        private static SemanticVersion GetMinVersion(SemanticVersion v1, SemanticVersion v2)
        {
            if (v1 != null && v1.CompareTo(v2) >= 0)
            {
                return v2;
            }
            return v1;
        }

        private static SemanticVersion GetMaxVersion(SemanticVersion v1, SemanticVersion v2)
        {
            if (v1 != null && v1.CompareTo(v2) >= 0)
            {
                return v1;
            }
            return v2;
        }

        public static SemanticVersion FindHighestValidVersion(bool for64bit)
        {
            SemanticVersion semanticVersion = FxChecker.FindHighestValidVersion(Path.Combine(for64bit ? FxChecker.ProgramFiles64Bit : FxChecker.ProgramFilesX86, "dotnet"));
            if (FxChecker.Is64Bit != for64bit)
            {
                return semanticVersion;
            }
            string environmentVariable = Environment.GetEnvironmentVariable("dotnet_root");
            if (environmentVariable == null || Path.GetInvalidPathChars().Intersect(environmentVariable).Any<char>())
            {
                return semanticVersion;
            }
            SemanticVersion result;
            try
            {
                result = FxChecker.GetMaxVersion(semanticVersion, FxChecker.FindHighestValidVersion(environmentVariable));
            }
            catch
            {
                result = semanticVersion;
            }
            return result;
        }

        private static SemanticVersion FindHighestValidVersion(string dotNetFolder)
        {
            return FxChecker.FindHighestVersion(dotNetFolder, "Microsoft.WindowsDesktop.app");
        }

        private static SemanticVersion FindHighestVersion(string dotNetFolder, string package)
        {
            return (from v in FxChecker.GetDotNetVersions(Path.Combine(dotNetFolder, "shared", package))
                    where v.Version.Major == 3 || v.Version.Major == 5 || v.Version.Major == 6
                    orderby v descending
                    select v).FirstOrDefault<SemanticVersion>();
        }

        private static IEnumerable<SemanticVersion> GetDotNetVersions(string baseFolder)
        {
            if (Directory.Exists(baseFolder))
            {
                return from dir in new DirectoryInfo(baseFolder).GetDirectories()
                       let parsedVersion = SemanticVersion.TryParse(dir.Name)
                       where parsedVersion != null && (string.IsNullOrEmpty(parsedVersion.Release) || parsedVersion.Version.Major >= 6)
                       select parsedVersion;
            }
            return new SemanticVersion[0];
        }

        internal static SemanticVersion MinVersion = SemanticVersion.TryParse("3.1.0");

        internal static SemanticVersion MinRecommendedVersion = (DateTime.Now < new DateTime(2021, 12, 1)) ? SemanticVersion.TryParse("6.0.0-rc.1.21451.3") : SemanticVersion.TryParse("6.0.0");

        private const string CorePackageName = "Microsoft.NETCore.app";

        private const string DesktopPackageName = "Microsoft.WindowsDesktop.app";

        private static bool? _isArm64;
    }
}
