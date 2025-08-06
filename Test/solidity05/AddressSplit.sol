pragma solidity ^0.5.0;

contract AddressSplit {
    // Only payable addresses can receive Ether
    function sendEther(address payable _recipient) public payable {
        _recipient.transfer(msg.value);
    }
    
    function convert(address _addr) public pure returns (address payable) {
        return payable(_addr); // Explicit conversion required
    }
}