using System;
using BRPLUSA.Domain.Entities;
using BRPLUSA.Domain.Entities.Events;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;

namespace BRPLUSA.Domain.Tests.Entities
{
    /// <summary>This class contains parameterized unit tests for UserOpenedModelEvent</summary>
    //[PexClass(typeof(UserOpenedModelEvent))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public class UserOpenedModelEventTest
    {
        private const string _modelName = "17640_ELEC";

        [Test]
        public void ShouldCreateUserOpenModelEventWithCorrectEventAndModelName()
        {
            var state = new UserOpenedModelEvent(_modelName);

            Assert.That(state.EventType == WorksharingEventType.ModelOpen);
            Assert.That(state.ModelName == _modelName);
        }

        [Test]
        public void ShouldCreateEventWithCurrentUser()
        {
            var state = new UserOpenedModelEvent(_modelName);

            Assert.That(state.User.ComputerName == Environment.MachineName);
            Assert.That(state.User.Name == Environment.UserName);
        }
    }
}
