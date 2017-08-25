
namespace OpenTl.Server.UnitTests
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    using Moq;

    using OpenTl.Common.Crypto;
    using OpenTl.Schema.Auth;

    using Xunit;

    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.Back.Entities;
    using OpenTl.Server.Back.Requests;
    using OpenTl.Server.UnitTests.Builders;
    using TAuthorization = OpenTl.Schema.Auth.TAuthorization;
    
    public class RequestSignInTest
    {
        [Fact]
        public async Task SimpleTest()
        {
            var request = new RequestSignIn
                          {
                              PhoneNumber = "7777",
                              PhoneCode = "7777",
                              PhoneCodeHashAsBinary = SHA1Helper.ComputeHashsum(Encoding.UTF8.GetBytes("7777"))
                          };
            var requestData = Serializer.SerializeObject(request);

            var mUserRepo = RepositoryBuilder.Build<User>();
            var userRepo = mUserRepo.Object;
            var user = new User
                       {
                           FirstName = "qqqq",
                           LastName = "bbbb",
                           Id = Guid.NewGuid(),
                           PhoneNumber = request.PhoneNumber,
                           UserId = 1
                       };
            userRepo.Create(user);
            
            var grain = new RequestSignInHandlerGrain(userRepo);
            var responseData = await grain.Handle(1, requestData);
            
            var response = Serializer.DeserializeObject(responseData).Cast<TAuthorization>();
            var responseUser = response.User.Cast<TUser>(); 
            
            Assert.Equal(responseUser.Phone, request.PhoneNumber);
            Assert.Equal(responseUser.FirstName, user.FirstName);
            Assert.Equal(responseUser.LastName, user.LastName);
            
            mUserRepo.Verify(r => r.GetAll(), Times.Once);
        }
    }
}