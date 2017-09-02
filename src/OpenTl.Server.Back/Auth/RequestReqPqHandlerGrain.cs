﻿using System;
using System.Threading.Tasks;

using OpenTl.Schema;
using OpenTl.Server.Back.Cache;
using OpenTl.Server.Back.Contracts.Auth;
using OpenTl.Server.Back.Helpers;

namespace OpenTl.Server.Back.Auth
{
    using OpenTl.Common.Auth.Server;

    public class RequestReqPqHandlerGrain:  BaseObjectHandlerGrain<RequestReqPq, TResPQ>, IRequestReqPqHandler
    {
        protected override Task<TResPQ> HandleProtected(ulong keyId, RequestReqPq obj)
        {
            var cache = AuthCache.NewAuthCache(keyId);

            var respq = Step1ServerHelper.GetResponse(obj.Nonce, RsaKeyHelper.PublicKeyFingerprint, out var p, out var q, out var serverNonce);
            
            cache.ServerNonce = serverNonce;
            cache.P = p;
            cache.Q = q;
            cache.Nonce = obj.Nonce;
            
            return Task.FromResult(respq);
        }
    }
}