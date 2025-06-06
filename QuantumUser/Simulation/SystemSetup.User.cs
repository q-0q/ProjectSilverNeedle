﻿namespace Quantum {
  using System;
  using System.Collections.Generic;
  

  public static partial class DeterministicSystemSetup {
    static partial void AddSystemsUser(ICollection<SystemBase> systems, RuntimeConfig gameConfig, SimulationConfig simulationConfig, SystemsConfig systemsConfig) {
      // The system collection is already filled with systems coming from the SystemsConfig. 
      // Add or remove systems to the collection: systems.Add(new SystemFoo());
      
      systems.Add(new GameSetupSystem());
      systems.Add(new GameFSMSystem());
      systems.Add(new SpawnSystem());
      systems.Add(new FSMSystemA());
      systems.Add(new FSMSystemB());
      systems.Add(new HitstopSystem());
      systems.Add(new AnimationEntitySystem());
      systems.Add(new InputSystem());
      systems.Add(new PlayerCommandsSystem());
    }
  }
}