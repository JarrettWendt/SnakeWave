using System.Threading;

public class PassableLock
{
	private readonly object passableLock = new object(), lockLock = new object();
	private object owner;

	// Will activate the lock and unconditionally assign it to the caller, unless passing is locked.
	public void PassLockTo(object caller)
	{
		if (Monitor.TryEnter(lockLock))
		{
			Monitor.TryEnter(passableLock);
			owner = caller;
			Monitor.Exit(lockLock);
		}
	}

	// Lock the passing lock - prevents further passing until released.
	public bool LockPassing(object caller)
	{
		if (Monitor.TryEnter(lockLock))
		{
			// This caller doesn't even have the lock, so forget it.
			if (owner != caller)
			{
				Monitor.Exit(lockLock);
				return false;
			}
		}
		return true;
	}

	public void Release()
	{
		owner = default;
		Monitor.Exit(passableLock);
		Monitor.Exit(lockLock);
	}
}
