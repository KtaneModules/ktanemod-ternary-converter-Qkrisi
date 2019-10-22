using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using UnityEngine;
using rdn = UnityEngine.Random;

public class qkTernaryConverter : MonoBehaviour {
	public KMSelectable[] numberedButtons;
	public KMSelectable[] symbolButtons;
	public KMSelectable submitButton;
	public KMSelectable clearButton;
	public GameObject Display;
	public GameObject inputDisplay;
	public GameObject decimalDisplay;
	private string balancedTernary = "";
	private int numberToMake = 0;
	private string base3 = "";
	private string inputstring = "";
	private int state = -1;
	private bool needBalance = false;
	private string finalAnswer="";
	public KMAudio Audio;
	private bool solved = false;
	static int moduleIdCounter;
	int moduleId;


	static int []arr = new int[33]; 
  
	static void balTernary(int ter) 
	{ 
    	int carry = 0, b = 10; 
    	int i = 32; 
    	while (ter > 0)  
    	{ 
        int rem = ter % b; 
        rem = rem + carry; 
        if (rem == 0)  
        { 
            arr[i--] = 0; 
            carry = 0; 
        } 
        else if (rem == 1)  
        { 
            arr[i--] = 1; 
            carry = 0; 
        } 
        else if (rem == 2) 
        { 
            arr[i--] = -1; 
            carry = 1; 
        } 
        else if (rem == 3)  
        { 
            arr[i--] = 0; 
            carry = 1; 
        } 
        ter = (int)(ter / b); 
    } 
    if (carry == 1) 
        arr[i] = 1; 
} 
  

static int ternary(int number) 
{ 
    int ans = 0, rem = 1, b = 1; 
    while (number > 0)  
    { 
        rem = number % 3; 
        ans = ans + rem * b; 
        number = (int)(number / 3); 
        b = b * 10; 
    } 
    return ans; 
} 
  

public void convert(int number) 
{ 

	//Code made by Rajput-Ji modified by Qkrisi
	//Original code: https://www.geeksforgeeks.org/game-theory-in-balanced-ternary-numeral-system-moving-3k-steps-at-a-time/
  
    int ter = ternary(number); 
	base3=ter.ToString();
    balTernary(ter); 

    int i = 0; 
  
    
    while (arr[i] == 0)  
    { 
        i++; 
    } 
  
    for (int j = i; j <= 32; j++)  
    { 
  
        
        if (arr[j] == -1) {
            balancedTernary=balancedTernary+"-";}
        else{
			if (arr[j] == 1) {
            	balancedTernary=balancedTernary+"+";}
				else{
        			balancedTernary=balancedTernary+arr[j].ToString();}
    } 
}}
 
  

	void resizeText(GameObject disp){
		int pluschars = 0;
		if(disp.GetComponent<TextMesh>().text.Contains("+")){pluschars = disp.GetComponent<TextMesh>().text.Length-6;}
		else{pluschars = disp.GetComponent<TextMesh>().text.Length-7;}
		while(pluschars>0){
			disp.GetComponent<TextMesh>().fontSize=disp.GetComponent<TextMesh>().fontSize-15;
			if(disp.GetComponent<TextMesh>().fontSize<45){
				disp.GetComponent<TextMesh>().fontSize=disp.GetComponent<TextMesh>().fontSize+20;
			}
			pluschars=pluschars-1;
		}
		return;
	}
	

	
	void Start () {
		moduleId=moduleIdCounter++;
		numberToMake=rdn.Range(0,10000);
		state=rdn.Range(0,2);
		if(state==1){
			needBalance=true;
		}
		else{
			needBalance=false;
		}
		convert(numberToMake);
		Debug.LogFormat("[Ternary Converter #{0}] Decimal number: {1}", moduleId, numberToMake.ToString());
		Debug.LogFormat("[Ternary Converter #{0}] In standard ternary: {1}", moduleId, base3);
		Debug.LogFormat("[Ternary Converter #{0}] In balanced ternary: {1}", moduleId, balancedTernary);
		if(needBalance){
			Debug.LogFormat("[Ternary Converter #{0}] The display is in standard ternary so you need to input balanced ternary", moduleId);
			Display.GetComponent<TextMesh>().text=base3;
			finalAnswer=balancedTernary;
		}
		else{
			Debug.LogFormat("[Ternary Converter #{0}] The display is in balanced ternary so you need to input standard ternary", moduleId);
			Display.GetComponent<TextMesh>().text=balancedTernary;
			finalAnswer=base3;
		}
		resizeText(Display);
		numberedButtons[0].OnInteract += delegate(){
			pressButton("0");
			return false;
		};
		numberedButtons[1].OnInteract += delegate(){
			pressButton("1");
			return false;
		};
		numberedButtons[2].OnInteract += delegate(){
			pressButton("2");
			return false;
		};
		symbolButtons[0].OnInteract += delegate(){
			pressButton("-");
			return false;
		};
		symbolButtons[1].OnInteract += delegate(){
			pressButton("+");
			return false;
		};
		clearButton.OnInteract += delegate(){
			clearButton.AddInteractionPunch(.5f);
			Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, clearButton.transform);
			if(!solved){
			inputstring="";
			inputDisplay.GetComponent<TextMesh>().text="";
			inputDisplay.GetComponent<TextMesh>().fontSize=120;}
			return false;
		};
		submitButton.OnInteract += delegate(){
			submitButton.AddInteractionPunch(.5f);
			Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submitButton.transform);
			if(!solved){
			if(inputstring==finalAnswer){
				Debug.LogFormat("[Ternary Converter #{0}] Correct answer entered! Module solved!", moduleId);
				solved=true;
				decimalDisplay.GetComponent<TextMesh>().text=numberToMake.ToString();
				resizeText(decimalDisplay);
				GetComponent<KMBombModule>().HandlePass();
			}
			else{
				if(inputstring==""){Debug.LogFormat("[Ternary Converter #{0}] Incorrect answer entered: *Literally nothing*! Strike!", moduleId);}
				else{Debug.LogFormat("[Ternary Converter #{0}] Incorrect answer entered: {1}! Strike!", moduleId, inputstring);}
				inputstring="";
				inputDisplay.GetComponent<TextMesh>().text="";
				GetComponent<KMBombModule>().HandleStrike();
			}}
			return false;
		};
	}

	void pressButton(string character){
		if(!solved){
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
		inputstring=inputstring+character;
		inputDisplay.GetComponent<TextMesh>().text=inputstring;
		resizeText(inputDisplay);
		}
	}

	public string TwitchHelpMessage = "Use '!{0} submit <answer>' to submit your answer! For ex. '!{0} submit +0--0+' or '!{0} submit 201021'";
	public IEnumerator ProcessTwitchCommand(string command){
		command=command.ToUpper().Replace("SUBMIT","").Replace(" ","");
		for(int i = 0;i<command.Length;i++){
			switch(command[i]){
				case '0':
					yield return null;
					numberedButtons[0].OnInteract();
					break;
				case '1':
					yield return null;
					numberedButtons[1].OnInteract();
					break;
				case '2':
					yield return null;
					numberedButtons[2].OnInteract();
					break;
				case '-':
					yield return null;
					symbolButtons[0].OnInteract();
					break;
				case '+':
					yield return null;
					symbolButtons[1].OnInteract();
					break;
				default:
					yield return null;
					clearButton.OnInteract();
					yield return "sendtochaterror Character not valid!";
					yield break;
					break;
			}
		}
		yield return null;
		submitButton.OnInteract();
		yield break;
	}
}
