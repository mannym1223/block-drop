using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockScriptable", menuName = "Block Drop/BlockScriptable")]
public class BlockScriptable : ScriptableObject
{
	public List<float> rotations = new List<float>(); // used when spawning
	public BlockTypeList blockTypes; // used for block colors
	public GameObject outlineBox;

	[SerializeField]
	[Tooltip("Specifies where the cubes will spawn in the grid relative to spawn point.")]
	protected List<Vector3Int> cubeSpawns = new List<Vector3Int>();

	public List<Vector3Int> GetRandomRotationSpawns ()
	{
		int rotation = (int)(Random.value * (4));
		List<Vector3Int> rotatedSpawns = cubeSpawns.GetRange(0, cubeSpawns.Count);

		switch (rotation) {
			case 0: // no rotation
				break;
			case 1: // rotate 90 degrees
				for (int i = 0; i < rotatedSpawns.Count; i++)
				{
					Vector3Int newIndex = new Vector3Int(rotatedSpawns[i].z, rotatedSpawns[i].y, rotatedSpawns[i].x);
					rotatedSpawns[i] = newIndex;
				}
				break;
		}
		return rotatedSpawns;
	}
}
