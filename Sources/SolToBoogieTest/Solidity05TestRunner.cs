using System;
using System.IO;
using System.Collections.Generic;
using SolidityAST;
using BoogieAST;
using SolToBoogie;
using VeriSolRunner.ExternalTools;

namespace SolToBoogieTest
{
    public class Solidity05TestRunner
    {
        private readonly string testDir = "Test/solidity05";
        private readonly string solcPath;

        public Solidity05TestRunner()
        {
            solcPath = ExternalToolsManager.Solc.Command;
        }

        public void RunAllTests()
        {
            Console.WriteLine("=== Solidity 0.5.0 Test Suite ===");
            
            TestCustomErrors();
            TestTryCatch();
            TestReceiveFunction();
            TestStructMemory();
            
            Console.WriteLine("=== Test Suite Complete ===");
        }

        private void TestCustomErrors()
        {
            Console.WriteLine("\n--- Testing Custom Errors ---");
            var contract = @"
pragma solidity ^0.5.0;

contract TestCustomErrors {
    uint256 public balance;
    
    function withdraw(uint256 amount) public {
        require(amount <= balance, ""Insufficient balance"");
        balance -= amount;
    }
}";
            
            var result = TranslateAndVerify(contract, "TestCustomErrors");
            Console.WriteLine($"Custom Errors: {(result.Success ? "PASS" : "FAIL")} - {result.Error}");
        }

        private void TestTryCatch()
        {
            Console.WriteLine("\n--- Testing Try-Catch ---");
            var contract = @"
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
}";
            
            var result = TranslateAndVerify(contract, "TestTryCatch");
            Console.WriteLine($"Try-Catch: {(result.Success ? "PASS" : "FAIL")} - {result.Error}");
        }

        private void TestReceiveFunction()
        {
            Console.WriteLine("\n--- Testing Receive Function ---");
            var contract = @"
pragma solidity ^0.5.0;

contract TestReceive {
    uint256 public received;
    
    function() external payable {
        received += msg.value;
    }
}";
            
            var result = TranslateAndVerify(contract, "TestReceive");
            Console.WriteLine($"Receive Function: {(result.Success ? "PASS" : "FAIL")} - {result.Error}");
        }

        private void TestStructMemory()
        {
            Console.WriteLine("\n--- Testing Struct Memory ---");
            var contract = @"
pragma solidity ^0.5.0;

contract TestStructMemory {
    struct Person {
        string name;
        uint256 age;
    }
    
    function createPerson(string memory name, uint256 age) public pure returns (Person memory) {
        return Person(name, age);
    }
}";
            
            var result = TranslateAndVerify(contract, "TestStructMemory");
            Console.WriteLine($"Struct Memory: {(result.Success ? "PASS" : "FAIL")} - {result.Error}");
        }

        private (bool Success, string Error) TranslateAndVerify(string solidityCode, string contractName)
        {
            try
            {
                // Write contract to temp file
                var tempFile = Path.Combine(testDir, $"{contractName}.sol");
                Directory.CreateDirectory(testDir);
                File.WriteAllText(tempFile, solidityCode);

                // Compile with solc
                var compiler = new SolidityCompiler();
                var compilerOutput = compiler.Compile(solcPath, tempFile, testDir);

                if (compilerOutput.ContainsError())
                {
                    return (false, "Solc compilation failed");
                }

                // Build AST and translate
                var ast = new AST(compilerOutput, testDir);
                var translator = new BoogieTranslator();
                var translatorFlags = new TranslatorFlags();
                
                var boogieAST = translator.Translate(ast, new HashSet<Tuple<string, string>>(), translatorFlags, contractName);

                // Verify Boogie syntax
                var boogieCode = boogieAST.GetRoot().ToString();
                if (string.IsNullOrWhiteSpace(boogieCode) || 
                    !boogieCode.Contains("procedure") || 
                    !boogieCode.Contains("implementation"))
                {
                    return (false, "Invalid Boogie output");
                }

                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
} 