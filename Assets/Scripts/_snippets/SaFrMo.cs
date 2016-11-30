using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using System.IO;

// Useful links for common dev tasks:
// =======================================
//		JSON visualizer, builder, loader: http://www.thomasfrank.se/downloadableJS/JSONeditor_example.html
//
//

namespace SaFrLib {
	public class SaFrMo : MonoBehaviour {

		// Boilerplate functions for all projects

		public static System.Random random = new System.Random(startingSeed);
		public static int randomSeed = startingSeed;
		public const int startingSeed = -1;

		/// <summary>
		/// Pick a random item from a list.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Pick<T>(List<T> source, int seed = startingSeed, bool forceNewRandom = false) {
			TrySetSeed (seed, forceNewRandom);

			return source[random.Next(source.Count)];
		}

		/// <summary>
		/// Pick a random enum from all available values.
		/// </summary>
		/// <returns>The enum.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T PickEnum<T>(int seed = startingSeed, bool forceNewRandom = false) {
			TrySetSeed (seed, forceNewRandom);

			Array values = Enum.GetValues(typeof(T));
			return (T)(values.GetValue(random.Next(values.Length)));
		}

		/// <summary>
		/// Gets the recommended subtitle time for a given string.
		/// </summary>
		/// <returns>The subtitle time.</returns>
		/// <param name="text">Text.</param>
		/// <param name="minLength">Minimum length.</param>
		/// <param name="wordsPerSecond">Words per second.</param>
		public static float GetSubtitleTime(string text, float minLength = 2f, float wordsPerSecond = 2f) {
			// Count number of spaces and add one to determine total number of words
			return Mathf.Clamp((float)(text.Count(x => x == ' ') + 1) / wordsPerSecond, minLength, Mathf.Infinity);
		}

		/// <summary>
		/// Roll for success.
		/// </summary>
		/// <param name="chanceOfSuccess">Chance of success.</param>
		public static bool Roll(int chanceOfSuccess = 50, int seed = startingSeed, bool forceNewRandom = false) {
			TrySetSeed (seed, forceNewRandom);

			return (random.Next (0, 101) < chanceOfSuccess);
		}
		
		/// <summary>
		/// Shuffle the array.
		/// Fisher-Yates shuffle from http://www.dotnetperls.com/fisher-yates-shuffle
		/// </summary>
		/// <typeparam name="T">Array element type.</typeparam>
		/// <param name="array">Array to shuffle.</param>
		public static T[] Shuffle<T>(T[] array, int seed = startingSeed, bool forceNewRandom = false)
		{
			TrySetSeed (seed, forceNewRandom);

			int n = array.Length;
			for (int i = 0; i < n; i++)
			{
				// NextDouble returns a random number between 0 and 1.
				int r = i + (int)(random.NextDouble() * (n - i));
				T t = array[r];
				array[r] = array[i];
				array[i] = t;
			}

			return array;
		}

		/// <summary>
		/// Generates a random string of all capital letters.
		/// </summary>
		/// <returns>The random string.</returns>
		/// <param name="length">Length.</param>
		public static string GenerateRandomString(int length = 8, int seed = startingSeed, bool forceNewRandom = false) {
			TrySetSeed (seed, forceNewRandom);

			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string result = new string(
				Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)])
				.ToArray());
			return result;
		}
			
		/// <summary>
		/// Loads the sprite at path.
		/// </summary>
		/// <returns>The <see cref="UnityEngine.Sprite"/>.</returns>
		/// <param name="path">Path.</param>
		public static Sprite LoadSpriteAt(string path) {

			Sprite s = default(Sprite);
			if (File.Exists(path)) {
				byte[] bytes = File.ReadAllBytes(path);
				Texture2D t = new Texture2D(1, 1);
				t.LoadImage(bytes);
				Rect rect = new Rect(0, 0, t.width, t.height);
				s = Sprite.Create(t, rect, new Vector2(.5f, .5f));
			}
			return s;
		}

		/// <summary>
		/// Points the sprite toward point. Assumes the sprite's "forward" point is (0, 1), or straight up.;
		/// </summary>
		/// <param name="toRotate">To rotate.</param>
		/// <param name="rotateTo">Rotate to.</param>
		public static void PointSpriteTowardPoint(Transform toRotate, Vector3 rotateTo) {
			Vector3 dir = rotateTo - toRotate.position;
			dir.Normalize();
			toRotate.transform.rotation = Quaternion.Euler( 0, 0, Mathf.Atan2 ( dir.y, dir.x ) * Mathf.Rad2Deg - 90);
		}

		/// <summary>
		/// Converts a number of seconds to clock value (MM:SS).
		/// </summary>
		/// <returns>The seconds to clock value.</returns>
		/// <param name="seconds">Seconds.</param>
		public static string ConvertSecondsToClockValue(float seconds) {
			string toReturn = "";
			int minutes = Mathf.RoundToInt(seconds) / 60;
			toReturn += minutes.ToString("D2");
			toReturn += ":";
			string sec = Mathf.RoundToInt(seconds % 60f).ToString("D2");
			if (sec == "60")
				sec = "00";
			toReturn += sec;
			return toReturn;
		}

		/// <summary>
		/// Gets the total scale of this Transform, moving up the hierarchy along the way.
		/// </summary>
		/// <returns>The world scale.</returns>
		/// <param name="transform">Transform.</param>
		public static Vector3 GetWorldScale(Transform transform) {
			Vector3 toReturn = transform.localScale;
			Transform parent = transform.parent;

			while (parent != null) {
				toReturn = Vector3.Scale(toReturn, parent.localScale);
				parent = parent.parent;
			}

			return toReturn;
		}
		
		/// <summary>
		/// Search a GameObject, its children, then its parents for a Component.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentInTree<T>(GameObject go) where T : Component {
			T toReturn = go.GetComponent<T>();
			if (toReturn == null)
				toReturn = go.GetComponentInChildren<T>();
			if (toReturn == null)
				toReturn = go.GetComponentInParent<T>();			
			return toReturn;
		}

		/// <summary>
		/// Gets a component. Creates one if none present.
		/// </summary>
		/// <returns>The or create.</returns>
		/// <param name="go">Go.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetOrCreate<T>(GameObject go) where T:Component { 
			T toReturn = go.GetComponent<T>(); 
			if (toReturn == null) { 
				toReturn = go.AddComponent<T>(); 
			} 
			return toReturn; 
		}

		/// <summary>
		/// Finds the specified component anywhere in the scene, or creates an empty GameObject with that component included.
		/// </summary>
		/// <returns>The or create.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T FindOrCreate<T>() where T : Component {
			T toReturn = FindObjectOfType<T> ();
			if (toReturn == null) {
				GameObject createdGameObject = new GameObject (typeof(T).ToString () + " (Created by SaFrMo.FindOrCreate)", typeof(T));
				toReturn = createdGameObject.GetComponent<T> ();
			}
			return toReturn;
		}

		/// <summary>
		/// Determines if the layer to test is in a given layer mask.
		/// </summary>
		/// <returns><c>true</c> if the layer is in the given mask; otherwise, <c>false</c>.</returns>
		/// <param name="layerToTest">Layer to test.</param>
		/// <param name="mask">Mask.</param>
		public static bool IsInLayerMask(int layerToTest, LayerMask mask) {
			return (mask.value & 1 << layerToTest) != 0;
		}

		/// <summary>
		/// Get a random float between min (inclusive) and max (inclusive).
		/// </summary>
		/// <returns>The float.</returns>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public static float RandomFloat(float min = 0, float max = 1f, int seed = startingSeed, bool forceNewRandom = false) {

			TrySetSeed (seed, forceNewRandom);

			// Adapted from http://stackoverflow.com/questions/3365337/best-way-to-generate-a-random-float-in-c-sharp
			// Perform arithmetic in double type to avoid overflowing
			double range = (double) max - (double) min;
			double sample = random.NextDouble ();
			double scaled = (sample * range) + min;
			return (float)scaled;
		}

		/// <summary>
		/// Tries the set the random seed. Ignores if the seed is already the given value and `force` is set to false. 
		/// Returns whether or not a new System.Random instance was created.
		/// </summary>
		/// <param name="newSeed">New seed.</param>
		/// <param name="force">If set to <c>true</c> force new System.Random instance with given seed.</param>
		public static bool TrySetSeed (int newSeed, bool force = false) {
			if (newSeed != startingSeed && (randomSeed != newSeed || force)) {
				random = new System.Random (newSeed);
				randomSeed = newSeed;
				return true;
			}

			return false;
		}


		/// <summary>
		/// Gets a point in a 2D square region defined by two points, accounting for padding on all sides.
		/// </summary>
		/// <returns>The point in region.</returns>
		/// <param name="bottomLeft">Bottom left.</param>
		/// <param name="topRight">Top right.</param>
		/// <param name="paddingTop">Padding top.</param>
		/// <param name="paddingRight">Padding right.</param>
		/// <param name="paddingBottom">Padding bottom.</param>
		/// <param name="paddingLeft">Padding left.</param>
		public static Vector3 GetPointInRegion(
				Vector3 bottomLeft, Vector3 topRight, 
				float paddingTop = 0, float paddingRight = 0, float paddingBottom = 0, float paddingLeft = 0,
				int seed = -1, bool forceNewSeed = false
			) {
			TrySetSeed (seed, forceNewSeed);

			float x = SaFrMo.RandomFloat (bottomLeft.x + paddingLeft, topRight.x - paddingRight);
			float y = SaFrMo.RandomFloat (bottomLeft.y + paddingBottom, topRight.y - paddingTop);
			return new Vector3 (x, y);
		}

		/// <summary>
		/// Gets the point in percentage padded region.
		/// </summary>
		/// <returns>The point in percentage padded region.</returns>
		/// <param name="bottomLeft">Bottom left.</param>
		/// <param name="topRight">Top right.</param>
		/// <param name="paddingTop">Padding top. Percentage value from 0 to 1.</param>
		/// <param name="paddingRight">Padding right. Percentage value from 0 to 1.</param>
		/// <param name="paddingBottom">Padding bottom. Percentage value from 0 to 1.</param>
		/// <param name="paddingLeft">Padding left. Percentage value from 0 to 1.</param>
		/// <param name="seed">Seed.</param>
		/// <param name="forceNewSeed">If set to <c>true</c> force new seed.</param>
		public static Vector3 GetPointInPercentagePaddedRegion(
				Vector3 bottomLeft, Vector3 topRight, 
				float paddingTop = 0, float paddingRight = 0, float paddingBottom = 0, float paddingLeft = 0,
				int seed = -1, bool forceNewSeed = false
			) {

			// Doesn't call TrySetSeed because it uses GetPointInRegion, which itself calls TrySetSeed

			float width = topRight.x - bottomLeft.x;
			float height = topRight.y - bottomLeft.y;

			return GetPointInRegion (
				bottomLeft, 
				topRight,
				height * paddingTop,
				width * paddingRight,
				height * paddingBottom,
				width * paddingLeft,
				seed,
				forceNewSeed
			);
		}

		
		// ===================
		/* WIP BELOW */
		// ===================

	}
}