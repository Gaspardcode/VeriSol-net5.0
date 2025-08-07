contract_name=$(echo "$1" | grep -oP "([^/]+/)+\K.*(?=_)" )
echo $contract_name
dotnet Sources/VeriSol/bin/Debug/VeriSol.dll "$1" "$contract_name"