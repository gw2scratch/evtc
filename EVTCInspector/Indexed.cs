namespace GW2Scratch.EVTCInspector
{
	public class Indexed<T> where T : struct
	{
		public T Item { get; }
		public int Index { get; }

		public Indexed(T item, int index)
		{
			Item = item;
			Index = index;
		}
	}
}