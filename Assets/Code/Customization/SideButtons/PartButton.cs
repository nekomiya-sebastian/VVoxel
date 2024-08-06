using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartButton
	:
	MonoBehaviour
{
	public void SetupCallback( PartsPanel panel,int index )
	{
		myPanel = panel;
		myIndex = index;
	}

	public void OnClick()
	{
		myPanel.SwitchToPanel( myIndex );
	}

	PartsPanel myPanel = null;
	int myIndex = -1;
}