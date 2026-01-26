using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridManager", menuName = "BlockDrop/BlockList")]
public class BlockTypeList : ScriptableObject
{
    [SerializeField]
    private List<Block> blockList;

    public List<Block> BlockList => blockList;

	[SerializeField]
	private List<BlockScriptable> blockTypes;

	public List<BlockScriptable> BlockTypes => blockTypes;

	public List<Material> materials = new List<Material>();
}
