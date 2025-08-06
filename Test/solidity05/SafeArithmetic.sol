pragma solidity ^0.5.0;

contract SafeArithmetic {
    function safeAdd() public pure returns (uint16) {
        uint8 a = 100;
        uint16 b = 30000;
        
        // Explicit conversion required
        return uint16(a) + b; 
    }
}