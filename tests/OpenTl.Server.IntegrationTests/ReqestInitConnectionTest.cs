﻿namespace OpenTl.Server.IntegrationTests
{
    using OpenTl.Common.MtProto;
    using OpenTl.Schema;
    using OpenTl.Schema.Help;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.IntegrationTests.Entities;
    using OpenTl.Server.IntegrationTests.Helpers;

    using Xunit;

    public class ReqestInitConnectionTest
    {
        [Fact]
        public void SimpleTest()
        {
            var seqNumber = 0;

            AuthHelper.InitConnection(out var networkStream, ref seqNumber, out var session, out int serverTime);
            
            var request = new RequestInvokeWithLayer
                          {
                              Layer = SchemaInfo.SchemaVersion,
                              Query = new RequestInitConnection
                                      {
                                          ApiId = 0,
                                          AppVersion = "1.0.0",
                                          DeviceModel = "PC",
                                          LangCode = "en",
                                          LangPack = "tdesktop",
                                          SystemLangCode = "en",
                                          Query = new RequestGetConfig(),
                                          SystemVersion = "Win 10.0"
                                      }
                          };
            
            var requestData = Serializer.SerializeObject(request);
            var encryptedRequestData = MtProtoHelper.FromClientEncrypt(requestData, session, seqNumber);
            seqNumber++;
            
            var encryptedResponseData = networkStream.SendAndRecive(encryptedRequestData, seqNumber);
            var responseData = MtProtoHelper.FromServerDecrypt(encryptedResponseData, session, out var authKeyId, out var serverSalt, out var sessionId, out var messageId, out var sNumber);
            
            var response = Serializer.DeserializeObject(responseData);
        }
    }
}