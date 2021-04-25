using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cobalt
{
	[CreateAssetMenu(fileName = "GregariousRuleTile.asset", menuName = "2D/Tiles/Gregarious Rule Tile", order = 2000)]
	public class GregariousRuleTile : RuleTile<GregariousNeighbour>
	{
        public override bool RuleMatch(int neighbor, TileBase other)
        {
            switch (neighbor)
            {
                case TilingRule.Neighbor.This: return other != null;
                case TilingRule.Neighbor.NotThis: return other == null;
            }
            return true;
        }
	}

	public class GregariousNeighbour : RuleTile.TilingRuleOutput.Neighbor
	{

	}
}