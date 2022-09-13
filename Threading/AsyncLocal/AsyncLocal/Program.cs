using System.Runtime.CompilerServices;

namespace AsyncLocal;

internal class Program
{
	public static async Task Main()
	{
		await Task.Run(async () =>
		{
			Console.WriteLine("Async flow entered...");

			// Init async value
			if (Cache.Instance.Item.Value is { })
				throw new InvalidOperationException("The async flow has just startet. A value should not be initialized.");

			var newValue = new object();
			Console.WriteLine($"Create: value = #{RuntimeHelpers.GetHashCode(newValue)}");

			Cache.Instance.Item.Value = newValue;
			await Foo();

			Console.WriteLine("Async flow exitted.");
		});

		Console.WriteLine("Main finished.\n\n");
		Console.ReadKey();
	}

	private static async Task Foo()
	{
		Console.WriteLine($"Foo: entered...");
		await Bar();

		Console.WriteLine($"Foo: getting value...");
		var knownValue = Cache.Instance.Item.Value;
		Console.WriteLine($"Foo: value = #{RuntimeHelpers.GetHashCode(knownValue)}");
		Console.WriteLine($"Foo: exitted.");
	}

	private static async Task Bar()
	{
		Console.WriteLine($"Bar: entered...");
		// The line below will break async flow
		//await Task.Delay(1);
		await Task.CompletedTask;
		Console.WriteLine($"Bar: exitted.");
	}
}

public sealed class Cache
{
	public static Cache Instance = new Cache();

	public AsyncLocal<object> Item { get; } = new AsyncLocal<object>(OnValueChanged);

	private static void OnValueChanged(AsyncLocalValueChangedArgs<object> args)
	{
		Console.WriteLine($"OnValueChanged! Prev: #{RuntimeHelpers.GetHashCode(args.PreviousValue)} Current: #{RuntimeHelpers.GetHashCode(args.CurrentValue)}");
	}
}