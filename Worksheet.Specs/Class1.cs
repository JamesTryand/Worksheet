using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WorkSheet;
using System.Diagnostics.Contracts;
using Simple.Testing.ClientFramework;

namespace Worksheet.Specs
{
    /// <summary>
    /// who
    /// what
    /// why
    /// where
    /// when
    /// howoften
    /// 
    /// </summary>
    
    public class CanDefineAScope
    {
        public CanDefineAScope()
        {

        }

        public Specification CanDefineAScopeInTermsOfADimension()
        {
            return new ConstructorSpecification<Scope>()
            {
                When = () => new Scope("Bob", Dimension.Where),
                Expect = {
                    scope => scope.Name.Equals("Bob"),
                    scope => scope is Scope,
                    scope => scope.Dimension.Equals(Dimension.Where),
                    scope => scope.ParentScope.Equals(Scope.None)
                }
            };
        }

        public Specification CanDefineAScopeInTermsOfAnotherScope()
        {
            return new QuerySpecification<Scope, Scope>()
            {
                On = () => new Scope("Parent", Dimension.Where),
                When = parentScope => new Scope("Bob", parentScope),
                Expect =
                {
                    newScope => newScope.Dimension.Equals(Dimension.Where),
                    newScope => newScope.Name.Equals("Bob"),
                    newScope => newScope.ParentScope.Equals(new Scope("Parent", Dimension.Where))
                }
            };
        }

        public Specification CanAssignAParentScopeAfterTheFact()
        {
            return new QuerySpecification<Scope, Scope>()
            {
                On = () => new Scope("Parent", Dimension.Where),
                When = parentScope => new Scope("Bob", Dimension.Where).AssignParent(parentScope),
                Expect =
                {
                    newScope => newScope.Dimension.Equals(Dimension.Where),
                    newScope => newScope.Name.Equals("Bob"),
                    newScope => newScope.ParentScope.Equals(new Scope("Parent", Dimension.Where))
                }
            };
        }

        public Specification CanNotAssignAParentScopeAfterTheFactToAParentScopeOfDifferentDimensions()
        {
            return new FailingSpecification<Scope, ArgumentException>()
            {
                On = () => new Scope("Parent", Dimension.What),
                When = parentScope => new Scope("Bob", Dimension.Where).AssignParent(parentScope),
                Expect =
                {
                    result => result is ArgumentException,
                }

            };
        }

        //[Test]
        //public void CanDefineAScopeInTermsOfADimension1()
        //{
        //    var newScope = new Scope("Bob", Dimension.Where);
        //    // Assert.IsInstanceOf<Scope>(newScope);
        //    Assert.That(newScope.Name.Equals("Bob"));
        //    Assert.That(newScope.Dimension.Equals(Dimension.Where));
        //    Assert.That(newScope.ParentScope.Equals(Scope.None));
        //}

        //[Test]
        //public void CanDefineAScopeInTermsOfAnotherScope1()
        //{
        //    var parentScope = new Scope("Parent", Dimension.Where);

        //    var newScope = new Scope("Bob", parentScope);
        //    Assert.That(newScope.Dimension.Equals(Dimension.Where));
        //    Assert.That(newScope.Name.Equals("Bob"));
        //    Assert.That(newScope.ParentScope.Equals(parentScope));
        //}

        //[Test]
        //public void CanAssignAParentScopeAfterTheFact1()
        //{
        //    var parentScope = new Scope("Parent", Dimension.Where);
        //    var newScope = new Scope("Bob", Dimension.Where);
        //    var newScopeWithParent = newScope.AssignParent(parentScope);

        //    Assert.That(newScopeWithParent.Dimension.Equals(Dimension.Where));
        //    Assert.That(newScopeWithParent.Name.Equals("Bob"));
        //    Assert.That(newScopeWithParent.ParentScope.Equals(parentScope));
        //}

        //[Test]
        //public void CanNotAssignAParentScopeAfterTheFactToAParentScopeOfDifferentDimensions1()
        //{
        //    var parentScope = new Scope("Parent", Dimension.When);
        //    var newScope = new Scope("Bob", Dimension.Where);
        //    var newScopeWithParent = newScope.AssignParent(parentScope);

        //    Assert.That(newScopeWithParent.Dimension.Equals(Dimension.Where));
        //    Assert.That(newScopeWithParent.Name.Equals("Bob"));
        //    Assert.That(newScopeWithParent.ParentScope.Equals(parentScope));
            
        //}

    }

    public class CanDefineAMeasure
    {
        public Specification CanDefineAFullMeasure()
        {
            return new ConstructorSpecification<Measure>
            {
                When = () => new Measure(
                    who: "Bob",
                    what: new Scope("what", Dimension.What),
                    when: new Scope("when", Dimension.When),
                    where: new Scope("where", Dimension.Where)),
                Expect =
                {
                    measure => measure is Measure,
                    measure => measure.Who == "Bob",
                    measure => measure.What == new Scope("what", Dimension.What),
                    measure => measure.When == new Scope("when", Dimension.When),
                    measure => measure.Where == new Scope("where", Dimension.Where)
                }
            };
        }
    }

    public class Measure
    {
        public readonly string Who;
        public readonly Scope What;
        public readonly Scope When;
        public readonly Scope Where;

        public Measure(string who = "", Scope what = Scope.None, Scope when = Scope.None, Scope where = Scope.None)
        {
            Who = who;
            What = what;
            When = when;
            Where = where;
        }
    }
}
