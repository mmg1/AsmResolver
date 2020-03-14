using System;
using System.Collections.Generic;
using System.Linq;
using AsmResolver.DotNet.Cloning;
using AsmResolver.DotNet.TestCases.Methods;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.Tests.Runners;
using Xunit;

namespace AsmResolver.DotNet.Tests.Cloning
{
    public class MetadataClonerTest : IClassFixture<TemporaryDirectoryFixture>
    {
        private readonly TemporaryDirectoryFixture _fixture;

        public MetadataClonerTest(TemporaryDirectoryFixture fixture)
        {
            _fixture = fixture;
        }

        private static ModuleDefinition PrepareTempModule()
        {
            var assembly = new AssemblyDefinition("SomeAssembly", new Version(1, 0, 0, 0));
            var module = new ModuleDefinition("SomeModule");
            assembly.Modules.Add(module);
            return module;
        }

        private static MethodDefinition CloneMethod(string methodName, out MethodDefinition method)
        {
            var sourceModule = ModuleDefinition.FromFile(typeof(MethodBodyTypes).Assembly.Location);
            var type = sourceModule.TopLevelTypes.First(t => t.Name == nameof(MethodBodyTypes));
            method = type.Methods.First(m => m.Name == methodName);

            var targetModule = PrepareTempModule();

            var result = new MetadataCloner(targetModule)
                .Include(method)
                .Clone();

            var clonedMethod = (MethodDefinition) result.ClonedMembers.First();
            return clonedMethod;
        }

        [Fact]
        public void CloneHelloWorldProgramType()
        {
            var sourceModule = ModuleDefinition.FromBytes(Properties.Resources.HelloWorld);
            var targetModule = PrepareTempModule();

            var result = new MetadataCloner(targetModule)
                .Include(sourceModule.TopLevelTypes.First(t => t.Name == "Program"))
                .Clone();

            foreach (var type in result.ClonedMembers.OfType<TypeDefinition>())
                targetModule.TopLevelTypes.Add(type);

            targetModule.ManagedEntrypointMethod = (MethodDefinition) result.ClonedMembers.First(m => m.Name == "Main");
            _fixture
                .GetRunner<FrameworkPERunner>()
                .RebuildAndRun(targetModule, "HelloWorld.exe", "Hello World!" + Environment.NewLine);
        }

        [Fact]
        public void CloneBranchInstructions()
        {
            var clonedMethod = CloneMethod(nameof(MethodBodyTypes.Switch), out var method);

            var originalBranches = method.CilMethodBody.Instructions
                .Where(i => i.IsBranch())
                .Select(i => i.Operand)
                .OfType<CilInstructionLabel>()
                .ToArray();
            
            var newBranches = clonedMethod.CilMethodBody.Instructions
                .Where(i => i.IsBranch())
                .Select(i => i.Operand)
                .OfType<CilInstructionLabel>()
                .ToArray();

            // Assert offsets match.
            Assert.Equal(
                originalBranches.Select(x => x.Offset), 
                newBranches.Select(x => x.Offset));

            // Assert all referenced instructions are instructions in the cloned method body.
            Assert.All(newBranches, label => Assert.Same(
                clonedMethod.CilMethodBody.Instructions.GetByOffset(label.Offset),
                label.Instruction));
        }

        [Fact]
        public void CloneSwitchInstruction()
        {
            var clonedMethod = CloneMethod(nameof(MethodBodyTypes.Switch), out var method);

            var originalBranches = (IEnumerable<ICilLabel>) method.CilMethodBody.Instructions
                .First(i => i.OpCode.Code == CilCode.Switch)
                .Operand;
            
            var newBranches = (IEnumerable<ICilLabel>) clonedMethod.CilMethodBody.Instructions
                .First(i => i.OpCode.Code == CilCode.Switch)
                .Operand;

            // Assert offsets match.
            Assert.Equal(
                originalBranches.Select(x => x.Offset), 
                newBranches.Select(x => x.Offset));

            // Assert all referenced instructions are instructions in the cloned method body.
            Assert.All(newBranches, label => Assert.Same(
                clonedMethod.CilMethodBody.Instructions.GetByOffset(label.Offset),
                ((CilInstructionLabel) label).Instruction));
        }
    }
}