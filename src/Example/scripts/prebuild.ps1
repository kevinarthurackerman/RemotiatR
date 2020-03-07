dotnet tool install -g dotnet-roundhouse
rh /d=RemotiatRContosoUniversity /f=..\Server\App_Data /s="(LocalDb)\mssqllocaldb" /silent
rh /d=RemotiatRContosoUniversity-Test /f=..\Server\App_Data /s="(LocalDb)\mssqllocaldb" /silent /drop
rh /d=RemotiatRContosoUniversity-Test /f=..\Server\App_Data /s="(LocalDb)\mssqllocaldb" /silent /simple