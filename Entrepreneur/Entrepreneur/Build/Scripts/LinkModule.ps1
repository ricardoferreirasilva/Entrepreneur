if ( Test-Path -Path 'A:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\Entrepreneur' -PathType Container ) { 
    "Module already exists or was linked." 
}
else{    "Linking build with Bannerlord modules."    New-Item -ItemType Junction -Path "A:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\Entrepreneur" -Target "C:\Users\Ricardo\source\repos\ricardoferreirasilva\Entrepreneur\Entrepreneur\Entrepreneur\Build\Entrepreneur"}