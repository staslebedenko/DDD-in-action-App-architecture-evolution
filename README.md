# DDD in action. Evolution of application architecture and complexity


## Step 0. Azure infrastructure.


```

subscriptionID=$(az account list --query "[?contains(name,'Microsoft')].[id]" -o tsv)
echo "Test subscription ID is = " $subscriptionID
az account set --subscription $subscriptionID
az account show

location=northeurope
postfix=$RANDOM

#----------------------------------------------------------------------------------
# Main group .
#----------------------------------------------------------------------------------

groupName=fwdaysDddAction$postfix
az group create --name $groupName --location $location

#----------------------------------------------------------------------------------
# Storage account with Blob container
#----------------------------------------------------------------------------------

location=northeurope
accountSku=Standard_LRS
accountName=${groupName,,}
echo "accountName  = " $accountName

az storage account create --name $accountName --location $location --kind StorageV2 \
--resource-group $groupName --sku $accountSku --access-tier Hot  --https-only true
  
storageAccountKey=$(az storage account keys list --resource-group $groupName --account-name $accountName --query "[0].value" | tr -d '"')
ordersAccConString="DefaultEndpointsProtocol=https;AccountName=$accountName;AccountKey=$storageAccountKey;EndpointSuffix=core.windows.net"
   
requestQueue=balance-requests
resultQueue=factory-requests
errorQueue=failed-orders
rollbackSuccessQueue=rollback-success
compensatingTable=compensatingCommands
resultTable=processedOrders
ordersQueue=received-orders

az storage queue create --name $ordersQueue --account-key $storageAccountKey \
--account-name $accountName --connection-string $ordersAccConString

az storage queue create --name $requestQueue --account-key $storageAccountKey \
--account-name $accountName --connection-string $ordersAccConString

az storage queue create --name $resultQueue --account-key $storageAccountKey \
--account-name $accountName --connection-string $ordersAccConString

az storage queue create --name $errorQueue --account-key $storageAccountKey \
--account-name $accountName --connection-string $ordersAccConString

az storage queue create --name $rollbackSuccessQueue --account-key $storageAccountKey \
--account-name $accountName --connection-string $ordersAccConString

az storage table create --name $compensatingTable --account-key $storageAccountKey \
--account-name $accountName --connection-string $ordersAccConString

az storage table create --name $resultTable --account-key $storageAccountKey \
--account-name $accountName --connection-string $ordersAccConString

#----------------------------------------------------------------------------------
# Function app with Application insights(created ayutomatically)
#----------------------------------------------------------------------------------

applicationName=${groupName,,}
accountName=${groupName,,}
echo "applicationName  = " $applicationName

az functionapp create --resource-group $groupName \
--name $applicationName --storage-account $accountName \
--consumption-plan-location $location --functions-version 3

az functionapp update --resource-group $groupName --name $applicationName --set dailyMemoryTimeQuota=400000
az functionapp config appsettings set --resource-group $groupName --name $applicationName --settings "MSDEPLOY_RENAME_LOCKED_FILES=1"
az functionapp config appsettings set --resource-group $groupName --name $applicationName --settings ASPNETCORE_ENVIRONMENT=Production
az functionapp config appsettings set --resource-group $groupName --name $applicationName --settings "StorageConnectionString=$ordersAccConString"

#----------------------------------------------------------------------------------
# Azure SQL Server and Serverless DB 1-4 cores and 32 Gb storage
#----------------------------------------------------------------------------------

location=northeurope
serverName=${groupName,,}
adminLogin=Admin$groupName
password=Sup3rStr0ng$groupName$postfix
databaseName=${groupName,,}
serverSku=S0
catalogCollation="SQL_Latin1_General_CP1_CI_AS"

az sql server create --name $serverName --resource-group $groupName --assign-identity \
--location $location --admin-user $adminLogin --admin-password $password

az sql db create --resource-group $groupName --server $serverName --name $databaseName \
--edition GeneralPurpose --family Gen5 --compute-model Serverless \
--auto-pause-delay 60 --capacity 4

outboundIps=$(az webapp show --resource-group $groupName --name $applicationName --query possibleOutboundIpAddresses --output tsv)
IFS=',' read -r -a ipArray <<< "$outboundIps"

for ip in "${ipArray[@]}"
do
echo "$ip add"
az sql server firewall-rule create --resource-group $groupName --server $serverName \
--name "WebApp$ip" --start-ip-address $ip --end-ip-address $ip
done

sqlClientType=ado.net
sqlConString=$(az sql db show-connection-string --name $databaseName --server $serverName --client $sqlClientType --output tsv)
sqlConString=${sqlConString/Password=<password>;}
sqlConString=${sqlConString/<username>/$adminLogin}

orderDbPassword=$password

#----------------------------------------------------------------------------------


#----------------------------------------------------------------------------------
# Additional group with second app
#----------------------------------------------------------------------------------

deliveryGroupName=tpaperDeliveryUA$postfix
az group create --name $deliveryGroupName --location $location

#----------------------------------------------------------------------------------
# Storage account
#----------------------------------------------------------------------------------

location=northeurope
accountSku=Standard_LRS
deliveryAccountName=${deliveryGroupName,,}
echo "accountName  = " $deliveryAccountName

az storage account create --name $deliveryAccountName --location $location --kind StorageV2 \
--resource-group $deliveryGroupName --sku $accountSku --access-tier Hot  --https-only true
  
deliveryStorageAccountKey=$(az storage account keys list --resource-group $deliveryGroupName --account-name $deliveryAccountName --query "[0].value" | tr -d '"')
deliveryAccConString="DefaultEndpointsProtocol=https;AccountName=$deliveryAccountName;AccountKey=$storageAccountKey;EndpointSuffix=core.windows.net"

#----------------------------------------------------------------------------------
# Function app with Application insights(created and connected automatically)
#----------------------------------------------------------------------------------

deliveryApplicationName=${deliveryGroupName,,}
echo "applicationName  = " $deliveryApplicationName

az functionapp create --resource-group $deliveryGroupName \
--name $deliveryApplicationName --storage-account $deliveryAccountName \
--consumption-plan-location $location --functions-version 3

az functionapp update --resource-group $deliveryGroupName --name $deliveryApplicationName --set dailyMemoryTimeQuota=400000
az functionapp config appsettings set --resource-group $deliveryGroupName --name $deliveryApplicationName --settings "MSDEPLOY_RENAME_LOCKED_FILES=1"
az functionapp config appsettings set --resource-group $deliveryGroupName --name $deliveryApplicationName --settings ASPNETCORE_ENVIRONMENT=Production
az functionapp config appsettings set --resource-group $deliveryGroupName --name $deliveryApplicationName --settings "StorageConnectionString=$deliveryAccConString"

#----------------------------------------------------------------------------------
# Azure SQL Server and Serverless DB 1-4 cores and 32 Gb storage
#----------------------------------------------------------------------------------

location=northeurope
serverName=${deliveryGroupName,,}
adminLogin=Admin$deliveryGroupName
password=Sup3rStr0ng$deliveryGroupName$postfix
databaseName=${deliveryGroupName,,}
serverSku=S0
catalogCollation="SQL_Latin1_General_CP1_CI_AS"

az sql server create --name $serverName --resource-group $deliveryGroupName --assign-identity \
--location $location --admin-user $adminLogin --admin-password $password

az sql db create --resource-group $deliveryGroupName --server $serverName --name $databaseName \
--edition GeneralPurpose --family Gen5 --compute-model Serverless \
--auto-pause-delay 60 --capacity 4

outboundIps=$(az webapp show --resource-group $deliveryGroupName --name $deliveryApplicationName --query possibleOutboundIpAddresses --output tsv)
IFS=',' read -r -a ipArray <<< "$outboundIps"

for ip in "${ipArray[@]}"
do
echo "$ip add"
az sql server firewall-rule create --resource-group $deliveryGroupName --server $serverName \
--name "WebApp$ip" --start-ip-address $ip --end-ip-address $ip
done

sqlClientType=ado.net
sqlConStringDelivery=$(az sql db show-connection-string --name $databaseName --server $serverName --client $sqlClientType --output tsv)
sqlConStringDelivery=${sqlConStringDelivery/Password=<password>;}
sqlConStringDelivery=${sqlConStringDelivery/<username>/$adminLogin}

deliveryDbPassword=$password

#----------------------------------------------------------------------------------

#----------------------------------------------------------------------------------
# Strings for local variables
#----------------------------------------------------------------------------------

printf "\n\nRun string below in local cmd prompt to assign secret to environment variable SqlPaperString:\nsetx SqlPaperString \"$sqlConString\"\n\n"
printf "\n\nRun string below in local cmd prompt to assign secret to environment variable SqlPaperPassword:\nsetx SqlPaperPassword \"$orderDbPassword\"\n\n"

printf "\n\nRun string below in local cmd prompt to assign secret to environment variable SqlPaperDeliveryString:\nsetx SqlPaperDeliveryString \"$sqlConStringDelivery\"\n\n"
printf "\n\nRun string below in local cmd prompt to assign secret to environment variable SqlPaperDeliveryPassword:\nsetx SqlPaperDeliveryPassword \"$deliveryDbPassword\"\n\n"

printf "\n\nRun string below in local cmd prompt to assign secret to environment variable ordersAccConString:\nsetx OrdersAccConString \"$ordersAccConString\"\n\n"
printf "\n\nRun string below in local cmd prompt to assign secret to environment variable deliveryAccConString:\nsetx DeliveryAccConString \"$deliveryAccConString\"\n\n"


```




