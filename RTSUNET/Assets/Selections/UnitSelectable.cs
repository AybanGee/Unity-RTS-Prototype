using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectable : MonoBehaviour, ISelectHandler, IPointerClickHandler, IDeselectHandler {
	public bool isSelected;
	public PlayerObject playerObject;

	Renderer myRenderer;

	[SerializeField]
	Material unselectedMat;
	[SerializeField]
	Material selectedMat;
	void Start(){
		//if(!isLocalPlayer)return;
		myRenderer = GetComponentInChildren<Renderer>();
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
		if(!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
        DeselectAll(eventData);
        OnSelect(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {//if(!isLocalPlayer)return;
       // selectedUnits.Add(this);
	   Debug.Log("SELECT");
	          if (isSelected && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
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
}
