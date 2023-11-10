// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");


var versions = new[] {
    "19.5.256",
    "19.5.248",
    "19.5.248+1",
    "19.5.248+2",
    "19.5.248+12",
    "19.5.259",
    "19.5.259+",
    "19.5.dev25500",
    "19.5.dev25500+1",
    "19.5.dev25500+12",
};


var versionOrdered = versions.Select(oo => new SemanticVersion(oo)).OrderDescending(new SemanticVersionComparer());

foreach (var version in versionOrdered) {
    Console.WriteLine(version);
}

Console.ReadLine();

public class SemanticVersion {
    static Regex regex = new(@"(?<major>[0-9]{1,9})\.(?<minor>[0-9]{1,9})\.?(?<patch>.*)");
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }
    public string PatchVersion { get; set; }
    public int HotfixVersion { get; set; }

    public SemanticVersion(string versionData) {

        var match = regex.Match(versionData);

        if (!match.Success) throw new InvalidOperationException($"({versionData}) in not a valid semantic version.");

        this.MajorVersion = int.Parse(match.Groups["major"].Value);
        this.MinorVersion = int.Parse(match.Groups["minor"].Value);
        var patch = match.Groups["patch"].Value;

        var hotfixIndex = patch.IndexOf('+');

        if (hotfixIndex != -1) {
            PatchVersion = patch[0..hotfixIndex];
            var hotfixData = patch[hotfixIndex..];
            if (hotfixData.EndsWith("+")) return;
            HotfixVersion = int.Parse(hotfixData);
            return;
        }

        PatchVersion = patch;
    }

    public override string ToString() {
        if (HotfixVersion > 0) return $"{MajorVersion}.{MinorVersion}.{PatchVersion}+{HotfixVersion}";

        return $"{MajorVersion}.{MinorVersion}.{PatchVersion}";
    }
}

public class SemanticVersionComparer : IComparer<SemanticVersion> {

    public int Compare(SemanticVersion x, SemanticVersion y) {
        if (x == null) return -1;
        if (y == null) return -1;

        var compareResult = x.MajorVersion.CompareTo(y.MajorVersion);

        if (compareResult != 0) return compareResult;

        compareResult = x.MinorVersion.CompareTo(y.MinorVersion);

        if (compareResult != 0) return compareResult;

        compareResult = x.PatchVersion.PadLeft(99, '0').CompareTo(y.PatchVersion.PadLeft(99, '0'));

        if (compareResult != 0) return compareResult;

        return x.HotfixVersion.CompareTo(y.HotfixVersion);
    }
}