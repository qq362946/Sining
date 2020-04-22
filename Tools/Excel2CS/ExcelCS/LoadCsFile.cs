using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MongoDB.Bson;
using Sining.Config;

namespace ExcelToCS
{
    public static class LoadCsFile
    {
        public static Assembly Load()
        {
            var syntaxTreeList =
                Directory.GetFiles(Path.Combine(Program.CsFileDirectory, "Entity"))
                    .Select(file => new StreamReader(file))
                    .Select(sr => CSharpSyntaxTree.ParseText(sr.ReadToEnd())).ToList();
            
            var currentDomain = AppDomain.CurrentDomain;
            var assemblyName = Path.GetRandomFileName();
            var assemblyArray = currentDomain.GetAssemblies();
            var metadataReferenceList = new List<MetadataReference>();
            AssemblyMetadata assemblyMetadata;
            MetadataReference metadataReference;
            
            // 注册引用
            
            foreach (var domainAssembly in assemblyArray)
            {
                assemblyMetadata = AssemblyMetadata.CreateFromFile(domainAssembly.Location);
                metadataReference = assemblyMetadata.GetReference();
                metadataReferenceList.Add(metadataReference);
            }

            // 添加Bson支持
            
            assemblyMetadata = AssemblyMetadata.CreateFromFile(typeof(BsonArray).Assembly.Location);
            metadataReference = assemblyMetadata.GetReference();
            metadataReferenceList.Add(metadataReference);
            
            assemblyMetadata = AssemblyMetadata.CreateFromFile(typeof(IConfig).Assembly.Location);
            metadataReference = assemblyMetadata.GetReference();
            metadataReferenceList.Add(metadataReference);

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: syntaxTreeList,
                references: metadataReferenceList,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            
            var result = compilation.Emit(ms);
            
            if (!result.Success)
            {
                throw new Exception("failures");
            }
            
            ms.Seek(0, SeekOrigin.Begin);
            
            return Assembly.Load(ms.ToArray());
        }
    }
}