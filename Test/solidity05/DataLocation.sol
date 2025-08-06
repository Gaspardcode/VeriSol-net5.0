pragma solidity ^0.5.0;

contract DataLocation {
    struct Item { uint id; string name; }
    Item[] public items;
    
    // Must specify memory location for array param
    function addItems(Item[] memory _newItems) public {
        for(uint i = 0; i < _newItems.length; i++) {
            items.push(_newItems[i]);
        }
    }
    
    // Must specify memory location for return type
    function getItem(uint index) public view returns (Item memory) {
        return items[index];
    }
}