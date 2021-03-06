﻿namespace OpenTl.Server.UnitTests.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Moq;

    using OpenTl.Common.Interfaces;
    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.Requests.Contacts;
    using OpenTl.Server.UnitTests.Builders;
    using OpenTl.Server.UnitTests.Extensions;

    using Ploeh.AutoFixture;

    using Xunit;

    public class RequestGetContactsTest: BaseTest
    {
        [Fact]
        public async Task SimpleTest()
        {
            var userLists = new List<User>(Fixture.CreateMany<User>(6));

            var currentUser = this.BuildUser(userLists.ToArray());
            userLists.Add(currentUser);

            var session = this.BuildSession(currentUser);
          
            this.BuildUserService(userLists.ToArray());

          
            RegisterSingleton<RequestGetContactsHandlerGrain>();

            this.RegisterAllMaps();
            Mapper.AssertConfigurationIsValid();

                
            var handler = Resolve<RequestGetContactsHandlerGrain>();

            var request = new RequestGetContacts();
            var requestData = Serializer.SerializeObject(request);

            var responseData = await handler.Handle(session.AuthKey.ToGuid(), requestData);
            var response = Serializer.DeserializeObject(responseData).Cast<TContacts>();


            Assert.All(response.Contacts.Items, contact => currentUser.Contacts.Any(c => c.UserId == contact.UserId));
            Assert.All(response.Users.Items, user => currentUser.Users.Contains(user.Cast<TUser>().Id));
        }
    }
}