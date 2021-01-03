using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIButton : UIButton
{
    public override void SetState (State state, bool immediate)
	{
		base.SetState(state, immediate);
        isEnabled = (State.Disabled != state);
    }

    public void ExecuteClick()
    {
        current = this;
        EventDelegate.Execute(onClick);
        current = null;
    }
}
