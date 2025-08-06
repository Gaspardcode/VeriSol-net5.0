pragma solidity ^0.5.0;

contract SafeTransfer {
    function sendViaCall(address payable _to) public payable {
        (bool success, ) = _to.call.value(msg.value)("");
        require(success, "Transfer failed");
    }
}