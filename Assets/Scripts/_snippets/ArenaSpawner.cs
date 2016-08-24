using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using SaFrLib;

public class ArenaSpawner : MonoBehaviour {

	/// ARENA SPAWNER
	/// The ultimate tool for testing enemies in a level!
	/// 
	/// WHAT
	/// 	The Arena Spawner can spawn enemies at random or predetermined locations. It's a self-contained way to
	/// 	playtest different enemy types in your game.
	/// 
	/// HOW
	/// 	Setup (in Editor Mode)
	/// 	1. Create an instance of the Arena Spawner prefab.
	/// 	2. Populate the list of GameObjects to instantiate as prefabs.
	/// 	3. Create empty GameObjects as children of the Arena Spawner prefab to use as spawn points.
	/// 
	/// 	(TODO: Not implemented yet!)
	/// 	To work in Placement mode (in Play Mode):
	/// 	1. Toggle 'Placement Mode' on with the button in the UI.
	/// 	2. Click the appropriate button to spawn an enemy.
	/// 	3. Click where you want the enemy to spawn.

	//

	/// <summary>
	/// The list of all the enemy types available.
	/// </summary>
	public List<GameObject> enemyTypes = new List<GameObject>();
	/// <summary>
	/// The list of available spawn points for enemies.
	/// </summary>
	public List<Transform> spawnPoints = new List<Transform>();
	public MenuRefresher spawnMenu;

	private void Start() {
		// Remove duplicates from enemyTypes
		enemyTypes = enemyTypes.Distinct().ToList();
		// Populate spawner UI with all spawnable prefabs
		spawnMenu.Setup<GameObject>(enemyTypes.ToArray(), (createdButton, enemy) => {
			// Change button text to reflect enemy name
			Text text = SaFrMo.GetComponentInTree<Text>(createdButton);
			text.text = enemy.name;
			// Callback to spawn enemy
			AttachEnemyAndSpawn(createdButton, enemy);
		});
	}

	private void AttachEnemyAndSpawn(GameObject createdButton, GameObject enemy) {
		// Trigger spawn when button clicked
		Button button = createdButton.GetComponent<Button>();
		button.onClick.AddListener(() => {
			Spawn(enemy);
		});
	}

	/// <summary>
	/// Spawn the specified enemy in a random spawn point.
	/// </summary>
	/// <param name="toSpawn">To spawn.</param>
	public void Spawn(GameObject toSpawn) {
		// Error if no spawn points
		if (spawnPoints.Count <= 0) {
			Debug.Log("No spawn points in the ArenaSpawner! Assign a Transform to the \"Spawn Points\" list to fix this error.");
			return;
		}
		// Find a random spawn point 
		Transform selectedPoint = SaFrLib.SaFrMo.Pick<Transform>(spawnPoints);
		Spawn(toSpawn, selectedPoint);
	}

	/// <summary>
	/// Spawn the specified enemy in the specified location.
	/// </summary>
	/// <param name="toSpawn">To spawn.</param>
	/// <param name="location">Location.</param>
	public void Spawn(GameObject toSpawn, Transform location) {
		// Spawn the desired object in place
		GameObject spawned = Instantiate(toSpawn) as GameObject;
		spawned.transform.position = location.position;
	}

}
