
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
                & dotnet clean "Source/Solace Core"
                & dotnet build "Source/Solace Core" --configuration $build

                $folders = Get-ChildItem "Solace Modules"
                foreach ($module in $folders)
                {
                    & dotnet clean "Solace Modules/$module"
                    & dotnet build "Solace Modules/$module" --configuration $build
                }
            }
        }
        elseif ($release -match "publish")
        {
            # TODO: also build modules and copy them to an output folder
            & dotnet publish "Source/Solace Core" --self-contained --configuration Release
        }
        else 
        {
            Write-Output "Unrecognized release parameter: $release. Use either 'debug' or 'release'."
        }
    }
    
    main
}

& $script
