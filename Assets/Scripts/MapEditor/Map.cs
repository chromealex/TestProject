using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TestProject.MapEditor {

	public class Map : MonoBehaviour {
	
		public Vector3Int size;
		public float nodeSize = 0.5f;
	
		public Node[] nodes;
		public Wall[] walls;
		public float wallHealth;
	
		public MapInfo mapInfo {
			get { return new MapInfo() { nodeSize = this.nodeSize, nodes = this.nodes, walls = this.walls, center = this.transform.position + new Vector3(this.size.x, 0f, this.size.z) * 2f, size = this.size }; }
		}
	
		[ContextMenu("Scan")]
		public void Scan() {
	
			// These lines are not optimized because of who cares about editor-only performance
			var walls = new Dictionary<Object, List<Vector3>>();
			var nodes = new List<Node>();
			var ix = 0;
			var iy = 0;
			var mp = 1f / this.nodeSize;
			var sizeX = this.size.x * mp;
			var sizeY = this.size.z * mp;
			for (var x = 0; x < sizeX; ++x) {
	
				for (var y = 0; y < sizeY; ++y) {
	
					var pos2d = new Vector2(x / mp, y / mp);
					bool isLand;
					var height = this.GetHeightAtPoint(pos2d, out isLand, walls);
					var node = new Node() {
						coord = new Vector2Int(ix, iy),
						worldPosition = pos2d,
						height = height,
						isLand = isLand,
					};
	
					nodes.Add(node);
	
					++iy;
	
				}
	
				++ix;
				iy = 0;
	
			}
	
			var mapWalls = new List<Wall>();
			foreach (var item in walls) {
	
				var wall = new Wall();
				var nodesIdx = new List<int>();
				foreach (var p in item.Value) {
	
					var x = Mathf.RoundToInt(p.x * (1f / this.nodeSize));
					var y = Mathf.RoundToInt(p.z * (1f / this.nodeSize));
					var coord = new Vector2Int(x, y);
					var width = Mathf.FloorToInt(this.size.x * mp);
					var idx = width * coord.x + coord.y;
	
					var found = false;
					foreach (var w in mapWalls) {
	
						if (w.nodes.Contains(idx) == true) {
	
							found = true;
							break;
	
						}
	
					}
	
					if (found == false) nodesIdx.Add(idx);
	
				}
	
				wall.health = this.wallHealth;
				wall.nodes = nodesIdx.ToArray();
				mapWalls.Add(wall);
	
			}
	
			this.nodes = nodes.ToArray();
			this.walls = mapWalls.ToArray();
	
		}
	
		private float GetHeightAtPoint(Vector2 position, out bool isLand, Dictionary<Object, List<Vector3>> walls) {
	
			var pos = this.transform.position;
			var rootPosition = new Vector2(pos.x, pos.z);
	
			var pos3d = new Vector3(position.x + rootPosition.x, this.size.y, position.y + rootPosition.y);
			var ray = new Ray(pos3d, Vector3.down);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, this.size.y * 2f, -1) == true) {
	
				List<Vector3> list;
				if (walls.TryGetValue(hit.collider, out list) == false) {
	
					list = new List<Vector3>();
					walls.Add(hit.collider, list);
	
				}
	
				list.Add(hit.point);
	
				isLand = false;
				return hit.point.y;
	
			}
	
			isLand = true;
			return 0f;
	
		}
	
		public void OnDrawGizmos() {
	
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(this.transform.position + new Vector3(this.size.x * 0.5f, 0f, this.size.z * 0.5f), this.size);
	
			if (this.nodes != null) {
	
				var position = this.transform.position;
				var rootPosition = new Vector2(position.x, position.z);
	
				const float nodesPadding = 0.1f;
	
				var offset = new Vector3(1f, 0f, 1f) * this.nodeSize * 0.5f + new Vector3(rootPosition.x, 0f, rootPosition.y);
				for (int i = 0; i < this.nodes.Length; ++i) {
	
					var node = this.nodes[i];
					Gizmos.color = (node.isLand == true && node.hasActor == false ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f));
					Gizmos.DrawCube(node.GetWorldPosition() + offset, new Vector3(this.nodeSize - nodesPadding, 0.1f, this.nodeSize - nodesPadding));
	
				}
	
			}
	
		}
	
	}

}