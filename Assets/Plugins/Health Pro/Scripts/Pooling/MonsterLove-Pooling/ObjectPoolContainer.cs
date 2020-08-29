
/*

 Modifications have been made to the original:
 Added an out parameter to indicate if the object has already been recycled
 This is useful when you want to add logic on first creation of an object.

 Renmaed namespace to prevent overlap issues for users that potentially might use this pooling solution already.

 Source : https://github.com/thefuntastic/unity-object-pool
 Licence : MIT

*/

namespace MonsterLove.HealthPro.Collections
{
	public class ObjectPoolContainer<T> where T : class
	{
		private T item;

		public bool Used { get; private set; }

		public void Consume()
		{
			Used = true;
		}

		public T Item
		{
			get
			{
				return item;
			}
			set
			{
				item = value;
			}
		}

		public void Release()
		{
			Used = false;
		}
	}
}
