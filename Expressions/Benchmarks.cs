using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Expressions
{
    public class Benchmark
    {
    }

    [SimpleJob(launchCount: 1, warmupCount: 1, iterationCount: 10, invocationCount: 1)]
    public class ExpressionsAndIQuerable
    {
        private DbSet<Product> _Products;
        private ApplicationDbContext _DbContext;
        private int count = 100000;

        [GlobalSetup]
        public void Setup()
        {

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var options = optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=12345678;")
            //.LogTo(Console.WriteLine, LogLevel.Information)
            .Options;

            _DbContext = new ApplicationDbContext(options);

            _Products = _DbContext.Products;
        }

        private static readonly Expression<Func<Product, bool>> s_ageExpression = e => e.Price == 23 && e.Price == 23 && e.Price == 23 && e.ProductId > e.Price;
        [Benchmark]
        public void Ex()
        {
            for (int i = 0; i < count; i++)
            {
                _Products.Where(s_ageExpression).First();
            }
        }

        [Benchmark(Baseline = true)]
        public void Lambda()
        {
            for (int i = 0; i < count; i++)
            {
                _Products.Where(e => e.Price == 23 && e.Price == 23 && e.Price == 23 && e.ProductId > e.Price).First();

            }
        }

        // Compiled
        private static readonly Func<Product, bool> s_ageExpressionCompiled = s_ageExpression.Compile();
        //[Benchmark]
        public void Ex_Compiled()
        {
            for (int i = 0; i < count; i++)
            {
                _Products.Where(s_ageExpressionCompiled).First();

            }
        }

        private static readonly Func<Product, bool> s_ageFunc = e => e.Price == 23 && e.Price == 23 && e.Price == 23 && e.ProductId > e.Price;
        //[Benchmark]
        public void Ex_Func()
        {
            for (int i = 0; i < count; i++)
            {
                _Products.Where(s_ageFunc).First();
            }
        }

        [Benchmark]
        public void Kexpression()
        {
            for (int i = 0; i < count; i++)
            {
                Expression<Func<Product, bool>> kexpr = e => e.Price == 23 && e.Price == 23 && e.Price == 23 && e.ProductId > e.Price;
                _Products.Where(kexpr).First();
            }
        }

        [Benchmark]
        public void Query()
        {
            for (int i = 0; i < count; i++)
            {
                Func<Product, bool> kexpr = e => e.Price == 23 && e.Price == 23 && e.Price == 23 && e.ProductId > e.Price;
                var  query = _Products.Where(e => e.Price == 23);
                query = query.Where(e => e.Price == 23);
                query = query.Where(e => e.Price == 23);
                query.Where(e => e.ProductId > e.Price).First();
            }
        }
    }

    public class CompiledFaster
    {
        private char[] _charArray = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'm', 'k' };
        private Random _random = new Random();
        private IEnumerable<Person> _persons;
        private int _lenght = 10000;
        public class Person
        {
            public int Age { get; set; }
            public string Name { get; set; }

        }
        private string GetName(int cnt)
        {
            var count = cnt / 10 + 1;

            string result = "";
            for (int i = 0; i < count; i++)
            {
                result += _charArray[_random.Next(10)];
            }
            return result;
        }
        [GlobalSetup]
        public void Setup()
        {
            var collection = new List<Person>();
            for (int i = 0; i < _lenght; i++)
            {
                var name = GetName(i);
                collection.Add(new Person() { Name = name, Age = _random.Next(1, 50) });
            }

            _persons = collection;
        }

        private static readonly Expression<Func<Person, bool>> s_ageExpression = e => e.Age == 23;
        [Benchmark]
        public int Ex() => _persons.Where(s_ageExpression.Compile()).Count();

        // Compiled
        private static readonly Func<Person, bool> s_ageExpressionCompiled = s_ageExpression.Compile();
        [Benchmark]
        public int Ex_StaticCompiled() => _persons.Where(s_ageExpressionCompiled).Count();

        [Benchmark]
        public int Lambda() => _persons.Where(e => e.Age == 23).Count();

        private Func<Person, bool> s_ageExpressionCompile ()=> s_ageExpression.Compile();
        [Benchmark]
        public int Ex_Compiled() => _persons.Where(s_ageExpressionCompile()).Count();

    }

    public class WhereIsCompiledDelegate
    {
        private char[] _charArray = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'm', 'k' };
        private Random _random = new Random();
        private IEnumerable<Product> _products;
        private int _lenght = 10000;
        private string GetName(int cnt)
        {
            var count = cnt / 10 + 1;

            string result = "";
            for (int i = 0; i < count; i++)
            {
                result += _charArray[_random.Next(10)];
            }
            return result;
        }
        [GlobalSetup]
        public void Setup()
        {
            var collection = new List<Product>();
            for (int i = 0; i < _lenght; i++)
            {
                var name = GetName(i);
                collection.Add(new Product() { Name = name, Price = _random.Next(1, 50), ProductId = i });
            }

            _products = collection;
        }

        private static readonly Expression<Func<Product, bool>> s_ageExpression = e => e.Price == 23 && e.Price == 23 && e.Price == 23 && e.ProductId > e.Price;
        // Compiled
        private static readonly Func<Product, bool> s_ageExpressionCompiled = s_ageExpression.Compile();
        [Benchmark]
        public int Ex_StaticCompiled() => _products.Where(s_ageExpressionCompiled).Count();


        private static readonly Func<Product, bool> s_ageExpressionFunc = e => e.Price == 23 && e.Price == 23 && e.Price == 23 && e.ProductId > e.Price;
        [Benchmark]
        public int Just_Func() => _products.Where(s_ageExpressionFunc).Count();

        [Benchmark]
        public int Lambda() => _products.Where(e => e.Price == 23 && e.Price == 23 && e.Price == 23 && e.ProductId > e.Price).Count();

        [Benchmark]
        public int WhereIs()
        {
            var result = _products.Where(e => e.Price == 23);
            result = result.Where(e=> e.Price == 23);
            result = result.Where(e => e.Price == 23);
            return result.Where(e => e.ProductId > e.Price).Count();
        }

    }

    public class SampleClass
    {
        public int Value { get; set; }
    }
    public class ReflectionOrExpression
    {
        public ReflectionOrExpression()
        {
            
        }

        public void Do()
        {
            int iterations = 1000000;

            // Пример с использованием рефлексии
            var sampleObject = new SampleClass();
            PropertyInfo propertyInfo = typeof(SampleClass).GetProperty("Value");
            object[] parameters = new object[] { 42 };

            Stopwatch reflectionStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                propertyInfo.SetValue(sampleObject, parameters[0]);
            }
            reflectionStopwatch.Stop();

            Stopwatch expressionStopwatch = Stopwatch.StartNew();
            // Пример с использованием выражений
            var sampleObjectExpression = Expression.Parameter(typeof(SampleClass));
            var valueExpression = Expression.Property(sampleObjectExpression, "Value");
            var lambdaExpression = Expression.Lambda<Action<SampleClass>>(
                Expression.Assign(valueExpression, Expression.Constant(43)),
                sampleObjectExpression
            ).Compile();

            for (int i = 0; i < iterations; i++)
            {
                lambdaExpression(sampleObject);
            }
            expressionStopwatch.Stop();

            Console.WriteLine(sampleObject.Value);
            Console.WriteLine($"Reflection Time: {reflectionStopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Expression Time: {expressionStopwatch.ElapsedMilliseconds} ms");

        }
    }

    public class ReflectionOrExpressionBenchmark
    {
        private readonly int _iterations = 1000000;
        private SampleClass _sampleObject = new SampleClass();

        // Пример с использованием рефлексии

        [Benchmark]
        public void ReflectionTest()
        {
            PropertyInfo propertyInfo = typeof(SampleClass).GetProperty("Value");
            object[] parameters = new object[] { 42 };

            for (int i = 0; i < _iterations; i++)
            {
                propertyInfo.SetValue(_sampleObject, parameters[0]);
            }
        }

        // Пример с использованием выражений

        [Benchmark(Baseline = true)]
        public void ExpressionTest()
        {
            var sampleObjectExpression = Expression.Parameter(typeof(SampleClass));
            var valueExpression = Expression.Property(sampleObjectExpression, "Value");
            var lambdaExpression = Expression.Lambda<Action<SampleClass>>(
                Expression.Assign(valueExpression, Expression.Constant(42)),
                sampleObjectExpression
            ).Compile();

            for (int i = 0; i < _iterations; i++)
            {
                lambdaExpression(_sampleObject);
            }
        }
    }
}
