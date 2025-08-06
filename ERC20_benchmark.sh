for file in $(ls ERC20);
do 
    #solv=$(cat ERC20/$file | grep -oP "pragma solidity \K[^;]+")
    #solc-select use $solv --always-install
    output=$(./verisol.sh ERC20/$file)
    if [ $(echo $output | grep -c "error") -ne 0 ]; then
        echo $(echo $output | grep "error")
        echo "$file has errors"
        echo "$file" >> errors.txt
    else
        echo "$file has no errors"
        echo "$file" >> success.txt
    fi

done

# for later, maybe run boogie on the bpl files
for file in $(ls *.bpl);
do
    home/g_code/VeriSol-net5.0/Sources/VeriSol/bin/Debug/boogie -doModSetAnalysis -inline:spec  -inlineDepth:4 -proc:BoogieEntry_* $file
done
