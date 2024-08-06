using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomizeButton
	:
	MonoBehaviour
{
	public enum ButtonType
	{
		Model,
		Color
	}

	void Start()
	{
		if( this.buttonType == ButtonType.Color )
		{
			GetComponentInChildren<TMP_Text>().enabled = false;
		}
	}

	public void SetupCallback( CustomizePanel obj,int index )
	{
		callbackObj = obj;

		this.index = index;
	}

	public void OnClick()
	{
		callbackObj.ButtonCallback( buttonType,index );
		// todo: change button color to indicate selected, deselect other buttons
	}

	[SerializeField] public ButtonType buttonType = ButtonType.Model;
	
	int index = -1;

	CustomizePanel callbackObj = null;
}