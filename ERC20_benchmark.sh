for file in $(ls ERC20);
do 
    #solv=$(cat ERC20/$file | grep -oP "pragma solidity \K[^;]+")
    #solc-select use $solv --always-install
    ./verisol.sh ERC20/$file

done

for file in $(ls *.bpl);
do
    /home/g_code/VeriSol-net5.0/Sources/VeriSol/bin/Debug/boogie -doModSetAnalysis -inline:spec  -inlineDepth:4 -proc:BoogieEntry_* $file
done
