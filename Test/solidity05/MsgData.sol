pragma solidity ^0.5.0;

contract MsgData {
    function getData() public view returns (bytes memory) {
        return msg.data; // Now returns bytes instead of bytes4
    }
}