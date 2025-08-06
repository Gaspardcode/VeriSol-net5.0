
pragma solidity ^0.5.0;

contract TestCustomErrors {
    uint256 public balance;
    
    function withdraw(uint256 amount) public {
        require(amount <= balance, "Insufficient balance");
        balance -= amount;
    }
}