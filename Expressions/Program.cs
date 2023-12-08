// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Expressions;

Console.WriteLine("Hello, World!");

//var expTest = new ReflectionOrExpression();
//expTest.Do();
//Console.ReadKey();
//return;

BenchmarkRunner.Run<ReflectionOrExpressionBenchmark>();
//BenchmarkRunner.Run<WhereIsCompiledDelegate>();
//BenchmarkRunner.Run<ExpressionsAndIQuerable>();
//BenchmarkRunner.Run<CompiledFaster>();