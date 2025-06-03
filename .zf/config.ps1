<#
This example demonstrates a software build process using the 'ZeroFailed.Build.DotNet' extension
to provide the features needed when building a .NET solutions.
#>

$zerofailedExtensions = @(
    @{
        # References the extension from its GitHub repository. If not already installed, use latest version from 'main' will be downloaded.
        Name = "ZeroFailed.Build.DotNet"
        GitRepository = "https://github.com/zerofailed/ZeroFailed.Build.DotNet"
        GitRef = "main"
    }
)

# Load the tasks and process
. ZeroFailed.tasks -ZfPath $here/.zf

#
# Build process configuration
#
#
# Build process control options
#
$SkipInit = $false
$SkipVersion = $false
$SkipBuild = $false
$CleanBuild = $Clean
$SkipTest = $false
$SkipTestReport = $false
$SkipAnalysis = $false
$SkipPackage = $false

$SolutionToBuild = (Resolve-Path (Join-Path $here "./Solutions/Marain.Workflow.sln")).Path
$IncludeAssembliesInCodeCoverage = "Marain.Workflow*"
$NugetPublishSource = property ZF_NUGET_PUBLISH_SOURCE "$here/_local-nuget-feed"
$ProjectsToPublish = @(
    "Solutions\Marain.Workflow.Api.EngineHost\Marain.Workflow.Api.EngineHost.csproj"
    "Solutions\Marain.Workflow.Api.MessageProcessingHost\Marain.Workflow.Api.MessageProcessingHost.csproj"
)
# $ExcludeFilesFromCodeCoverage = "**/Marain.Workflow.Api.Client/**/Models/*.cs,**/Marain.Workflow.Api.Client/**/MarainWorkflowService*.cs,**/Marain.Workflow.Api.EngineHost.Client/**/Models/*.cs,**/Marain.Workflow.Api.EngineHost.Client/**/MarainWorkflowEngine*.cs"


# Customise the build process
task . FullBuild

#
# Build Process Extensibility Points - uncomment and implement as required
#

# task RunFirst {}
# task PreInit {}
# task PostInit {}
# task PreVersion {}
# task PostVersion {}
# task PreBuild {}
# task PostBuild {}
# task PreTest {}
# task PostTest {}
# task PreTestReport {}
# task PostTestReport {}
# task PreAnalysis {}
# task PostAnalysis {}
# task PrePackage {}
# task PostPackage {}
# task PrePublish {}
# task PostPublish {}
# task RunLast {}
