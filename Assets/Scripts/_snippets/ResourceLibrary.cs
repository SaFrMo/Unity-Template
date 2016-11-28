using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SaFrLib;

/// ResourceLibrary
/// A memory-lenient way to load and instantiate resources.
///
/// HOW
///     1. Create a ResourceLibrary GameObject somewhere in your scene OR use SaFrMo.FindOrCreate<ResourceLibrary>.
/// 	2. Call ResourceLibrary.GetResource(key) to load or create a copy of the desired resource. That's it!
///
public class ResourceLibrary : MonoBehaviour {

	/// <summary>
	/// The dictionary containing all resources loaded by this script.
	/// </summary>
	public Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
	/// <summary>
	/// A shortcut to get all keys in the dictionary.
	/// </summary>
	/// <value>The keys.</value>
	public List<string> keys { get { return dictionary.Keys.ToList (); } }

	/// <summary>
	/// Creates a copy of the desired resource by key name.
	/// Leave resourcePath blank to load from Resources/resourceKey, otherwise enter the relative path (for example, Resources/Foo/Bar would have its path
	/// be "Foo/Bar").
	/// </summary>
	/// <returns>The resource.</returns>
	/// <param name="resourceKey">Resource key.</param>
	public GameObject CopyResource(string resourceKey, string resourcePath = "") {

		// Do we have the resource loaded already?
		if (!dictionary.ContainsKey (resourceKey)) {
			SetResource(resourceKey, resourcePath);
		}

		// Clone the resource from the loaded copy
		GameObject toReturn = Instantiate (dictionary [resourceKey]) as GameObject;
		toReturn.SetActive (true);
		return toReturn;
	}

	/// <summary>
	/// Saves the resource by key name. Loads from Resource.Load if not prepared yet, otherwise returns null.
	/// Leave resourcePath blank to load from Resources/resourceKey, otherwise enter the relative path (for example, Resources/Foo/Bar would have its path
	/// be "Foo/Bar").
	/// </summary>
	/// <returns>The resource.</returns>
	/// <param name="resourceKey">Resource key.</param>
	public void SetResource(string resourceKey, string resourcePath = "") {
		// Ignore if we've already saved the resource
		if (dictionary.ContainsKey (resourceKey)) {
			return;
		}

		// Load resource and instantiate original
		GameObject resource = Resources.Load(resourcePath.Length > 0 ? resourcePath : resourceKey) as GameObject;
		GameObject instantiation = Instantiate (resource) as GameObject;
		instantiation.name = resource.name;
		instantiation.SetActive (false);
		instantiation.transform.SetParent (transform);

		// Save the reference to the resource
		dictionary.Add(resourceKey, instantiation);
	}

	/// <summary>
	/// Gets the original resource (ie, the GameObject instantiated by Resource.Load). If the original resource has already been set, the resourcePath
	/// parameter is unnecessary.
	/// Leave resourcePath blank to load from Resources/resourceKey, otherwise enter the relative path (for example, Resources/Foo/Bar would have its path
	/// be "Foo/Bar").
	/// </summary>
	/// <returns>The original.</returns>
	/// <param name="resourceKey">Resource key.</param>
	/// <param name="resourcePath">Resource path.</param>
	public GameObject GetOriginal(string resourceKey, string resourcePath = "") {
		SetResource (resourceKey, resourcePath);

		return dictionary [resourceKey];
	}

	/// <summary>
	/// Gets a random object instantiated by the ResourceLibrary.
	/// </summary>
	/// <returns>The random original (ie, the GameObject instantiated by Resource.Load).</returns>
	public GameObject GetRandomOriginal() {
		string key = SaFrMo.Pick<string> (keys);

		return dictionary [key];
	}

	/// <summary>
	/// Copies a random resource held in this ResourceLibrary.
	/// </summary>
	/// <returns>The random resource.</returns>
	public GameObject CopyRandomResource() {
		string key = SaFrMo.Pick<string> (keys);

		return CopyResource (key);
	}

}