
> az login

> az group create --name ddt2-resource-group --location japaneast

az account list-locations
az account list-locations --query "[].{Region:name}" --out table

> az storage account create --name ddt2functions --resource-group ddt2-resource-group --location japaneast --sku Standard_RAGRS --kind StorageV2

> az storage account keys list --account-name ddt2functions --resource-group ddt2-resource-group --output table

> $env:AZURE_STORAGE_ACCOUNT = "hoge"
> $env:AZURE_STORAGE_KEY = "your_storage_key"

> az storage container create --name excel
> az storage container create --name json
> az storage container create --name pdf

> az functionapp create --consumption-plan-location japaneast --name ddt2functions --resource-group ddt2-resource-group --runtime dotnet --storage-account ddt2functions

> az provider register --namespace Microsoft.EventGrid

> az eventgrid resource event-subscription create -g myResourceGroup \
--provider-namespace Microsoft.Storage --resource-type storageAccounts \
--resource-name myblobstorage12345 --name myFuncSub  \
--included-event-types Microsoft.Storage.BlobCreated \
--subject-begins-with /blobServices/default/containers/images/blobs/ \
--endpoint https://mystoragetriggeredfunction.azurewebsites.net/runtime/webhooks/eventgrid?functionName=imageresizefunc&code=<key>