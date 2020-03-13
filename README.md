# MAAV - Automated Version Updated Model

Currently, applications are more complex and involve multidisciplinary teams. In this sense, the software development process also needed to adapt to meet users' demands. A very important part of this process is software version control, also called semantic versioning. The purpose of this work is to implement a program that provides an automated way to control version numbers for applications.

## Motivation

Under construction

## Versioning

Under construction

## Getting Started
Before starting see the [Prerequisites](#prerequisites)

Clone repository at project directory and run the scripts below:


```console
$ ./tools/build.sh
```

You can run the command below, is the same operation of above command:

```console
$ dotnet build
```

The output from build:

```console
Microsoft (R) Build Engine version 16.4.0+e901037fe for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restore completed in 34.72 ms for ../src/maav.datacontracts/MAAV.DataContracts.csproj.
  Restore completed in 34.77 ms for ../src/maav.domain/MAAV.Domain.csproj.
  Restore completed in 34.73 ms for ../src/maav.infrastructure.repository.mongodb/MAAV.Infrastructure.Repository.MongoDB.csproj.
  Restore completed in 39.9 ms for ../src/maav.infrastructure.repository.litedb/MAAV.Infrastructure.Repository.LiteDB.csproj.
  Restore completed in 117.17 ms for ../src/maav.webapi/MAAV.WebAPI.csproj.
  Restore completed in 175.28 ms for ../tests/maav.application.test/MAAV.Application.Test.csproj.
  Restore completed in 176.79 ms for ../tests/maav.webapi.test/MAAV.WebAPI.Test.csproj.
  Restore completed in 182.28 ms for ../src/maav.application/MAAV.Application.csproj.
  MAAV.Infrastructure.Repository.MongoDB -> ../src/maav.infrastructure.repository.mongodb/bin/Debug/netstandard2.1/MAAV.Infrastructure.Repository.MongoDB.dll
  MAAV.Domain -> ../src/maav.domain/bin/Debug/netstandard2.1/MAAV.Domain.dll
  MAAV.DataContracts -> ../src/maav.datacontracts/bin/Debug/netstandard2.1/MAAV.DataContracts.dll
  MAAV.Infrastructure.Repository.LiteDB -> ../src/maav.infrastructure.repository.litedb/bin/Debug/netstandard2.1/MAAV.Infrastructure.Repository.LiteDB.dll
  MAAV.Application -> ../src/maav.application/bin/Debug/netstandard2.1/MAAV.Application.dll
  MAAV.Application.Test -> ../tests/maav.application.test/bin/Debug/netcoreapp3.1/MAAV.Application.Test.dll
  MAAV.WebAPI -> ../src/maav.webapi/bin/Debug/netcoreapp3.1/MAAV.WebAPI.dll
  MAAV.WebAPI.Test -> ../tests/maav.webapi.test/bin/Debug/netcoreapp3.1/MAAV.WebAPI.Test.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.00
```

The command will build the project and you can check project is not broken!

To run the project, you need check some configuration before this step, open the file `launchSettings.json`, the location of this file is `src/maav.webapi/properties`, and check the settings for debug:
```json
"MAAV.WebAPI": {
  "commandName": "Project",
  "launchBrowser": true,
  "environmentVariables": {
    "ASPNETCORE_URLS": "http://*:$YOUR_PORT",
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```
This block contains a part of the configuration to run the project, the `$YOUR_PORT` is the listen port of the application, choose some available port(The default value is 5892).
After that, you can run the project:

```console
$ ./tools/run.sh
```

Or you can run...


```console
$ dotnet run --project src/maav.webapi/
```

The output will be:
```console
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://[::]:5892
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: .../src/maav.webapi
info: Microsoft.Hosting.Lifetime[0]
      Application is shutting down...
```
The both command has the output and operation. But you can run the project in docker container with docker-compose:

```console
$ ./tools/run.sh docker
```

Or you can run...

```console
$ docker-compose up --build
```

The output from docker command execution:

```console
mmaav.api           |info: Microsoft.AspNetCore.DataProtection.Repositories.FileSystemXmlRepository[60]
mmaav.api           |      Storing keys in a directory '/root/.aspnet/DataProtection-Keys' that may not be persisted outside of the container. Protected data will be unavailable when container is destroyed.
mmaav.api           |info: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[35]
mmaav.api           |      No XML encryptor configured. Key {5d98046b-0c73-45ae-8db8-e661d8b7a7f4} may be persisted to storage in unencrypted form.
mmaav.api           |info: Microsoft.Hosting.Lifetime[0]
mmaav.api           |      Now listening on: http://[::]:80
mmaav.api           |info: Microsoft.Hosting.Lifetime[0]
mmaav.api           |      Application started. Press Ctrl+C to shut down.
mmaav.api           |info: Microsoft.Hosting.Lifetime[0]
mmaav.api           |      Hosting environment: Production
mmaav.api           |info: Microsoft.Hosting.Lifetime[0]
mmaav.api           |      Content root path: /app
```

### Prerequisites
- [.Net Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [Docker](https://docs.docker.com/install)
- [MongoDB](https://docs.mongodb.com/manual/installation)


## Authors

* **Augusto Mesquita** - [Perfil](https://github.com/AugustoDeveloper)
