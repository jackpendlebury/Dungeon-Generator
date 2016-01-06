using System;

[Serializable]
public class IntRange {

	public int iMIN;
	public int iMAX;

	public IntRange(int min, int max){
		iMIN = min;
		iMAX = max;
	}

	public int Random{
		get { return UnityEngine.Random.Range(iMIN, iMAX); }
	}
	
}
