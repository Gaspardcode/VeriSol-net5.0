pragma solidity ^0.5.0;

contract Visibility {
    uint public stateVar; // Explicit public visibility required
    
    // Constructor visibility required
    constructor() public {
        stateVar = 1;
    }
    
    // Function visibility required
    function setVar(uint _val) public {
        stateVar = _val;
    }
}