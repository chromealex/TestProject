using UnityEngine;

namespace TestProject {

	[System.Serializable]
	public struct MapInfo {
	
		public Node[] nodes;
		public Wall[] walls;
		public Vector3 center;
		public Vector3 size;
		public float nodeSize;
		
		public bool IsOnMap(Vector3 position) {
	
			return (position.x > this.size.x) == false && (position.y > this.size.y) == false && (position.z > this.size.z) == false;
	
		}
	
		public Vector3 GetRandomPointOnLand() {
	
			var list = TestProject.Extensions.ListPool<int>.Get();
			for (int i = 0; i < this.nodes.Length; ++i) {
	
				if (this.nodes[i].isLand == true) {
	
					list.Add(i);
	
				}
	
			}
	
			var pos = this.nodes[list[Random.Range(0, list.Count)]].GetWorldPosition();
			TestProject.Extensions.ListPool<int>.Release(list);
	
			return pos;
	
		}
	
		public Vector3 GetRandomPointOnLandAndEmpty() {
	
			var list = TestProject.Extensions.ListPool<int>.Get();
			for (int i = 0; i < this.nodes.Length; ++i) {
	
				if (this.nodes[i].isLand == true && this.nodes[i].hasActor == false) {
	
					list.Add(i);
	
				}
	
			}
	
			var pos = this.nodes[list[Random.Range(0, list.Count)]].GetWorldPosition();
			TestProject.Extensions.ListPool<int>.Release(list);
	
			return pos;
	
		}
	
		public bool HasNonZeroHeightBetweenPoints(Vector3 point1, Vector3 point2) {
	
			var dir = point2 - point1;
			var ray = new Ray(point1, dir);
			var distance = dir.magnitude;
	
			for (float d = 0f; d < distance; d += this.nodeSize * 0.5f) {
	
				var point = ray.GetPoint(d);
				var height = this.GetHeightAtPoint(point);
				if (height > 0f) {
	
					return true;
	
				}
	
			}
	
			return false;
	
		}
	
		public int GetNodeIndexByCoord(Vector2Int coord) {
	
			var mp = (1f / this.nodeSize);
			var width = Mathf.FloorToInt(this.size.x * mp);
			return width * coord.x + coord.y;
	
		}
	
		public Node GetNodeAtPoint(Vector2Int coord) {
	
			return this.nodes[this.GetNodeIndexByCoord(coord)];
	
		}
	
		public void SetNodeAtPoint(Vector2Int coord, Node node) {
	
			this.nodes[this.GetNodeIndexByCoord(coord)] = node;
	
		}
		
		public float GetHeightAtPoint(Vector3 position) {
	
			var coord = this.ConvertToCoord(position);
			var node = this.GetNodeAtPoint(coord);
			return node.height;
	
		}
	
		public void RemoveActor(Vector3 position) {
	
			var coord = this.ConvertToCoord(position);
			var node = this.GetNodeAtPoint(coord);
			node.hasActor = false;
			this.SetNodeAtPoint(coord, node);
	
		}
	
		public void PlaceActor(Vector3 position) {
	
			var coord = this.ConvertToCoord(position);
			var node = this.GetNodeAtPoint(coord);
			node.hasActor = true;
			this.SetNodeAtPoint(coord, node);
	
		}
	
		public bool HasActor(Vector3 position) {
	
			var coord = this.ConvertToCoord(position);
			return this.GetNodeAtPoint(coord).hasActor;
			
		}
	
		public Vector2Int ConvertToCoord(Vector3 position) {
	
			//position += new Vector3(this.size.x * 0.5f, 0f, this.size.z * 0.5f);
			var x = Mathf.RoundToInt(position.x * (1f / this.nodeSize));
			var y = Mathf.RoundToInt(position.z * (1f / this.nodeSize));
			return new Vector2Int(x, y);
	
		}
	
	}
	
	[System.Serializable]
	public struct Wall {
	
		public float health;
		public int[] nodes;
	
	}
	
	[System.Serializable]
	public struct Node {
	
		public Vector2Int coord;
		public Vector2 worldPosition;
		public float height;
		public bool isLand;
		public bool hasActor;
	
		public Vector3 GetWorldPosition() {
	
			return new Vector3(this.worldPosition.x, this.height, this.worldPosition.y);
	
		}
	
	}

}