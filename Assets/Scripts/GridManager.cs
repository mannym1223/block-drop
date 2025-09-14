using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridManager", menuName = "BlockDrop/GridManager")]
public class GridManager : ScriptableObject
{
    [SerializeField]
    private List<Block> blockList;

    public List<Block> BlockList => blockList;
}
