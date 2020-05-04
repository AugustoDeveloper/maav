$currentCommit = $(commitBuild) #'026498cfdad1c72f756e9bdf89d2804372947afd'
$endpointCommit = '$(Api:BaseUri)/$(OrgId)/teams/$(TeamCode)/apps/$(AppId)/webhook/$currentCommit'#'http://localhost:5892/api/v1/unicarioca/teams/tcc/apps/5eb052746ecf403e989d1cf9/webhook/$currentCommit'
$endpointAuth = '$(Api:BaseUri)/$(OrgId)/authenticate' #'http://localhost:5892/api/v1/unicarioca/authenticate'
$endpointVersion = '$(baseUri)/$(OrgId)/teams/$(TeamCode)/apps/$(AppId)/version' #'http://localhost:5892/api/v1/unicarioca/teams/tcc/apps/5eb052746ecf403e989d1cf9/version'
$currentBranchName = $(commitBranchName)
$preRelease = 'alpha'
$currentRevision = $(revision)
$accessToken = ''
$username = $(User:Integration)
$password = $(Password:Integration)

Write-Host "Commit: $currentCommit"
Write-Host "Endpoint: $endpointCommit"
Write-Host "Endpoint: $endpointAuth"
Write-Host "Endpoint: $endpointVersion"
Write-Host "Branch: $currentBranchName"
Write-Host "Branch: $currentBranchName"
Write-Host "Revision: $currentRevision"

$auth = @{
    Username = $username
    Password = $password
} | ConvertTo-Json

$result = Invoke-RestMethod -Method 'POST' -Body $auth -Uri "$endpointAuth" -ContentType 'application/json' -ErrorAction Stop

$accessToken = $result.accessToken

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

    Start-Sleep 1000
}

$headers = @{Authorization = "Bearer $accessToken"}

$result = Invoke-RestMethod -Method Post -Body $Body -Uri "$endpointVersion" -Headers $headers -ContentType 'application/json' -ErrorAction Stop
$result
$version = '$result.major.$result.minor.$result.patch.$result.build'
Write-Host "##vso[build.updatebuildnumber]$version"