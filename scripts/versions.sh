for file in $(ls $1); do grep -oP "pragma solidity [^;]*" $1/$file; done

