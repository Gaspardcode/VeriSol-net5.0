pragma solidity ^0.5.0;

contract ConstructorExample {
    address owner;
    
    // Must use 'constructor' keyword
    constructor() public {
        owner = msg.sender;
    }
}