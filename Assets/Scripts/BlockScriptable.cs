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
		int rotation = (int)(Random.value * (4f));
		List<Vector3Int> rotatedSpawns = cubeSpawns.GetRange(0, cubeSpawns.Count);

		switch (rotation) {
			case 0: // no rotation
				break;
			case 1: // rotate 90 degrees
				for (int i = 0; i < rotatedSpawns.Count; i++)
				{
					// swap x and z
					Vector3Int newIndex = new Vector3Int(rotatedSpawns[i].z, rotatedSpawns[i].y, rotatedSpawns[i].x);
					rotatedSpawns[i] = newIndex;
				}
				break;
			case 2: // rotate 180 degrees
				for (int i = 0; i < rotatedSpawns.Count; i++)
				{
					// make z negative
					Vector3Int newIndex = new Vector3Int(rotatedSpawns[i].x, rotatedSpawns[i].y, -rotatedSpawns[i].z);
					rotatedSpawns[i] = newIndex;
				}
				break;
			case 3: // rotate 270 degrees
				for (int i = 0; i < rotatedSpawns.Count; i++)
				{
					// swap x and z, then make x negative
					Vector3Int newIndex = new Vector3Int(-rotatedSpawns[i].z, rotatedSpawns[i].y, rotatedSpawns[i].x);
					rotatedSpawns[i] = newIndex;
				}
				break;
			default:
				break;
		}
		return rotatedSpawns;
	}
}
