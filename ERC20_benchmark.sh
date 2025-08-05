for file in $(ls ERC20);
do 
    #solv=$(cat ERC20/$file | grep -oP "pragma solidity \K[^;]+")
    #solc-select use $solv --always-install
    ./verisol.sh ERC20/$file;

done
