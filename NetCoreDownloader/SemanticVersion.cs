using System;
using System.Linq;

namespace NetCoreDownloader
{
    internal class SemanticVersion
    {
        public static SemanticVersion TryParse(string s)
        {
            string text = new string(s.TakeWhile((char c) => char.IsDigit(c) || c == '.').ToArray<char>());
            int length = text.Length;
            if (text.EndsWith("."))
            {
                text = text.Substring(0, text.Length - 1);
            }
            Version version;
            if (!Version.TryParse(text, out version))
            {
                return null;
            }
            string text2 = s.Substring(length).Trim();
            if (text2.StartsWith("-"))
            {
                text2 = text2.Substring(1).Trim();
            }
            return new SemanticVersion(s, version, text2);
        }

        public int CompareTo(object obj)
        {
            SemanticVersion semanticVersion = obj as SemanticVersion;
            if (semanticVersion == null)
            {
                return 1;
            }
            int num = this.Version.CompareTo(semanticVersion.Version);
            if (num != 0)
            {
                return num;
            }
            if (this.Release == semanticVersion.Release)
            {
                return 0;
            }
            if (this.Release == "")
            {
                return 1;
            }
            if (semanticVersion.Release == "")
            {
                return -1;
            }
            return this.Release.ToLowerInvariant().CompareTo(semanticVersion.Release.ToLowerInvariant());
        }

        public override bool Equals(object obj)
        {
            SemanticVersion semanticVersion = obj as SemanticVersion;
            return semanticVersion != null && semanticVersion.Version.Equals(this.Version) && semanticVersion.Release.ToLowerInvariant().Equals(this.Release.ToLowerInvariant());
        }

        public override int GetHashCode()
        {
            return this.Version.GetHashCode() * 37 + this.Release.GetHashCode();
        }

        public override string ToString()
        {
            return this._original;
        }

        private SemanticVersion(string original, Version version, string release)
        {
            this._original = original;
            this.Version = version;
            this.Release = release;
        }

        public readonly Version Version;

        public readonly string Release;

        private readonly string _original;
    }
}
