namespace GW2Scratch.EVTCAnalytics.LogTests.LocalSets
{
	public class Result<T>
	{
		public bool Correct { get; private set; }
		public bool Checked { get; private set; }
		public T ExpectedValue { get; private set; }
		public T ActualValue { get; private set; }

		public static Result<T> UncheckedResult(T value)
		{
			return new Result<T>
			{
				Correct = true,
				Checked = false,
				ExpectedValue = default,
				ActualValue = value
			};
		}

		public static Result<T> CheckedResult(T expected, T actual)
		{
			return new Result<T>
			{
				Correct = expected.Equals(actual),
				Checked = true,
				ExpectedValue = expected,
				ActualValue = actual
			};
		}

		public static Result<T> CorrectResult(T actual)
		{
			return new Result<T>
			{
				Correct = true,
				Checked = true,
				ExpectedValue = actual,
				ActualValue = actual
			};
		}

		public static Result<T> IncorrectResult(T expected, T actual)
		{
			return new Result<T>
			{
				Correct = false,
				Checked = true,
				ExpectedValue = expected,
				ActualValue = actual
			};
		}
	}
}