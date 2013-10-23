using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkSheet
{

    public enum Dimension
    {
        Who, // who is entering the data, person/team
        What, // the thing being audited
        Why, // the context with which this is happening
        When, // the time/date / timespan for entering the data
        Where, // the physical location of where the data is being entered

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
    public class Measure
    {
        public readonly string Who;
        public readonly Scope What;
        public readonly Scope When;
        public readonly Scope Where;
        public readonly Scope Why;

        public Measure(string who, Scope what, Scope when, Scope where, Scope why)
        {
            Who = who;
            What = what;
            When = when;
            Where = where;
            Why = why;
        }
    }

    public class MeasureBuilder
    {
        public MeasureBuilder WithName(string who)
        {
            this.who = who;
            return this;
        }
        public MeasureBuilder What(Scope what)
        {
            this.what = what;
            return this;
        }
        public MeasureBuilder When(Scope when)
        {
            this.when = when;
            return this;
        }
        public MeasureBuilder Where(Scope where)
        {
            this.where = where;
            return this;
        }
        public MeasureBuilder WithContext(Scope why)
        {
            this.why = why;
            return this;
        }

        private string who;
        private Scope what;
        private Scope when;
        private Scope where;
        private Scope why;

        public static implicit operator Measure(MeasureBuilder builder)
        {
            return new Measure(
                who: builder.who ?? "",
                what: builder.what ?? Scope.None,
                when: builder.when ?? Scope.None,
                where: builder.where ?? Scope.None,
                why: builder.why ?? Scope.None);
        }


        public static implicit operator MeasureBuilder(Measure builder)
        {
            var b = new MeasureBuilder();
            if (!string.IsNullOrWhiteSpace(builder.Who)) { b.WithName(builder.Who); }
            if (builder.What != Scope.None) { b.What(builder.What); }
            if (builder.When != Scope.None) { b.When(builder.When); }
            if (builder.Where != Scope.None) { b.Where(builder.Where); }
            if (builder.Why != Scope.None) { b.Where(builder.Why); }
            return b;
        }
    }
}
