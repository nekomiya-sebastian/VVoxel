using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlPanelSwitcher
	:
	MonoBehaviour
{
	void Start()
	{
		OpenPartsPanel();
	}

	public void OpenPartsPanel()
	{
		title.text = "Parts";
		partsPanel.TogglePanel( true );
		posePanel.TogglePanel( false );
		bgPanel.TogglePanel( false );
	}

	public void OpenPosePanel()
	{
		title.text = "Poses";
		partsPanel.TogglePanel( false );
		posePanel.TogglePanel( true );
		bgPanel.TogglePanel( false );
	}

	public void OpenBGPanel()
	{
		title.text = "BG";
		partsPanel.TogglePanel( false );
		posePanel.TogglePanel( false );
		bgPanel.TogglePanel( true );
	}

	[SerializeField] TMP_Text title = null;
	[SerializeField] SidePanelBase partsPanel = null;
	[SerializeField] SidePanelBase posePanel = null;
	[SerializeField] SidePanelBase bgPanel = null;
}