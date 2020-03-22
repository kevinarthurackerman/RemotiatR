
# RemotiatR Contributor Setup
This file details setting up your developer environment to have the most productive experience possible

### Getting Started

- Clone the repo at `https://github.com/kevinarthurackerman/RemotiatR.git`
- Ensure that you have a local instance of SQL Server installed and running
    - SQL Server and SQL Server Express can be downloaded [here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Test build and run to make sure that everything works. You can use the [example server project](https://github.com/kevinarthurackerman/RemotiatR/blob/master/src/Example/Server/ContosoUniversity.Server.csproj) as your startup project
    - The example server project uses a pre-build event to automatically build the database for you

### Making Changes

To support easy extensibility this code makes heavy use of nuget packages. This can create some friction in development, so the code include scripts to make it easier to deploy your changes locally as you code

- After verifying that everything builds and before creating any changes open one of the project files and look for a reference to a RemotiatR package (ex: `<PackageReference Include="RemotiatR.abc.Client/Server" Version="x.y.z-beta" />`)
- Perform a find-and-replace to increment all the version numbers from `Version="x.y.z-beta"` to `Version="x.y.v-beta"` where v = z+1
- Increment the `$buildVersion` version number in [/scripts/variables.ps1](https://github.com/kevinarthurackerman/RemotiatR/blob/master/scripts/variables.ps1)
    - By incrementing the version number you ensure that you are always running against the local packages
- Run the [/scripts/pack.ps1](https://github.com/kevinarthurackerman/RemotiatR/blob/master/scripts/pack.ps1) script to automatically rebuild all of the packages
- Run the program again to ensure that it locates your new packages and builds
- If you add new extension projects, be sure to add them to the pack script by adding a variables file to [/scripts](https://github.com/kevinarthurackerman/RemotiatR/tree/master/scripts), and adding a reference to that file to the `$buildOrder` in [/scripts/variables.ps1](https://github.com/kevinarthurackerman/RemotiatR/blob/master/scripts/variables.ps1)
- After making changes make sure to rerun the pack script each time before running to pick up your changes
- Before merging to master make sure that the version is incremented to the latest version+1 and that the packages have been deployed to nuget so that other users who pull the repo will have access to them