using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiModels.V1 {
    public class ChangeSetItem<T> where T : class {
        public ChangeSetItem(T value, EntityState state) {
            Contract.NotNull(value, "value");

            this.Value = value;
            this.State = state;
        }

        private ChangeSetItem() {
            this.Value = default(T);
            this.State = EntityState.Detached;
        }

        public T Value { get; private set; }

        public EntityState State { get; private set; }

        public void UpdateState(EntityState state) {
            this.State = state;
        }
    }
}