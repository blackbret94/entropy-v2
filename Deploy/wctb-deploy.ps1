# Copy Windows files
$steamDir = "D:\steamworks_sdk_157_WCTB\sdk\tools\ContentBuilder\content\wctb-pt"
$exclude = "Wizard Cats Tank Battle_BackUpThisFolder_ButDontShipItWithYourGame"
$steamBuild = "D:\steamworks_sdk_157_WCTB\sdk\tools\ContentBuilder\run_build.bat"
$sourcePath = "..\Wizard Cats Tank Battle\Builds\Windows\*"

"Copying Windows Files..."
Copy-Item -Path $sourcePath -Destination $steamDir -Exclude $exclude -Recurse -Force

# Run Steam upload bat
"Uploading build..."
& $steamBuild
# Start-Process -FilePath $steamBuild -Wait

# Force user to quit manually
Read-Host -Prompt "Press Enter to exit"