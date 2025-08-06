pragma solidity ^0.5.0;

contract LoopRestrictions {
    mapping(uint => address) public accounts;
    
    function validate() public view {
        // Can't iterate over mapping - must use fixed array length
        uint[10] memory temp;
        for(uint i = 0; i < temp.length; i++) {
            // Requires known array length
        }
    }
}