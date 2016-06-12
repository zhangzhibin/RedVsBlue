using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IGIIEventObserver{
	/// <summary>
	/// process event
	/// return true if the event should be passed down
	/// </summary>
	/// <param name="eventData">Event data.</param>
	bool OnEvent (IGIIEvent eventData);
}

public interface IGIIEventObserverEx: IGIIEventObserver{
	IList<string> InterestedEvents{
		get;
	}
}