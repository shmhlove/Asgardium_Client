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
}
