using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ConsoleApplication2;
namespace BlockingQueueTest
{
	/// <summary>
	/// This class subclasses the System.Collections.Queue class so that it
	/// will block on a dequeue call.  It also has some extra code to make
	/// the class threadsafe.
	/// </summary>
	public class BlockingQueue<T> : Queue<T>
	{
		/// <summary>
		/// Add an object to the queue, and notify all blocked readers
		/// that the queue has an entry.
		/// </summary>
		/// <param name="thing">The object to add to the queue.</param>
		public new void Enqueue(T thing)
		{
			lock(this)
			{
				base.Enqueue(thing);
			}
			// Notify anyone waiting that there's now data available
			DataAvailable.Set();
		}



		/// <summary>
		/// Remove an object from the queue.  If the queue is empty, block
		/// until a writer adds an object to the queue.
		/// </summary>
		/// <param name="maxWait">The timeout value for blocking.</param>
		/// <returns>The next object in the queue.</returns>
		public T Dequeue(int maxWait)
		{
			T result = default(T);
			while(result == null)
			{
				lock(this)
				{
					if(Count > 0) result = base.Dequeue();
				}
				if(result == null)
				{
					if(maxWait == int.MaxValue) DataAvailable.WaitOne();
					else if(!DataAvailable.WaitOne(maxWait, false)) break;
				}
			}
			return result;
		}



		/// <summary>
		/// Calls dequeue with the maximum timeout.
		/// </summary>
		/// <returns>The object at the head of the queue.</returns>
		public new T Dequeue()
		{
			return Dequeue(int.MaxValue);
		}



		/// <summary>
		/// The number of elements in the queue.
		/// </summary>
		public new int Count
		{
			get
			{
				int result;
				lock(this)
				{
					result = base.Count;
				}
				return result;
			}
		}

		/// <summary>
		/// The AutoResetEvent used to signal blocked readers that
		/// a writer has added an element to the queue.
		/// </summary>
		private AutoResetEvent DataAvailable = new AutoResetEvent(true);

	}
}
