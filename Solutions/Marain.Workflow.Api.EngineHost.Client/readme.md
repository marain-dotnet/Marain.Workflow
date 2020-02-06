# API client project

This project contains an auto-generated client for the main API. It was generated with the `nswag` tool - https://github.com/RSuter/NSwag - rather than Autorest because the project's YAML uses some OpenApi 3.0 constructs which aren't yet supported in Autorest.

The easiest way to install NSwag is via npm. using the command `npm install nswag -g`

You will also need to convert the .YAML file to a json file. This can be done using OpenAPI Document Converters for visual studio. You can install them from here: https://blogs.endjin.com/2018/05/openapi-document-converters-for-visual-studio-2017/

Once OpenAPI Document Converters is installed, ensure that it is configured correctly to output openAPI 2.0.

Once NSwag is installed, navigate to this folder in a command prompt and execute `nswag run`. This will use settings from the `nswag.json` file to regenerate the proxy.