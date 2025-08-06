pragma solidity ^0.5.0;

contract ExternalCall {
    function externalFunc() external pure returns (string memory) {
        return "External!";
    }
    
    function callExternally() public view returns (string memory) {
        // Must use explicit external call syntax
        return this.externalFunc();
    }
}