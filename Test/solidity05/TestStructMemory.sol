
pragma solidity ^0.5.0;

contract TestStructMemory {
    struct Person {
        string name;
        uint256 age;
    }
    
    function createPerson(string memory name, uint256 age) public pure returns (Person memory) {
        return Person(name, age);
    }
}