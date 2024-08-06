using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTriggerVisToggle
	:
	AnimTriggerBase
{
	void Start()
	{
		ToggleHidden( invis );
	}

	public override void PerformAction()
	{
		invis = !invis;
		ToggleHidden( invis );
	}

	void ToggleHidden( bool hidden )
	{
		transform.GetChild( 0 ).gameObject.SetActive( !hidden );
	}

	[SerializeField] bool invis = false;
}