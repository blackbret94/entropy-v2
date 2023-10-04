# Login
./azcopy.exe login --tenant-id aa9e8c99-b285-4c4e-a2c2-e1c46f5aa170

# Upload to staging
./azcopy.exe copy './Wizard Cats Tank Battle/Assets/Bundles' "https://wizardcatsstorage.blob.core.windows.net/wizardcatstankbattle-dev"

# Upload to prod
./azcopy.exe copy './Wizard Cats Tank Battle/Assets/Bundles' "https://wizardcatsstorage.blob.core.windows.net/wizardcatstankbattle"

# Force user to quit manually
Read-Host -Prompt "Press Enter to exit"