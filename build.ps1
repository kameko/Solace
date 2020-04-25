
# NOTE: This only needs to be run on the first build or when new projects
# are introduced to the solution.

param (
     [string]$build   = "debug"
    ,[string]$release = "debug"
)

$script = {
    
    function main
    {
        & dotnet restore
        
        if ($release -match "debug")
        {
            & dotnet build
        }
        elseif ($release -match "publish")
        {
            # TODO: run dotnet publish and put deployable output into it's own folder.
            & dotnet build
        }
        else 
        {
            Write-Output "Unrecognized release parameter: $release. Use either 'debug' or 'publish'."
        }
    }
    
    main
}

& $script
