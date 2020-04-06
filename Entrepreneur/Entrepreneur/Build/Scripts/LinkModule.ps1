if ( Test-Path -Path 'D:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\Entrepreneur' -PathType Container ) { 
    "Module already exists or was linked." 
}
else{    "Linking build with Bannerlord modules."    New-Item -ItemType Junction -Path "D:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\Entrepreneur" -Target "C:\Users\Ricardo\source\repos\ricardoferreirasilva\Entrepreneur\Entrepreneur\Entrepreneur\Build\Entrepreneur"}