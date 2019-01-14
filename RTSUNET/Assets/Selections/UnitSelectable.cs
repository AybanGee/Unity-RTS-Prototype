using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectable : MonoBehaviour, ISelectHandler, IPointerClickHandler, IDeselectHandler {
	public bool isSelected;
	public PlayerObject playerObject;

	public MonoUnitFramework unit;

public bool isOneSelection = false;
	Renderer myRenderer;

	[SerializeField]
	Material unselectedMat;
	[SerializeField]
	Material selectedMat;

	
	void Start(){
		//if(!isLocalPlayer)return;
		myRenderer = GetComponentInChildren<Renderer>();
		unit = GetComponent<MonoUnitFramework>();

		if(myRenderer == null)return;
		unselectedMat = myRenderer.material;
		Color color = unselectedMat.color;
		float r,b,g;
		float add = 0.3f;
		r=Mathf.Clamp(color.r + add,0f,1f);
		b=Mathf.Clamp(color.b + add,0f,1f);
		g=Mathf.Clamp(color.g + add,0f,1f);
		selectedMat= new Material(unselectedMat);
		selectedMat.color = new Color(r,g,b);

	
		
	}

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && playerObject.selectedUnits.Count > 0)
        {
            playerObject.DeselectAll(new BaseEventData(EventSystem.current));
        }
    }
 public void OnDeselect(BaseEventData eventData)
    {//if(!isLocalPlayer)return;
		myRenderer.material = unselectedMat;
        isSelected = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {	
		if(!IsOnTeam())return;
		if(!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
        DeselectAll(eventData);
        OnSelect(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {//if(!isLocalPlayer)return;
       // selectedUnits.Add(this);
	//	if(!IsOnTeam())return;

	   Debug.Log("SELECT");
	          if (!isOneSelection && (isSelected && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
			isSelected = false;
            playerObject.selectedUnits.Remove(gameObject);
            myRenderer.material = unselectedMat;
            return;
        }

		myRenderer.material = selectedMat;
		isSelected = true;
        playerObject.selectedUnits.Add(this.gameObject);
    }

    private void DeselectAll(BaseEventData eventData)
    {//if(!isLocalPlayer)return;
        playerObject.DeselectAll(eventData);
    }

	public bool IsOnTeam(){
		if(playerObject == null){
			Debug.Log("YaAaAaAaA it's rewind time!");
		}
		if(playerObject.team == unit.team)
		return true;
		else
		return false;
	}
}
