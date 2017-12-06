﻿namespace OpenTl.Server.Back.Requests
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;

    using BarsGroup.CodeGuard;

    using OpenTl.Common.Crypto;
    using OpenTl.Common.GuardExtensions;
    using OpenTl.Schema;
    using OpenTl.Schema.Auth;
    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.Contracts.Requests;
    using OpenTl.Server.Back.DAL.Interfaces;

    using IAuthorization = OpenTl.Schema.Auth.IAuthorization;
    using TAuthorization = OpenTl.Schema.Auth.TAuthorization;

    public class RequestSignInHandlerGrain : BaseObjectHandlerGrain<RequestSignIn, IAuthorization>,
                                             IRequestSignInHandler
    {
        private readonly IRepository<User> _userRepository;

        private readonly IRepository<ServerSession> _sessionStore;

        public RequestSignInHandlerGrain(IRepository<User> userRepository, IRepository<ServerSession> sessionStore)
        {
            _userRepository = userRepository;
            _sessionStore = sessionStore;
        }

        protected override Task<IAuthorization> HandleProtected(Guid keyId, RequestSignIn obj)
        {
            Guard.That(obj.PhoneCode).IsEqual("7777");
            Guard.That(obj.PhoneCodeHashAsBinary).IsItemsEquals(SHA1Helper.ComputeHashsum(Encoding.UTF8.GetBytes("7777")));

            var user = _userRepository.GetAll().Single(u => u.PhoneNumber == obj.PhoneNumber);

            var session = _sessionStore.Get(keyId);
            session.UserId = user.UserId;
            
            var result = Mapper.Map<TAuthorization>(user).Cast<IAuthorization>();

            return Task.FromResult(result);
        }
    }
}