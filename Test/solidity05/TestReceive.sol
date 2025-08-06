
pragma solidity ^0.5.0;

contract TestReceive {
    uint256 public received;
    
    function() external payable {
        received += msg.value;
    }
}