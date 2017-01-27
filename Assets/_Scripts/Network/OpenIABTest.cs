/*******************************************************************************
 * Copyright 2012-2014 One Platform Foundation
 *
 *       Licensed under the Apache License, Version 2.0 (the "License");
 *       you may not use this file except in compliance with the License.
 *       You may obtain a copy of the License at
 *
 *           http://www.apache.org/licenses/LICENSE-2.0
 *
 *       Unless required by applicable law or agreed to in writing, software
 *       distributed under the License is distributed on an "AS IS" BASIS,
 *       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *       See the License for the specific language governing permissions and
 *       limitations under the License.
 ******************************************************************************/

using UnityEngine;
using OnePF;
using System.Collections.Generic;
using System.Collections;

/**
 * Example of OpenIAB usage
 */ 
public class OpenIABTest : MonoBehaviour
{
	public static OpenIABTest instance;
	public static string[] productid;
	
	string _label = "";
	public bool _isInitialized = false;
	Inventory _inventory = null;	
	
	void Awake(){
			instance = this;
	}
	
	private void OnEnable()
	{
		// Listen to all events for illustration purposes
		OpenIABEventManager.billingSupportedEvent += billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent += purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
		
	}
	
	void intiatizeIds(){
//				print ("id initiaialized");
		productid = new string[5];
		//Undo
		productid [0] = "";
		productid [1] = "";
		productid [2] = "";
		productid [3] = "";
		productid [4] = "";
	}

	private void OnDisable()
	{
		// Remove all event handlers
		OpenIABEventManager.billingSupportedEvent -= billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}
	
	bool isIabCalled;
	
	private void Start()
	{
			//initialize openiab
		initializeOpenIAB ();
		//add the ids to array
		intiatizeIds ();
	}
	
	public void initializeOpenIAB(){

		//nine men'morris fantasy demeter games
		var publicKey = "";

		var options = new Options();
		options.checkInventoryTimeoutMs = Options.INVENTORY_CHECK_TIMEOUT_MS * 2;
		options.discoveryTimeoutMs = Options.DISCOVER_TIMEOUT_MS * 2;
		options.checkInventory = true;
		options.verifyMode = OptionsVerifyMode.VERIFY_SKIP;
		options.storeKeys = new Dictionary<string, string> { {OpenIAB_Android.STORE_GOOGLE, publicKey} };
		options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;
		
		// Transmit options and start the service
		OpenIAB.init(options);
		_label+="openiab initialized";
	}
	
//	void OnGUI(){
//		if (!_isInitialized)
//			return;
//				GUI.TextArea (new Rect(10f,10f,400f,400f), _label);
//	}
	
	public void queryInventory(){
		if (!_isInitialized)
			return;
		OpenIAB.queryInventory (productid);
	}
	string item;
	public void purchaseItem(int id){
		print ("purchase "+id);
		item = productid[id];
		_label += " purchase "+item;
		OpenIAB.purchaseProduct (item);
	}
	
	private void billingSupportedEvent()
	{
		_isInitialized = true;
		queryInventory ();
		//		Debug.Log("billingSupportedEvent");
	}
	private void billingNotSupportedEvent(string error)
	{
		//		Debug.Log("billingNotSupportedEvent: " + error);
	}
	
	
	private void queryInventorySucceededEvent(Inventory inventory)
	{
		//		Debug.Log("queryInventorySucceededEvent: " + inventory);
		if (inventory != null)
		{
			_label = inventory.ToString();
			_inventory = inventory;
			Purchase[] purchaseList =  inventory.GetAllPurchases().ToArray();
			SkuDetails[] skuList =  inventory.GetAllAvailableSkus().ToArray();
			
			_label += "purchase length "+purchaseList.Length;
			//if there any purchse which are not consumed
			if(purchaseList.Length!=0){
				
				for(int k=0;k<purchaseList.Length;k++){
					OpenIAB.consumeProduct (inventory.GetPurchase(purchaseList[k].Sku));
					_label += "product Consumed";
					//add in your Game
					if (purchaseList[k].Sku == "remove_ad") {
						PlayerPrefs.SetInt (GameConstants.key_playerPrefs_Remove_Ads, 1);
						PlayerPrefs.Save ();
						InAppManager.instance.removebutton.SetActive(false);
					} else
						InAppManager.giveUndoForInAppPurchase(purchaseList[k].Sku);
					_label +="undo added for "+purchaseList[k].Sku;
				}
			}
			
		}
		
		
	}
	
	
	private void queryInventoryFailedEvent(string error)
	{
		//		Debug.Log("queryInventoryFailedEvent: " + error);
		_label = error+"queryInventoryFailedEvent: ";
	}
	private void purchaseSucceededEvent(Purchase purchase)
	{
		//		Debug.Log("purchaseSucceededEvent: " + purchase.Sku);
		_label = "PURCHASED:" + purchase.ToString();
		
		OpenIAB.consumeProduct (purchase);
		
	}
	private void purchaseFailedEvent(int errorCode, string errorMessage)
	{
		//		Debug.Log("purchaseFailedEvent: " + errorMessage);
		_label = "Purchase Failed: " + errorMessage;
	}
	private void consumePurchaseSucceededEvent(Purchase purchase)
	{
		//		Debug.Log("consumePurchaseSucceededEvent: " + purchase);
		_label = "CONSUMED: " + purchase.ToString();
		
		if (purchase.Sku == "remove_ad") {
			PlayerPrefs.SetInt (GameConstants.key_playerPrefs_Remove_Ads, 1);
			PlayerPrefs.Save ();
			InAppManager.instance.removebutton.SetActive(false);
		} else
			InAppManager.giveUndoForInAppPurchase(purchase.Sku);
	}
	private void consumePurchaseFailedEvent(string error)
	{
		//		Debug.Log("consumePurchaseFailedEvent: " + error);
		_label = "Consume Failed: " + error;
	}
}