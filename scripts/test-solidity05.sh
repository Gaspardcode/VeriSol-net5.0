#!/bin/bash

# Run Solidity 0.5.0 test suite
echo "Running Solidity 0.5.0 test suite..."

dotnet run --project Sources/SolToBoogieTest/SolToBoogieTest.csproj test-solidity05 