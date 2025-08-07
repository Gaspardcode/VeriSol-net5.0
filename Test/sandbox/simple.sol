// SPDX-License-Identifier: MIT
pragma solidity 0.8.0;

contract SimpleStorage {
    // State variable
    uint256 private storedData;

    // Setter function (no access control)
    function set(uint256 x) public {
        storedData = x;
    }

    // Getter function
    function get() public view returns (uint256) {
        return storedData;
    }
}
