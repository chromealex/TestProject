using views = TestProject.Gameplay.Views;

namespace TestProject.Gameplay {

    public abstract class Unit : LogicElement {

        private views::View view;
        public float health { private set; get; }

        public UnityEngine.Vector3 position { private set; get; }

        public Unit(views::View view) : base() {

            if (view != null) {

                this.view = TestProject.Extensions.GameObjectPool<views::View>.Get(view);
                this.health = view.health;

            }

        }

        public void SetPosition(UnityEngine.Vector3 position) {

            this.position = position;

        }

        public override void UpdateVisual(float deltaTime) {
            
            base.UpdateVisual(deltaTime);

            if (this.view != null) this.view.SetPosition(this.position);

        }

        public override void OnDead() {
            
            base.OnDead();

            if (this.view != null) TestProject.Extensions.GameObjectPool<views::View>.Release(this.view);
            this.view = null;

        }

        public void Damage(float value) {

            this.health -= value;
            if (this.health <= 0f) { 
            
                this.battle.gameplay.DespawnUnit(this.position);
                this.SetDead();
            
            }

        }

    }

}