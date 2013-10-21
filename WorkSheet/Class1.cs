using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkSheet
{

    public enum Dimension
    {
        Who,
        What,
        Why,
        When,
        Where,

    }



    public class Scope
    {
        public readonly string Name;
        public readonly Dimension Dimension;
        public readonly Scope ParentScope;
        public Scope(string name, Dimension dimension, Scope parentScope)
        {
            Name = name;
            Dimension = dimension;
            ParentScope = parentScope;
        }

        public Scope(string name, Dimension dimension) : this(name, dimension, Scope.None) { }
        public Scope(string name, Scope parentScope) : this(name, parentScope.Dimension, parentScope) { }

        private static Scope s;

        private Scope() { Name = "None"; }

        public static Scope None
        {
            get
            {
                if (s == null)
                {
                    s = Activator.CreateInstance<Scope>();
                }
                return s;
            }
        }


        public Scope AssignParent(Scope parentScope)
        {
            if (this.ParentScope != Scope.None && this.Dimension != parentScope.Dimension)
            {
                throw new ArgumentException();
            }
            return new Scope(this.Name, this.Dimension, this.ParentScope);
        }

        public Scope RemoveParentScope()
        {
            return AssignParent(Scope.None);
        }

    }
}
