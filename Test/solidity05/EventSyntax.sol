pragma solidity ^0.5.0;

contract EventSyntax {
    event ValueChanged(address indexed changer, uint newValue);
    
    uint public value;
    
    function setValue(uint _val) public {
        value = _val;
        emit ValueChanged(msg.sender, _val); // 'emit' keyword required
    }
}