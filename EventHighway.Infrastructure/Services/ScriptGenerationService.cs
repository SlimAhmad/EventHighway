// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using ADotNet.Clients;
using ADotNet.Clients.Builders;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;

namespace EventHighway.Infrastructure.Services
{
    internal class ScriptGenerationService
    {
        private readonly ADotNetClient adotNetClient;

        public ScriptGenerationService() =>
            adotNetClient = new ADotNetClient();

        public void GenerateBuildScript(
            string branchName,
            string projectName,
            string dotNetVersion)
        {
            GitHubPipelineBuilder.CreateNewPipeline()
              .SetName(projectName)
              .OnPush(branchName)
              .OnPullRequest(branchName)

              .AddJob("build-windows", job => job
                  .WithName("Build (Windows)")
                  .RunsOn(BuildMachines.WindowsLatest)
                  .AddCheckoutStep("Check out")
                  .AddSetupDotNetStep(dotNetVersion)
                  .AddRestoreStep()
                  .AddBuildStep()
                  .AddTestStep())

              .AddJob("build-integration", job => job
                  .WithName("Build & Test (DB matrix)")
                  .RunsOn(BuildMachines.UbuntuLatest)
                  .AddMatrix("database", "sqlserver", "postgres")
                  .AddService("sqlserver", service => service
                      .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
                      .AddEnvironmentVariable("ACCEPT_EULA", "Y")
                      .AddEnvironmentVariable("SA_PASSWORD", "Your_password123")
                      .AddPort(1433, 1433))
                  .AddService("postgres", service => service
                      .WithImage("postgres:17")
                      .AddEnvironmentVariable("POSTGRES_DB", "EventHighwayDb")
                      .AddEnvironmentVariable("POSTGRES_USER", "postgres")
                      .AddEnvironmentVariable("POSTGRES_PASSWORD", "postgres")
                      .AddPort(5432, 5432))
                  .AddEnvironmentVariables(new Dictionary<string, string>
                  {
                      ["SQLSERVER_CONN"] =
                          "Server=localhost;Database=EventHighwayDb;User Id=sa;Password=Your_password123;",
                      ["POSTGRES_CONN"] =
                          "Host=localhost;Database=EventHighwayDb;Username=postgres;Password=postgres"
                  })
                  .AddEnvironmentVariable(
                      "CONNECTION_STRING",
                      "${{ matrix.database == 'sqlserver' && env.SQLSERVER_CONN || env.POSTGRES_CONN }}")
                  .AddCheckoutStep()
                  .AddSetupDotNetStep("10.0.100")
                  .AddRestoreStep()
                  .AddBuildStep()
                  .AddTestStep())

              .AddJob("add_tag", job => job
                  .WithName("Tag and Release")
                  .RunsOn(BuildMachines.UbuntuLatest)
                  .DependsOn("build")
                  .WithCondition(
                      "needs.build.result == 'success' && " +
                      "github.event.pull_request.merged && " +
                      "github.event.pull_request.base.ref == 'main' && " +
                      "startsWith(github.event.pull_request.title, 'RELEASES:') && " +
                      "contains(github.event.pull_request.labels.*.name, 'RELEASES')")
                  .AddActionStep(
                      name: "Checkout code",
                      uses: "actions/checkout@v3",
                      with: new Dictionary<string, string>
                      {
                          ["token"] = "${{ secrets.PAT_FOR_TAGGING }}"
                      })
                  .AddGenericStep(
                      name: "Configure Git",
                      runCommand:
                          "git config user.name \"GitHub Action\"\n" +
                          "git config user.email \"action@github.com\"")
                  .AddGenericStep(
                      id: "extract_version",
                      name: "Extract Version",
                      shell: "bash",
                      runCommand:
                          "sudo apt-get install xmlstarlet\n" +
                          "version_number=$(xmlstarlet sel -t -v \"//Version\" " +
                          "-n EventHighway.Core/EventHighway.Core.csproj)\n" +
                          "echo \"$version_number\"\n" +
                          "echo \"version_number<<EOF\" >> $GITHUB_OUTPUT\n" +
                          "echo \"$version_number\" >> $GITHUB_OUTPUT\n" +
                          "echo \"EOF\" >> $GITHUB_OUTPUT")
                  .AddGenericStep(
                      name: "Display Version",
                      runCommand: "echo \"Version number: ${{ steps.extract_version.outputs.version_number }}\"")
                  .AddGenericStep(
                      id: "extract_package_release_notes",
                      name: "Extract Package Release Notes",
                      shell: "bash",
                      runCommand:
                          "sudo apt-get install xmlstarlet\n" +
                          "package_release_notes=$(xmlstarlet sel -t -v \"//PackageReleaseNotes\" " +
                          "-n EventHighway.Core/EventHighway.Core.csproj)\n" +
                          "echo \"$package_release_notes\"\n" +
                          "echo \"package_release_notes<<EOF\" >> $GITHUB_OUTPUT\n" +
                          "echo \"$package_release_notes\" >> $GITHUB_OUTPUT\n" +
                          "echo \"EOF\" >> $GITHUB_OUTPUT")
                  .AddGenericStep(
                      name: "Display Package Release Notes",
                      runCommand: "echo \"Package Release Notes:" +
                      " ${{ steps.extract_package_release_notes.outputs.package_release_notes }}\"")
                  .AddGenericStep(
                      name: "Create GitHub Tag",
                      runCommand:
                          "git tag -a \"v${{ steps.extract_version.outputs.version_number }}\" -m \"Release -" +
                          " v${{ steps.extract_version.outputs.version_number }}\"\n" +
                          "git push origin --tags")
                  .AddActionStep(
                      name: "Create GitHub Release",
                      uses: "actions/create-release@v1",
                      with: new Dictionary<string, string>
                      {
                          ["tag_name"] = "v${{ steps.extract_version.outputs.version_number }}",
                          ["release_name"] = "Release - v${{ steps.extract_version.outputs.version_number }}",
                          ["body"] =
                              "## Release - v${{ steps.extract_version.outputs.version_number }}\n\n" +
                              "### Release Notes\n" +
                              "${{ steps.extract_package_release_notes.outputs.package_release_notes }}"
                      },
                      environmentVariables: new Dictionary<string, string>
                      {
                          ["GITHUB_TOKEN"] = "${{ secrets.PAT_FOR_TAGGING }}"
                      }))

              .AddJob("publish", job => job
                  .WithName("Publish to NuGet")
                  .RunsOn(BuildMachines.UbuntuLatest)
                  .DependsOn("add_tag")
                  .WithCondition("needs.add_tag.result == 'success'")
                  .AddCheckoutStep("Check out")
                  .AddSetupDotNetStep(dotNetVersion)
                  .AddRestoreStep()
                  .AddGenericStep(
                      name: "Build",
                      runCommand: "dotnet build --no-restore --configuration Release")
                  .AddGenericStep(
                      name: "Pack NuGet Package",
                      runCommand: "dotnet pack --configuration Release --include-symbols")
                  .AddGenericStep(
                      name: "Push NuGet Package",
                      runCommand:
                          "dotnet nuget push **/bin/Release/**/*.nupkg " +
                          "--source https://api.nuget.org/v3/index.json " +
                          "--api-key ${{ secrets.NUGET_ACCESS }} --skip-duplicate"))

              .SaveToFile("../../../../.github/workflows/build.yml");
        }

        public void GeneratePrLintScript(string branchName)
        {
            var githubPipeline = new GithubPipeline
            {
                Name = "PR Linter",

                OnEvents = new Events
                {
                    PullRequest = new PullRequestEvent
                    {
                        Types = ["opened", "edited", "synchronize", "reopened", "closed"],
                        Branches = [branchName]
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "label",
                        new LabelJobV2(runsOn: BuildMachines.UbuntuLatest)
                        {
                            Name = "Label",
                            Permissions = new Dictionary<string, string>
                            {
                                { "contents", "read" },
                                { "pull-requests", "write" },
                                { "issues", "write" }
                            }
                        }
                    },
                    {
                        "requireIssueOrTask",
                        new RequireIssueOrTaskJob()
                        {
                            Name = "Require Issue Or Task Association",
                        }
                    },
                }
            };

            string buildScriptPath = "../../../../.github/workflows/prLinter.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adotNetClient.SerializeAndWriteToFile(
                adoPipeline: githubPipeline,
                path: buildScriptPath);
        }
    }
}
