using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentHandler : MonoBehaviour {
	public List<ComponentUnit> components = new List<ComponentUnit>();

	public List<ComponentUnit> getComponentsByName(string componentName){
		List<ComponentUnit> results = new List<ComponentUnit>();
		foreach (ComponentUnit com in components)
		{
			if(com.name == componentName){
				results.Add(com);
			}
		}
		return results;
	}

	public	List<ComponentUnit> getComponentsByTag(string componentTag){
		List<ComponentUnit> results = new List<ComponentUnit>();
		foreach (ComponentUnit com in components)
		{
			if(com.tag == componentTag){
				results.Add(com);
			}
		}
		return results;
	}

	public ComponentUnit getComponentByName(string componentName){

		foreach (ComponentUnit com in components)
		{
			if(com.name == componentName){
				return com;
			}
		}
		return null;
	}


}