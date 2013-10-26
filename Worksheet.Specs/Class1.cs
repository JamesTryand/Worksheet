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
                    where: new Scope("where", Dimension.Where),
                    why: new Scope("why", Dimension.Why)),
                Expect =
                {
                    measure => measure is Measure,
                    measure => measure.Who == "Bob",
                    measure => measure.What == new Scope("what", Dimension.What),
                    measure => measure.When == new Scope("when", Dimension.When),
                    measure => measure.Where == new Scope("where", Dimension.Where),
                    measure => measure.Why == new Scope("why", Dimension.Why),
                }
            };                       
        }

        public Specification CanDefineAMeasureInStages()
        {
            return new QuerySpecification<MeasureBuilder, Measure>()
            {
                On = () => new MeasureBuilder().What(new Scope("what", Dimension.What)).When(new Scope("when", Dimension.When)).Where(new Scope("where", Dimension.Where)).WithContext(new Scope("why", Dimension.Why)),
                When = builder => (Measure)(builder.WithName("Bob")),
                Expect =
                {
                    result => result is Measure,
                    result => result.Who == "Bob",
                    result => result.What == new Scope("what",Dimension.What),
                    result => result.When == new Scope("when",Dimension.When),
                    result => result.Where == new Scope("where",Dimension.Where),
                    result => result.Why == new Scope("why",Dimension.Why),
                }
            };
        }

        public Specification CanDefineMultipleMeasuresFromASingleBuilder()
        {
            return new QuerySpecification<MeasureBuilder, IEnumerable<Measure>>()
            {
                On = () => new MeasureBuilder().What(new Scope("what", Dimension.What)).When(new Scope("when", Dimension.When)).Where(new Scope("where", Dimension.Where)).WithContext(new Scope("why", Dimension.Why)),
                When = builder => new[] { "Bob", "John", "Sam" }.Select(name => (Measure)(builder.WithName(name))),
                Expect =
                {
                    measures => measures != null,
                    measures => measures.Count() == 3,
                    measures => measures.OrderBy(measure => measure.Who).First().Who == "Bob",
                    measures => measures.All(measure => measure.What == new Scope("what",Dimension.What)),
                    measures => measures.All(measure => measure.When == new Scope("when",Dimension.When)),
                    measures => measures.All(measure => measure.Where == new Scope("where",Dimension.Where)),
                    measures => measures.All(measure => measure.Why == new Scope("why",Dimension.Why)),
                }
            };
        }
    }


    /// <summary>
    /// This needs to be reworked to ensure that the value is stored
    /// </summary>
    public class AMeasureCanBeTicked
    {
        public Specification CanTickAMeasure()
        {
            return new QuerySpecification<MeasureTickBuilder, Tick<bool>>()
            {
                On = () => new MeasureTickBuilder(
                    who: "Bob",
                    what: new Scope("wat", Dimension.What),
                    when: new Scope("wen", Dimension.When),
                    where: new Scope("where", Dimension.Where),
                    why: new Scope("why", Dimension.Why),
                    tickTime: DateTime.Now,
                    tickUser: "Jeff"),
                When = source =>
                    new Tick<bool>(
                        true,
                        source.Measure,
                        source.DateTime,
                        source.User),
                Expect =
                {

                }
            };
        }
    }


    public class MeasureTickBuilder
    {
        public Measure Measure { get; private set; }
        public DateTime DateTime { get; private set; }
        public string User { get; private set; }

        public MeasureTickBuilder(string who, Scope what, Scope when, Scope where, Scope why, DateTime tickTime, string tickUser)
        {
            this.Measure = new Measure(who, what, when, where, why);
            this.DateTime = tickTime;
            this.User = tickUser;
        }
    }

    public class Tick<T>
    {
        public T Record { get; private set; }
        public DateTime DateTime { get; private set; }
        public string User { get; private set; }
        public Measure Measure { get; private set; }
        public Tick(T record, Measure measure, DateTime dateTime, string user)
        {
            Record = record;
            Measure = measure;
            DateTime = dateTime;
            User = user;
        }
    }

    public class ACollectionOfRelatedMeasuresIsAForm
    {

    }

    // Sparse tables
}
