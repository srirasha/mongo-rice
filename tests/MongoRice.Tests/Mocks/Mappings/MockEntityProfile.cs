using AutoMapper;
using MongoRice.Tests.Mocks.Entities;

namespace MongoRice.Tests.Mocks.Mappings
{
    public class MockEntityProfile : Profile
    {
        public MockEntityProfile()
        {
            CreateMap<MockEntity, MockEntityDocument>().ReverseMap();
        }
    }
}