using System;
using System.Threading.Tasks;

namespace Reviews.Core.Projections
{
    public abstract class Projection
    {
        private readonly Type type;

        protected Projection() => type = GetType();

        public abstract Task Handle(object e);
        
        public override string ToString() => type.Name;

        public static implicit operator string(Projection self) => self.ToString();
    }
}
