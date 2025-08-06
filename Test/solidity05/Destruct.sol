pragma solidity ^0.5.0;

contract Destruct {
    function destroy() public {
        // 'suicide' renamed to 'selfdestruct'
        selfdestruct(msg.sender);
    }
}