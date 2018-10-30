using views = TestProject.Gameplay.Views;
using TestProject.Extensions;

namespace TestProject.Gameplay {

    public class Bomb : Unit {

        public Bomb(views::BombView view) : base(view) {

            this.throwSpeed = view.throwSpeed;
            this.damage = view.damage;
            this.damageRange = view.damageRange;
            this.damageRangeSqr = this.damageRange * this.damageRange;

        }

        private float throwSpeed;
        private float damage;
        private float damageRange;
        private float damageRangeSqr;
        
        public override void UpdateLogic(float deltaTime) {
            
            base.UpdateLogic(deltaTime);

            var pos = this.position;
            pos += UnityEngine.Vector3.down * deltaTime * this.throwSpeed;
            this.SetPosition(pos);

            if (pos.y <= this.battle.map.GetHeightAtPoint(pos)) {

                this.SetDead();
            
            }

        }

        public override void OnDead() {
            
            base.OnDead();
            
            this.Fire();
            
        }

        public void Fire() {

            var position = this.position;

            {
                var list = ListPool<SoldierUnit>.Get();
                this.battle.gameplay.GetUnits(list);

                for (int i = 0; i < list.Count; ++i) {

                    var unit = list[i];
                    if ((unit.position - position).sqrMagnitude <= this.damageRangeSqr) {

                        // check for walls
                        if (this.battle.map.HasNonZeroHeightBetweenPoints(unit.position, position) == true) continue;

                        unit.Damage(this.damage);

                    }

                }

                ListPool<SoldierUnit>.Release(list);
            }

            {
                var listWalls = ListPool<WallUnit>.Get();
                this.battle.gameplay.GetUnits(listWalls);

                for (int i = 0; i < listWalls.Count; ++i) {

                    var unit = listWalls[i];
                    unit.Damage(position, this.damageRangeSqr, this.damage);

                }

                ListPool<WallUnit>.Release(listWalls);
            }
            
        }

    }

}