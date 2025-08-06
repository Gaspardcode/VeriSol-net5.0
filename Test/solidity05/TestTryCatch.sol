
pragma solidity ^0.5.0;

contract TestTryCatch {
    function testTryCatch() public returns (bool) {
        try this.externalFunction() returns (bool success) {
            return success;
        } catch {
            return false;
        }
    }
    
    function externalFunction() external returns (bool) {
        return true;
    }
}