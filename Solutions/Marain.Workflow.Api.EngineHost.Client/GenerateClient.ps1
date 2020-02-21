
# This requires the Engine Host function to be running locally on its default port of 7075
$tmp = (New-TemporaryFile).FullName
Invoke-WebRequest http://localhost:7075/swagger -o $tmp

$OutputFolder = Join-Path $PSScriptRoot "Marain\Workflows\EngineHost\Client\"


# If you do not have autorest, install it with:
#   npm install -g autorest
# Ensure it is up to date with
#   autorest --latest
autorest --input-file=$tmp --csharp --output-folder=$OutputFolder --namespace=Marain.Workflows.EngineHost.Client --add-credentials