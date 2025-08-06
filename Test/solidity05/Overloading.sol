pragma solidity ^0.5.0;

contract Overloading {
    // Allowed: Different parameter types
    function getValue(uint a) public pure returns (uint) { return a; }
    function getValue(string memory b) public pure returns (string memory) { return b; }
    
    // NOT ALLOWED: Same parameters different return types
    // function getValue(uint a) public pure returns (string memory) { ... }
}