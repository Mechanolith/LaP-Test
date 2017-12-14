using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a rectangular section of screenspace based on two corners (as Vector2s).
/// </summary>
[System.Serializable]
public class Div
{
	public Vector2 topRightBound;
	public Vector2 bottomLeftBound;
}

/// <summary>
/// Handles everything related to the spatial division of screenspace for word spawning.
/// Can return random divisions, mark them as used, and free them up again based on and index or Div.
/// </summary>
public class DivisionSystem : MonoBehaviour {

	[Tooltip("If enabled, will render sphere gizmos on all Divs to show their bounds.")]
	public bool debugMode = false;

	List<Div> divisions = new List<Div>();
	List<Div> usedDivisions = new List<Div>();
	int xCount;
	int yCount;
	float xDivSize;
	float yDivSize;

	/// <summary>
	/// Generates a list containing a grid of Divs based on the amount of padding and number of divs required.
	/// </summary>
	/// <param name="_xCount">The number of divs required on the X axis.</param>
	/// <param name="_yCount">The number of divs required on the Y axis.</param>
	/// <param name="_xPad">Amount of X padding in normalised screen space.</param>
	/// <param name="_yPad">Amount of Y padding in normalised screen space.</param>
	public void GenerateDivs(int _xCount, int _yCount, float _xPad, float _yPad, bool _outerOnly)
	{
		xCount = _xCount;
		yCount = _yCount;
		DeriveDivSize(_xPad, _yPad);

		for(int indexA = 0; indexA < xCount * yCount; ++indexA)
		{
			Div curDivision = new Div();
			Vector2 coords = DeriveCoords(indexA);  //This removes the need for a nested loop.

			//Check if the div is going to be on the outer edge.
			bool xIsOuter = (coords.x == 0 || coords.x == (xCount - 1));
			bool yIsOuter = (coords.y == 0 || coords.y == (yCount - 1));

			//If we're doing every division OR we're doing the outer divs and this div is on the outer edge.
			if (!_outerOnly || (_outerOnly && (xIsOuter || yIsOuter)))
			{
				//Calculate Bottom Left Bound on the X axis.
				float prevPad = _xPad * (1f + (coords.x * 2f));
				float prevDiv = xDivSize * coords.x;

				curDivision.bottomLeftBound.x = prevPad + prevDiv;

				//Calculate Bottom Left Bound on the Y axis.
				prevPad = _yPad * (1f + (coords.y * 2f));
				prevDiv = yDivSize * coords.y;

				curDivision.bottomLeftBound.y = prevPad + prevDiv;

				//Calculate Top Right Bounds by adding the known div sizes.
				curDivision.topRightBound.x = curDivision.bottomLeftBound.x + xDivSize;
				curDivision.topRightBound.y = curDivision.bottomLeftBound.y + yDivSize;

				//Add it to the list.
				divisions.Add(curDivision);
			}
		}
	}

	/// <summary>
	/// Returns a coordinate as a Vector2, representing the position of a division in the grid, based on a single integer.
	/// </summary>
	/// <param name="_index"></param>
	/// <returns></returns>
	private Vector2 DeriveCoords (int _index)
	{
		Vector2 result = Vector2.zero;

		result.x = _index % xCount;
		result.y = (_index - result.x)/xCount;

		return result;
	}

	/// <summary>
	/// Sets the size of divisions' X and Y, based on the amount of padding and number of divisions needed.
	/// </summary>
	/// <param name="_xPad">Amount of X padding in normalised screen space.</param>
	/// <param name="_yPad">Amount of Y padding in normalised screen space.</param>
	private void DeriveDivSize(float _xPad, float _yPad)
	{
		float totalDivSizeX = 1f - (xCount * _xPad * 2f);
		xDivSize = totalDivSizeX / xCount;


		float totalDivSizeY = 1f - (yCount * _yPad * 2f);
		yDivSize = totalDivSizeY / yCount;
	}

	/// <summary>
	/// Returns a random unused Div and moves it to the used list.
	/// </summary>
	/// <returns></returns>
	public Div GetRandomDiv()
	{
		int index = Random.Range(0, divisions.Count);
		Div result = divisions[index];

		usedDivisions.Add(divisions[index]);
		divisions.RemoveAt(index);

		return result;
	}

	/// <summary>
	/// Takes a div from the used list and makes it unused again.
	/// </summary>
	/// <param name="_division"></param>
	public void FreeDiv(Div _division)
	{
		divisions.Add(_division);
		usedDivisions.Remove(_division);
	}

	public void OnDrawGizmos()
	{
		if (debugMode)
		{
			Vector3 target = Vector3.zero;

			for (int indexA = 0; indexA < divisions.Count; ++indexA)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
				target = new Vector3(divisions[indexA].bottomLeftBound.x, divisions[indexA].bottomLeftBound.y, 0f);
				target = Camera.main.ViewportToWorldPoint(target);
				Gizmos.matrix = Matrix4x4.TRS(target, transform.localRotation, new Vector3(0.5f, 0.5f, 0.5f));
				Gizmos.DrawSphere(Vector3.zero, 1);

				Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
				target = new Vector3(divisions[indexA].topRightBound.x, divisions[indexA].topRightBound.y, 0f);
				target = Camera.main.ViewportToWorldPoint(target);
				Gizmos.matrix = Matrix4x4.TRS(target, transform.localRotation, new Vector3(0.5f, 0.5f, 0.5f));
				Gizmos.DrawSphere(Vector3.zero, 1);
			}

			for (int indexB = 0; indexB < usedDivisions.Count; ++indexB)
			{
				Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
				target = new Vector3(usedDivisions[indexB].bottomLeftBound.x, usedDivisions[indexB].bottomLeftBound.y, 0f);
				target = Camera.main.ViewportToWorldPoint(target);
				Gizmos.matrix = Matrix4x4.TRS(target, transform.localRotation, new Vector3(0.5f, 0.5f, 0.5f));
				Gizmos.DrawSphere(Vector3.zero, 1);

				Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
				target = new Vector3(usedDivisions[indexB].topRightBound.x, usedDivisions[indexB].topRightBound.y, 0f);
				target = Camera.main.ViewportToWorldPoint(target);
				Gizmos.matrix = Matrix4x4.TRS(target, transform.localRotation, new Vector3(0.5f, 0.5f, 0.5f));
				Gizmos.DrawSphere(Vector3.zero, 1);
			}
		}
	}
}
