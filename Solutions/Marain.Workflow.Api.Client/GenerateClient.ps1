
# This requires the Message Host function to be running locally on its default port of 7073
$tmp = (New-TemporaryFile).FullName
Invoke-WebRequest http://localhost:7073/swagger -o $tmp

$OutputFolder = Join-Path $PSScriptRoot "Marain\Workflows\Api\Client\"


# If you do not have autorest, install it with:
#   npm install -g autorest
# Ensure it is up to date with
#   autorest --latest
autorest --input-file=$tmp --csharp --output-folder=$OutputFolder --namespace=Marain.Workflows.Api.Client --add-credentials --override-client-name=MarainWorkflowService