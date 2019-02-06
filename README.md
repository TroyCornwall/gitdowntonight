# GitDownTonight
#### Find out who has contributed the most to a Github Organization
A Technical Test for a Senior Software Engineer role at JRNY.ai


## Setting the configuration
There are 3 ways to set your access token and which organization to query

You can mix and pass one in one way (like your access key in the appsettings.json and the organization in cli parameters)

NOTE: The examples contain an older access token of mine, I have revoked it, after I accidentally committed it.

#### The appsettings.json file
In the root of the project is a appsettings.json file.
This contains example settings, overridding defaults in the project
The file should look like the following. 
`{
    "GhAccessToken": "fca86dc1af657b53f42870262113db6224da7b47",
    "Organization": "Github"
 }`
 
 After this, the easiest way to run this is to go
 `dotnet run` in the project directory (the one with appsettings.json)

#### Environment Variables
If you don't want to put your access token in a file
You can instead pass as environment variables

An example of passing these in like this is: 
 `GhAccessToken=fca86dc1af657b53f42870262113db6224da7b47 Organization=Apple dotnet run`
 
#### CLI Arguements
The last way to pass in the config is by CLI Arguments

`dotnet run --GhAccessToken fca86dc1af657b53f42870262113db6224da7b47 --Organization IBM `
 
 
 ##### Mixing these
 If you want to query multiple organizations, but don't want to pass your token over the CLI or in Environment Variables, you could do something like, storing your access token in the `appsettings.json`
 
 appsettings.json:
 `{
      "GhAccessToken": "fca86dc1af657b53f42870262113db6224da7b47"
 }`
 
 Then running the following commands:
 
 `dotnet run --Organization Github`
 
 `dotnet run --Organization Apple`
 
 `dotnet run --Organization IBM`
 