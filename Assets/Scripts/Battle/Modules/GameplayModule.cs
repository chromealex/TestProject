using UnityEngine;
using System.Collections.Generic;
using views = TestProject.Gameplay.Views;

namespace TestProject.Gameplay {

    public abstract class UnitsFactory<T, TView> where T : Unit where TView : views::View {

        public abstract T Create(TView view);

    }

    public class SoldierUnitFactory : UnitsFactory<SoldierUnit, views::SoldierView> {

        public override SoldierUnit Create(views::SoldierView view) {

            return new SoldierUnit(view);

        }

    }

    public class WallUnitFactory : UnitsFactory<WallUnit, views::WallView> {

        public override WallUnit Create(views::WallView view) {

            return new WallUnit(view);

        }

    }

    public class BombFactory : UnitsFactory<Bomb, views::BombView> {

        public override Bomb Create(views::BombView view) {

            return new Bomb(view);

        }

    }

    public class GameplayModule : ModuleBase {

        public GameplayModule(Battle battle) : base(battle) {

            this.bombsTimer = 0f;

            this.SpawnWalls();

        }

        private readonly SoldierUnitFactory soldierUnitFactory = new SoldierUnitFactory();
        private readonly WallUnitFactory wallUnitFactory = new WallUnitFactory();
        private readonly BombFactory bombUnitFactory = new BombFactory();
        private readonly List<Unit> units = new List<Unit>();

        private float bombsTime {
            get { return this.battle.bombsConfig.bombTime; }
        }

        private void SpawnWalls() {

            for (int i = 0; i < this.battle.map.walls.Length; ++i) {

                var wall = this.battle.map.walls[i];
                var unit = this.wallUnitFactory.Create(null);
                unit.SetData(wall);
                this.units.Add(unit);

                for (int j = 0; j < wall.nodes.Length; ++j) {

                    this.battle.map.PlaceActor(this.battle.map.nodes[wall.nodes[j]].GetWorldPosition());

                }
                
            }

        }

        public bool SpawnUnit(views::SoldierView view, Vector3 position) {

            if (this.battle.map.IsOnMap(position) == false) return false;
            if (this.battle.map.HasActor(position) == true) return false;

            var unit = this.soldierUnitFactory.Create(view);
            unit.SetPosition(position);
            this.units.Add(unit);
            this.battle.map.PlaceActor(position);
            
            return true;

        }

        public void DespawnUnit(Vector3 position) {

            this.battle.map.RemoveActor(position);
            
        }

        public void GetUnits<T>(List<T> output) where T : Unit {

            for (int i = 0, count = this.units.Count; i < count; ++i) {

                var unit = this.units[i] as T;
                if (unit != null) output.Add(unit);
            
            }

        }

        public override void UpdateLogic(float deltaTime) {

            base.UpdateVisual(deltaTime);

            for (int i = 0, count = this.units.Count; i < count; ++i) {

                var unit = this.units[i];
                if (unit.IsAlive() == true) {

                    unit.UpdateLogic(deltaTime);

                }
                
                if (unit.IsAlive() == false) {

                    this.units.RemoveAt(i);
                    --count;
                    --i;

                }

            }

            this.UpdateBombs(deltaTime);

        }

        public override void UpdateVisual(float deltaTime) {

            base.UpdateVisual(deltaTime);

            for (int i = 0, count = this.units.Count; i < count; ++i) {

                var unit = this.units[i];
                if (unit.IsAlive() == true) unit.UpdateVisual(deltaTime);

            }

        }

        private float bombsTimer = 0f;
        private void UpdateBombs(float deltaTime) {

            this.bombsTimer += deltaTime;

            if (this.bombsTimer >= this.bombsTime) {

                this.bombsTimer -= this.bombsTime;
                
                var point = this.battle.map.GetRandomPointOnLand();
                var view = this.battle.bombsConfig.GetViewByIndex(Random.Range(0, this.battle.bombsConfig.GetCount()));
                var bomb = this.bombUnitFactory.Create(view);
                bomb.SetPosition(new Vector3(point.x, view.throwHeight, point.z));
                this.units.Add(bomb);

            }

        }

    }

}