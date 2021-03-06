﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.OData.Test.Domain;
using NHibernate.OData.Test.Support;
using NUnit.Framework;

namespace NHibernate.OData.Test.Criterions
{
    [TestFixture]
    internal class DynamicComponents : DomainTestFixture
    {
        [Test]
        public void SelectByComponentMemberEq()
        {
            Verify(
                "DynamicComponent/DynamicString eq 'Value 1'",
                Session.QueryOver<Child>().Where(x => x.Name == "Child 1").List()
            );
        }

        [Test]
        public void SelectByChildComponentMemberEq()
        {
            Verify(
                "Child/DynamicComponent/DynamicString eq 'Value 1'",
                Session.QueryOver<Parent>().Where(x => x.Name == "Parent 1").List()
            );
        }

        // In NHibernate a component is considered to be null when all of its mapped properties are null
        [Test]
        public void SelectByComponentIsNull()
        {
            Verify(
                "DynamicComponent eq null",
                Session.QueryOver<Child>().Where(x => x.Name == "Child 10").List()
            );
        }

        [Test]
        public void SelectByChildComponentIsNull()
        {
            Verify(
                "Child/DynamicComponent eq null",
                Session.QueryOver<Parent>().Where(x => x.Name == "Parent 10").List()
            );
        }

        [Test]
        public void SelectByComponentManyToOneMember()
        {
            Verify(
                "Child/DynamicComponent/DynamicChildRef/Name eq 'Child 4'",
                Session.QueryOver<Parent>().Where(x => x.Name == "Parent 5").List()
            );
        }

        [Test]
        public void SelectByMultipleConditionsOnManyToOneMember()
        {
            Verify(
                "(Child/DynamicComponent/DynamicChildRef/Name eq 'Child 4') or (Child/DynamicComponent/DynamicChildRef/Name eq 'Child 5')",
                Session.QueryOver<Parent>().Where(x => x.Name == "Parent 5" || x.Name == "Parent 6").List()
            );
        }

        [Test]
        public void SelectByComponentManyToOneMemberCaseInsensitive()
        {
            Verify(
                "child/dynamiccomponent/dynamicchildref/name eq 'Child 4'",
                Session.QueryOver<Parent>().Where(x => x.Name == "Parent 5").List(),
                new ODataParserConfiguration { CaseSensitive = false }
            );
        }

        [Test]
        public void ThrowsOnUnknownComponentMember()
        {
            VerifyThrows<Parent>("Child/DynamicComponent/UnknownMember eq 10", typeof(QueryException));
        }
    }
}