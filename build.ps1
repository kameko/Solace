
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
            $config = (Get-Culture).TextInfo.ToTitleCase($build)
            if (($config -ne "Debug") -and ($config -ne "Release"))
            {
                Write-Output "Build flag must be either debug or release."
            }
            else
            {   
                & dotnet clean Source/Core
                & dotnet build Source/Core --configuration $build

                $folders = Get-ChildItem Modules
                foreach ($module in $folders)
                {
                    & dotnet clean Modules/$module
                    & dotnet build Modules/$module --configuration $build
                }
            }
        }
        elseif ($release -match "publish")
        {
            # TODO: also build modules and copy them to an output folder
            & dotnet publish Source/Core --self-contained --configuration Release
        }
        else 
        {
            Write-Output "Unrecognized release parameter: $release. Use either 'debug' or 'release'."
        }
    }
    
    main
}

& $script
