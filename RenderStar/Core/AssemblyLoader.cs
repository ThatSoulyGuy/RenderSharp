using System.Reflection;

namespace Engine.Core
{
    public class AssemblyLoader
    {
        public string Name { get; private set; } = string.Empty;
        public string Path { get; private set; } = string.Empty;

        public Assembly Assembly { get; private set; } = null!;

        public void CallMethod(string className, string methodName, params object[] arguments)
        {
            Type? module = Assembly.GetType(className);

            if (module == null)
            {
                Console.WriteLine($"Error: Type '{className}' not found in assembly.");
                return;
            }

            if (module == null)
            {
                Console.WriteLine($"Error: Type '{className}' not found in assembly.");
                return;
            }

            MethodInfo method = module.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)!;

            if (method == null)
            {
                Console.WriteLine($"Method '{methodName}' not found in type '{className}'.");
                return;
            }

            try
            {
                if (method.IsStatic)
                    method.Invoke(null, arguments);
                else
                {
                    object? classInstance = Activator.CreateInstance(module);

                    method.Invoke(classInstance, arguments);
                }
            }
            catch (TargetParameterCountException exception)
            {
                Console.WriteLine($"Parameter count mismatch: {exception.Message}");
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine($"Argument exception: {exception.Message}");
            }
            catch (TargetInvocationException exception)
            {
                Console.WriteLine($"Error during method execution: {exception.InnerException?.Message}");
            }
        }

        public static AssemblyLoader Create(string name, string path)
        {
            AssemblyLoader loader = new()
            {
                Name = name,
                Path = path,
            };

            try
            {
                loader.Assembly = Assembly.LoadFrom(path);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error loading assembly: " + exception.Message);
            }

            return loader;
        }
    }
}
