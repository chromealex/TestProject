using System.Collections.Generic;
using UnityEngine;
using db = TestProject.Database;
using views = TestProject.Gameplay.Views;

namespace TestProject.Gameplay {

    public abstract class ModuleBase {

        protected Battle battle { private set; get; }

        public ModuleBase(Battle battle) {

            this.battle = battle;

        }

        public virtual void UpdateLogic(float deltaTime) { }
        public virtual void UpdateVisual(float deltaTime) { }

    }

    public abstract class LogicElement {

        protected Battle battle { get; private set; }
        protected bool isAlive { get; private set; }

        public LogicElement() {

            this.isAlive = true;
            this.battle = Battle.current;

        }

        public bool IsAlive() {
            
            return this.isAlive;
            
        }

        public void SetDead() {

            this.isAlive = false;
            this.OnDead();

        }

        public virtual void OnDead() { }
        public virtual void UpdateLogic(float deltaTime) { }
        public virtual void UpdateVisual(float deltaTime) { }
        
    }

    public class Battle {

        public enum State {
        
            Initialized,
            Playing,
            Paused,
            Stopped,
        
        };

        internal static Battle current;
        
        internal GameplayModule gameplay { private set; get; }
        public InputModule input { private set; get; }

        internal db::UnitsConfig unitsConfig { private set; get; }
        internal db::BombsConfig bombsConfig { private set; get; }
        internal MapInfo map { private set; get; }

        private readonly List<ModuleBase> modules = new List<ModuleBase>();
        public State state { private set; get; }

        public Battle(MapInfo map, db::UnitsConfig unitsConfig, db::BombsConfig bombsConfig) {

            Battle.current = this;

            this.map = map;
            this.unitsConfig = unitsConfig;
            this.bombsConfig = bombsConfig;

            this.modules.Add(this.gameplay = new GameplayModule(this));
            this.modules.Add(this.input = new InputModule(this));

            this.state = State.Initialized;

        }

        public bool Start() {

            if (this.state == State.Initialized) {

                this.state = State.Playing;
                return true;

            }

            return false;

        }

        public void Pause() {

            if (this.state == State.Playing) this.state = State.Paused;
        
        }

        public void Resume() {

            if (this.state == State.Paused) this.state = State.Playing;
        
        }

        public void Stop() {

            if (this.state == State.Paused || this.state == State.Playing) this.state = State.Stopped;
            
        }

        public void Update(float deltaTime) {

            if (this.state != State.Playing) return;
            
            Battle.current = this;
            
            var count = this.modules.Count;

            for (int i = 0; i < count; ++i) {

                this.modules[i].UpdateLogic(deltaTime);

            }

            for (int i = 0; i < count; ++i) {

                this.modules[i].UpdateVisual(deltaTime);

            }

        }

    }

}