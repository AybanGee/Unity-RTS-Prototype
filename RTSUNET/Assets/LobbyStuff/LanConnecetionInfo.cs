﻿using System;

public class LanConnectionInfo {

	public string ipAddress;
	public int port;
	public string name;
	public string[] rawData ;
	public float timeout;

	public LanConnectionInfo (string fromAddress, string data){
		timeout = 5f;
		ipAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1, fromAddress.Length - (fromAddress.LastIndexOf(":") + 1));
		string portText = data.Substring(data.LastIndexOf(":") + 1, data.Length - (data.LastIndexOf(":") + 1) );
		port = 7777;
		int.TryParse(portText, out port);
		name = "local";
		rawData = data.Split(new string[] { ":" }, StringSplitOptions.None);;
		//rawInfo = fromAddress;
		//rawData = data;
	}

	public string ArrToData(string[] arrStr){
		string data = "";
		if(arrStr.Length == 0) return data;
		if(arrStr.Length == 1) return arrStr[0];
		//adds first data in arr
		data = arrStr[0];
		//adds proceeding data with colons
		for (int i = 1; i < arrStr.Length; i++)
		{	
			data = data + ":" + arrStr[i];
		}
		return data;
	}
	
}
	