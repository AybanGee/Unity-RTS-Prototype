using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlmanacButton : MonoBehaviour {
	public UnitFramework unit;
	public AlmanacLoader almanacLoader;
	public TextMeshProUGUI buttonName;


	public void OnClick(){
		almanacLoader.ChangeSelected(unit);
	}
}
