using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

//[DotNetVerbosityMapping]
//[UnsetVisualStudioEnvironmentVariables]
[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[GitHubActions(
    "ci",
    GitHubActionsImage.WindowsLatest,
    AutoGenerate = true,
    OnPushBranches = new[] { "main", "feature/**" },
    OnPullRequestBranches = new[] { "feature/**" },
    InvokedTargets = new[] { nameof(GitHubActions) },
    ImportSecrets = new[] { nameof(NuGetApiKey) })]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Default);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] string NuGetApiUrl = "https://api.nuget.org/v3/index.json";
    [Parameter] [Secret] string NuGetApiKey;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath NugetDirectory => OutputDirectory / "nuget";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(o => o.SetProjectFile(Solution));
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPack(o => o.SetConfiguration(Configuration)
                             .SetVersion(GitVersion.NuGetVersion)
                             .EnableNoBuild()
                             .EnableNoRestore()
                             .SetNoDependencies(true)
                             .SetOutputDirectory(NugetDirectory));
        });

    Target Push => _ => _
       .DependsOn(Pack)
       .Requires(() => !string.IsNullOrWhiteSpace(NuGetApiUrl))
       .Requires(() => !string.IsNullOrWhiteSpace(NuGetApiKey))
       .Requires(() => Configuration.Equals(Configuration.Release))
       .Executes(() =>
       {
           foreach(var c in NuGetApiKey)
           {
               Logger.Info($"Char: {c}");
           }
           
           var files = GlobFiles(NugetDirectory, "*.nupkg")
                .Where(filename => !filename.EndsWith("symbols.nupkg"))
                .ToArray();

           Assert.NotEmpty(files);

           files.ForEach(filename =>
           {
               DotNetNuGetPush(s => s
                   .SetTargetPath(filename)
                   .SetSource(NuGetApiUrl)
                   .SetApiKey(NuGetApiKey)
               );
           });
       });

    Target GitHubActions => _ => _
        .DependsOn(Push)
        .Executes();

    Target Default => _ => _
        .DependsOn(Pack)
        .Executes();
}
