# VeriSol Setup Guide

## Setup Instructions

### 1. Environment Setup
```bash
# Add .NET to PATH (add to ~/.bashrc for persistence)
export PATH=$PATH:/home/g_code/.dotnet

# Verify installation
dotnet --version  # Should show 6.0.428
```

### 2. Build the Project
```bash
# Navigate to project root
cd /home/g_code/VeriSol-net5.0

# Build the solution
dotnet build Sources/VeriSol.sln
```

### 3. Install External Tool Runtimes
The external tools (Boogie, Corral) need .NET runtime:

```bash
# Install .NET runtime for external tools
./dotnet-install.sh --runtime 6.0

# Or install system-wide
sudo apt install dotnet-runtime-6.0
```

### 4. Test the Installation
```bash
# Run the test script
./test_verisol.sh

# Or run manually
dotnet Sources/VeriSol/bin/Debug/VeriSol.dll Test/regressions/AssertTrue.sol AssertTrue
```

## Architecture

### **Core Components:**

```
Sources/
├── VeriSol/              # Main application 
├── SolToBoogie/          # Translation engine
├── SolidityAST/          # Solidity AST parsing
├── BoogieAST/            # Boogie AST generation
├── SolidityCFG/          # Control flow graph 
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

```bash
dotnet Sources/SolToBoogieTest/bin/Debug/SolToBoogieTest.dll Test/
```