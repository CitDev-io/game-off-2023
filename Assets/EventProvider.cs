using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StringDelegate(string String);

public class EventProvider
{
    public StringDelegate OnAnnouncedEvent;
}
