$majorVersion=1
$minorVersion=0
$buildVersion=18
$postfixVersion="-beta"
$version="$majorVersion.$minorVersion.$buildVersion$postfixVersion"

$authors="Kevin Ackerman"
$copyright="MIT"
$repoUrl="https://github.com/kevinarthurackerman/RemotiatR"

$buildOrder= `
    "variables-remotiatr-core-shared", `
    "variables-remotiatr-core-client", `
    "variables-remotiatr-core-server", `
    "variables-remotiatr-serializer-json-shared", `
    "variables-remotiatr-serializer-json-client", `
    "variables-remotiatr-serializer-json-server", `
    "variables-remotiatr-messagetransport-http-shared", `
    "variables-remotiatr-messagetransport-http-client", `
    "variables-remotiatr-messagetransport-http-server", `
    "variables-remotiatr-fluentvalidation-shared", `
    "variables-remotiatr-fluentvalidation-client", `
    "variables-remotiatr-fluentvalidation-server"