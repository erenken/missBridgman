param(
    [string] $ResourceGroupName = 'missBridgman',
    [string] $StorageAccountName = 'missbridgman',
    [string] $ContainerName = 'images'
)

$ErrorActionPreference = 'Stop'

function ConvertTo-MetadataValue([string] $Value) {
    $encoded = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($Value))
    return "b64:$encoded"
}

$storageKey = az storage account keys list --resource-group $ResourceGroupName --account-name $StorageAccountName --query '[0].value' --output tsv
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($storageKey)) {
    throw 'Could not obtain a storage key. Check Azure sign-in and Storage Account permissions.'
}

$knownPhotos = @{
    '2019/01-Miss Bridgman.jpg' = @{ name = 'Krystin Buist'; title = 'Miss Bridgman'; category = 'Miss Bridgman' }
    '2019/02-Miss Bridgman.jpg' = @{ name = 'Krystin Buist'; title = 'Miss Bridgman'; category = 'Miss Bridgman' }
    '2019/03-Little Miss Bridgman.jpg' = @{ name = 'Dempsey DeMercado & LaShanna Sparks'; title = 'Little Miss Bridgman'; category = 'Little Miss Bridgman' }
    '2019/04-Mr Nice Guy.jpg' = @{ name = 'Justin Jannert'; title = 'Mr Nice Guy'; category = 'Mr Nice Guy' }
}

try {
    $blobsJson = az storage blob list --account-name $StorageAccountName --account-key $storageKey --container-name $ContainerName --query '[].name' --output json
    if ($LASTEXITCODE -ne 0) { throw 'Could not list image blobs.' }
    $blobNames = @($blobsJson | ConvertFrom-Json)

    foreach ($blobName in $blobNames) {
        $parts = $blobName -split '/', 2
        if ($parts.Count -ne 2 -or $parts[0] -notmatch '^\d{4}$') {
            Write-Warning "Skipping blob without a year folder: $blobName"
            continue
        }

        $filename = [System.IO.Path]::GetFileNameWithoutExtension($parts[1]) -replace '^\d+-', ''
        $name = $filename
        $title = $filename
        $category = 'Court'
        if ($knownPhotos.ContainsKey($blobName)) {
            $name = $knownPhotos[$blobName].name
            $title = $knownPhotos[$blobName].title
            $category = $knownPhotos[$blobName].category
        }

        $metadata = @(
            "name=$(ConvertTo-MetadataValue $name)"
            "title=$(ConvertTo-MetadataValue $title)"
            "year=$($parts[0])"
            "description=$(ConvertTo-MetadataValue '')"
            "category=$(ConvertTo-MetadataValue $category)"
        )
        $arguments = @(
            'storage', 'blob', 'metadata', 'update',
            '--account-name', $StorageAccountName,
            '--account-key', $storageKey,
            '--container-name', $ContainerName,
            '--name', $blobName,
            '--metadata'
        ) + $metadata + @('--output', 'none')
        & az @arguments
        if ($LASTEXITCODE -ne 0) { throw "Metadata update failed for $blobName" }
        Write-Host "Updated metadata: $blobName"
    }
}
finally {
    $storageKey = $null
}
