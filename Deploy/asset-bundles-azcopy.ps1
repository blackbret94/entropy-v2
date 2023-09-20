# Login
azcopy login

# Upload to staging
azcopy copy '.\Wizard Cats Tank Battle\Assets\Bundles' "https://wizardcatsstorage.blob.core.windows.net/wizardcatstankbattle-dev"

# Upload to prod
azcopy copy '.\Wizard Cats Tank Battle\Assets\Bundles' "https://wizardcatsstorage.blob.core.windows.net/wizardcatstankbattle"
