# VeriSol Setup Guide

## Setup Instructions

### 1. Environment Setup
```bash

./scripts/dotnet-install.sh --runtime 8.0

# Add .NET to PATH (add to ~/.bashrc for persistence)
export PATH=$PATH:/home/g_code/.dotnet

# Verify installation
dotnet --version  # Should show 6.0.428
```
### 3. Build the Project
```bash

# Build the solution
dotnet build Sources/VeriSol.sln
```

### 4. Test the build
```bash
# Run the test script
./scripts/assert_software_is_working.sh

```


## Architecture

### **Core Components:**

```
Sources/
├── VeriSol/              # Main application 
├── SolToBoogie/          # Translation engine
├── SolidityAST/          # Solidity AST parsing
├── BoogieAST/            # Boogie AST generation
├── ExternalToolsManager/  # Solc management
└── SolToBoogieTest/      # Test suite 
```

### **Test Structure:**

```
Test/
├── regressions/          # Solidity test files
├── config/              # Test configurations (unused)
└── records.txt          # Test enable/disable list
```

## Usage Examples

```bash
dotnet Sources/VeriSol/bin/Debug/VeriSol.dll contract.sol ContractName
```