$currentCommit = '026498cfdad1c72f756e9bdf89d2804372947afd' #$(Build.SourceVersion)
$endpointCommit = 'https://maav-webapi.herokuapp.com/api/v1/unicarioca/teams/tcc/apps/5eb052746ecf403e989d1cf9/webhook/$currentCommit' #$(endpoint)
$endpointAuth = 'https://maav-webapi.herokuapp.com/api/v1/unicarioca/authenticate'
$endpointVersion = 'https://maav-webapi.herokuapp.com/api/v1/unicarioca/teams/tcc/apps/5eb052746ecf403e989d1cf9/version'
$currentBranchName = 'master'#$(Build.SourceVersion)
$preRelease = 'alpha'
$currentRevision = '01'
$accessToken = ''
$username = 'azdevops-user'
$password = 'azdvps-ntgrt'

$auth = @{
    Username = $username
    Password = $password
} | ConvertTo-Json

$result = Invoke-RestMethod -Method Post -Body $auth -Uri "$endpointAuth" -ContentType 'application/json' -ErrorAction Stop

$accessToken = $result.accessToken

#$response = Invoke-WebRequest -Method 'POST' -Body $Body -Uri "$(endpointVersion)" -UseBasicParsing -ErrorAction Stop
Write-Host "Commit: $currentCommit"
Write-Host "Endpoint: $endpointCommit"
$body = @{
    From = $currentBranchName
    To= ''
    Commit = $currentCommit
    Message = 'Automated Build Run Message'
    PreReleaseLabel = $preRelease
    BuildLabel = $currentRevision
} | ConvertTo-Json

For ($i=1; $i -lt 5; $i++)
{
    try
    {
        $response = Invoke-RestMethod -Method Get -Uri "$endpointCommit" -ErrorAction Stop
        $statusCode = $response.StatusCode
        $body = @{
            From = $response.fromBranch
            To= $response.toBranch
            Commit = $currentCommit
            Message = $response.commitMessag
            PreReleaseLabel = $preRelease
            BuildLabel = $currentRevision
        } | ConvertTo-Json

        Write-Host "Status: $statusCode"
        $response
        break
    }
    catch
    {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "[$i]Status: $statusCode"
    }

    Start-Sleep 1
}

$headers = @{Authorization = "Bearer $accessToken"}

$result = Invoke-RestMethod -Method Post -Body $Body -Uri "$endpointVersion" -Headers $headers -ContentType 'application/json' -ErrorAction Stop
$major = $result.major;
$minor = $result.minor;
$patch = $result.patch;
$preRelease = $result.preRelease;
$build = $result.build;
$version = "$major.$minor.$patch$preRelease$build"
Write-Host "$version"
#Write-Host "##vso[build.updatebuildnumber]$version