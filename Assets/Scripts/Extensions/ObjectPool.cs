using System.Collections.Generic;

namespace TestProject.Extensions {

	#region POOL

	public abstract class ListPoolBase {

		protected static List<ListPoolBase> allocatedPools = new List<ListPoolBase>();

		public static void Destroy() {

			for (var i = 0; i < ListPoolBase.allocatedPools.Count; ++i) {

				ListPoolBase.allocatedPools[i].Reset();

			}

			ListPoolBase.allocatedPools.Clear();

		}

		protected abstract void Reset();

	}

	public class ListPool<T> : ListPoolBase {

		private readonly ObjectPoolInternal<List<T>> listPool = new ObjectPoolInternal<List<T>>(null, l => l.Clear());
		private static ListPool<T> instance;

		/*
		public static void Allocate(int capacity) {

			ListPool<T>.listPool = new ObjectPoolInternal<List<T>>(null, l => l.Clear(), capacity);

		}
		*/

		private static void CreatePoolInstance() {

			if (ListPool<T>.instance != null) return;

			ListPool<T>.instance = new ListPool<T>();
			ListPoolBase.allocatedPools.Add(ListPool<T>.instance);


		}

		public static List<T> Get() {

			ListPool<T>.CreatePoolInstance();

			return ListPool<T>.instance.listPool.Get(CreateInstance);

		}

		private void ReleaseAllocated(List<T> toRelease) {

			if (toRelease == null) return;

			this.listPool.Release(toRelease);

		}


		public static void Release(List<T> toRelease) {

			ListPool<T>.CreatePoolInstance();

			ListPool<T>.instance.ReleaseAllocated(toRelease);

		}

		public static void ReleaseAll() {

			ListPool<T>.instance.Reset();

		}

		protected override void Reset() {

			this.listPool.Reset();
			ListPool<T>.instance = null;

		}

		private static List<T> CreateInstance() {

			return new List<T>();
		}

	}

	public static class ObjectPool<T> where T : new() {

		private static ObjectPoolInternal<T> pool = new ObjectPoolInternal<T>(null, null);

		public static void Allocate(int capacity) {

			ObjectPool<T>.pool = new ObjectPoolInternal<T>(null, null, capacity);

		}

		public static T Get() {

			return ObjectPool<T>.pool.Get(ObjectPool<T>.CreateInstance);

		}

		public static void Release(T toRelease) {

			ObjectPool<T>.pool.Release(toRelease);

		}

		public static void ResetAll() {

			ObjectPool<T>.pool.Reset();

		}

		private static T CreateInstance() {

			return new T();
		}

	}

	public static class GameObjectPool<T> where T : TestProject.Gameplay.Views.View {

		private static Dictionary<int, ObjectPoolInternal<T>> pool = new Dictionary<int, ObjectPoolInternal<T>>();
		
		public static T Get(T source) {

			var poolId = source.poolId;

			ObjectPoolInternal<T> pool;
			if (GameObjectPool<T>.pool.TryGetValue(poolId, out pool) == false) {

				pool = new ObjectPoolInternal<T>(
					(item) => { item.gameObject.SetActive(true); },
					(item) => {
						item.gameObject.SetActive(false);
						item.transform.SetParent(null);
					});
				GameObjectPool<T>.pool.Add(poolId, pool);
				
			}

			return pool.Get(() => UnityEngine.Object.Instantiate(source));

		}

		public static void Release(T toRelease) {

			var poolId = toRelease.poolId;

			ObjectPoolInternal<T> pool;
			if (GameObjectPool<T>.pool.TryGetValue(poolId, out pool) == true) {
			
				pool.Release(toRelease);
			
			}
			
		}

	}

	public interface IObjectPoolItem {

		void OnPoolGet();
		void OnPoolRelease();

	}

	public static class ObjectPoolEventable<T> where T : IObjectPoolItem, new() {

		private static ObjectPoolInternal<T> pool = new ObjectPoolInternal<T>(x => x.OnPoolGet(), x => x.OnPoolRelease());

		public static void Allocate(int capacity) {

			ObjectPoolEventable<T>.pool = new ObjectPoolInternal<T>(x => x.OnPoolGet(), x => x.OnPoolRelease(), capacity);

		}

		public static T Get() {

			return ObjectPoolEventable<T>.pool.Get(() => new T());

		}

		public static void Release(T toRelease) {

			ObjectPoolEventable<T>.pool.Release(toRelease);

		}

	}

	internal class ObjectPoolInternal<T> {

		private readonly Stack<T> stack = new Stack<T>();
		private System.Action<T> actionOnGet;
		private System.Action<T> actionOnRelease;

		public int countAll { get; private set; }

		public int countActive {
			get { return this.countAll - this.countInactive; }
		}

		public int countInactive {
			get { return this.stack.Count; }
		}

		public ObjectPoolInternal(System.Action<T> actionOnGet, System.Action<T> actionOnRelease, int capacity = 1) {

			this.actionOnGet = actionOnGet;
			this.actionOnRelease = actionOnRelease;

			if (capacity > 0) {

				this.stack = new Stack<T>(capacity);

			}

		}

		public T Get(System.Func<T> constructor) {

			T element;
			if (this.stack.Count == 0) {

				element = constructor.Invoke();
				++this.countAll;

			} else {

				element = this.stack.Pop();
				if (element == null) element = constructor.Invoke();

			}

			if (this.actionOnGet != null) this.actionOnGet.Invoke(element);

			return element;

		}

		public void Release(T element) {

			if (this.actionOnRelease != null) this.actionOnRelease.Invoke(element);
			this.stack.Push(element);

		}

		public void Reset() {

			this.stack.Clear();
			this.actionOnGet = null;
			this.actionOnRelease = null;

		}

	}

	#endregion

}