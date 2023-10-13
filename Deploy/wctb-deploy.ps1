# Copy Windows files
$steamDir = "G:\My Drive\Vashta Entertainment\steamworks_sdk_157_WCTB\sdk\tools\ContentBuilder\content\wctb-pt"
$exclude = "Wizard Cats Tank Battle_BackUpThisFolder_ButDontShipItWithYourGame"
$steamBuild = "G:\My Drive\Vashta Entertainment\steamworks_sdk_157_WCTB\sdk\tools\ContentBuilder\run_build.bat"
"Copying Windows Files..."
$sourcePath = "Wizard Cats Tank Battle\Builds\Windows\*"

Copy-Item -Path $sourcePath -Destination $steamDir -Exclude $exclude -Recurse -Force
# Run Steam upload bat
# "Uploading build..."
# Start-Process -FilePath $steamBuild
# Run AZCopy
# Upload for Android
# Force user to quit manually
Read-Host -Prompt "Press Enter to exit"