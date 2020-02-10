﻿# Generates the models and service types.
#
# Much of the code in this project is generated by downloading a Swagger file, and running the AutoRest
# tool on the results. If the service is changed, you can run this script to regenerate the code.

# This requires the Workflow Message Ingestion function to be running locally on its default port of 7071
$tmp = (New-TemporaryFile).FullName
iwr http://localhost:7071/api/swagger -o $tmp


# If you do not have autorest, install it with:
#   npm install -g autorest
# Ensure it is up to date with
#   autorest --latest
$OutputFolder = Join-Path $PSScriptRoot "Endjin\Workflow\Client\Generated"
autorest --input-file=$tmp --csharp --output-folder=$OutputFolder --namespace=Endjin.Workflow.Client --override-client-name=WorkflowService --add-credentials