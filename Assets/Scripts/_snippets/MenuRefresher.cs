using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuRefresher : MonoBehaviour {

	// MENU REFRESHER
	// =====================
	// Utility function to refresh menus, lists, etc.
	//
	// Example usage:
	// If I wanted to create a menu of UI.Texts, but all I have is the array of strings and a prefab of the text, I can:
	// 1. Assign that prefab to this.itemPrototype
	// 2. Call MenuRefresher m.Setup<string>(sourceArray, (createdGameObject, sourceString) => { createdGameObject.GetComponent<Text>().text = sourceString; });
	// 3. THAT'S IT!

	[HideInInspector]
	public List<GameObject> allCreatedItems = new List<GameObject>();

	/// <summary>
	/// The item prototype. When Refreshed, a MenuRefresher will create copies of this prototype.
	/// </summary>
	public GameObject itemPrototype;

	public delegate void RefreshAction<T>(GameObject go, T sourceReference);
	public delegate void OnComplete();

	/// <summary>
	/// Calls initial setup functions on this MenuRefresher, then calls Refresh.
	/// </summary>
	/// <param name="sourceArray">Source array.</param>
	/// <param name="toPerformOnItem">To perform on item.</param>
	/// <param name="sameParentAsPrototype">If set to <c>true</c> same parent as prototype.</param>
	/// <param name="keepWorldPosition">If set to <c>true</c> keep world position.</param>
	/// <param name="deactivatePrototype">If set to <c>true</c> deactivate prototype.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public List<GameObject> Setup<T>(T[] sourceArray, RefreshAction<T> toPerformOnItem = null, OnComplete onCompleteCallback = null, bool sameParentAsPrototype = true, bool keepWorldPosition = false, bool deactivatePrototype = true) {
		if (deactivatePrototype)
			itemPrototype.SetActive(false);

		ClearExistingItems();

		return Refresh<T>(sourceArray, toPerformOnItem, onCompleteCallback, sameParentAsPrototype, keepWorldPosition);
	}

	/// <summary>
	/// Create a new GameObject for each T item in sourceArray. Make sure Setup() has been called already.
	/// </summary>
	/// <param name="sourceArray">Source array.</param>
	/// <param name="toPerformOnItem">Action to perform after creating each item. The first parameter represents the GameObject created, the second 
	/// represents the value in the sourceArray that triggers the GameObject's creation.</param>
	/// <param name="sameParentAsPrototype">If set to <c>true</c> same parent as prototype.</param>
	/// <param name="keepWorldPosition">If set to <c>true</c> keep world position.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public List<GameObject> Refresh<T>(T[] sourceArray, RefreshAction<T> toPerformOnItem = null, OnComplete onCompleteCallback = null, bool sameParentAsPrototype = true, bool keepWorldPosition = false) {


		ClearExistingItems();
		foreach (T t in sourceArray) {
			CreateNewItemFrom<T>(t, toPerformOnItem, sameParentAsPrototype, keepWorldPosition);
		}
		if (onCompleteCallback != null)
			onCompleteCallback.Invoke();
		return allCreatedItems;
	}

	/// <summary>
	/// Clears the existing items.
	/// </summary>
	public void ClearExistingItems() {
		// Remove any existing created items
		if (allCreatedItems.Count > 0) {
			for (int i = 0; i < allCreatedItems.ToArray().Length; i++) {
				Destroy (allCreatedItems[i]);
			}
		}
		
		// Clear the list
		allCreatedItems.Clear();
	}

	/// <summary>
	/// Creates the new item from a source reference.
	/// </summary>
	/// <returns>The new item from.</returns>
	/// <param name="sourceReference">Source reference.</param>
	/// <param name="toPerformOnItem">To perform on item.</param>
	/// <param name="sameParentAsPrototype">If set to <c>true</c> same parent as prototype.</param>
	/// <param name="keepWorldPosition">If set to <c>true</c> keep world position.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public GameObject CreateNewItemFrom<T>(T sourceReference, RefreshAction<T> toPerformOnItem = null,  bool sameParentAsPrototype = true, bool keepWorldPosition = false) {
		GameObject clonedItem = Instantiate(itemPrototype) as GameObject;
		clonedItem.SetActive(true);
		if (sameParentAsPrototype)
			clonedItem.transform.SetParent(itemPrototype.transform.parent, keepWorldPosition);
		if (toPerformOnItem != null)
			toPerformOnItem.Invoke(clonedItem, sourceReference);

		allCreatedItems.Add(clonedItem);
		return clonedItem;
	}
}
