using UnityEngine;
using views = TestProject.Gameplay.Views;

namespace TestProject.Gameplay {

    public class WallUnit : Unit {

        public WallUnit(views::View view) : base(view) { }

        private Wall data;
        private float[] healths;
        private float startHeight;

        public void SetData(Wall data) {

            this.data = data;
            this.healths = new float[this.data.nodes.Length];
            for (int i = 0; i < this.healths.Length; ++i) {

                this.healths[i] = data.health;

            }

            var nodeIdx = this.data.nodes[0];
            var node = this.battle.map.nodes[nodeIdx];
            this.startHeight = node.height;
            
        }

        public void Damage(Vector3 center, float rangeSqr, float value) {

            for (int i = 0; i < this.data.nodes.Length; ++i) {

                var nodeIdx = this.data.nodes[i];
                var node = this.battle.map.nodes[nodeIdx];
                var position = node.GetWorldPosition();
                if ((position - center).sqrMagnitude <= rangeSqr) {

                    if (this.battle.map.HasNonZeroHeightBetweenPoints(center, position - (position - center).normalized * this.battle.map.nodeSize) == true) continue;
                    
                    this.healths[i] -= value;
                    node.height = Mathf.Lerp(0f, this.startHeight, this.healths[i] / this.data.health);
                    node.hasActor = node.height > 0f;
                    node.isLand = node.height <= 0f;
                    this.battle.map.nodes[nodeIdx] = node;

                }

            }

            for (int i = 0; i < this.healths.Length; ++i) {

                if (this.healths[i] > 0f) return;

            }

            this.SetDead();

        }

    }

}