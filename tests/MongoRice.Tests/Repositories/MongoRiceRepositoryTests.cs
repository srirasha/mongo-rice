using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MongoRice.Configurations;
using MongoRice.Repositories;
using MongoRice.Tests.Mocks.Entities;
using Xunit;

namespace MongoRice.Tests.Repositories
{
    public class MongoRiceRepositoryTests
    {
        private readonly Fixture _fixture = new();
        Mapper _mapper = new(new MapperConfiguration(map => { map.CreateMap<MockEntity, MockEntityDocument>().ReverseMap(); }));

        [Fact]
        public void Instanciation_Should_Throw_ValidationException_when_ConnectionString_IsNullOrEmpty()
        {
            IMongoConfiguration mongoConfiguration = _fixture.Build<MongoConfiguration>()
                                                             .With(prop => prop.ConnectionString, string.Empty)
                                                             .Create();

            Action constructor = () => { _ = new MongoRiceRepository<MockEntity, MockEntityDocument>(mongoConfiguration, _mapper); };

            constructor.Should().Throw<ValidationException>("Connection string is empty");
        }

        [Fact]
        public void Instanciation_Should_Throw_ValidationException_when_Database_IsNullOrEmpty()
        {
            IMongoConfiguration mongoConfiguration = _fixture.Build<MongoConfiguration>()
                                                             .With(prop => prop.Database, string.Empty)
                                                             .Create();

            MapperConfiguration mapperConfiguration = new MapperConfiguration(map => map.CreateMap<MockEntity, MockEntityDocument>().ReverseMap()) { };


            Action constructor = () => { _ = new MongoRiceRepository<MockEntity, MockEntityDocument>(mongoConfiguration, _mapper); };

            constructor.Should().Throw<ValidationException>("Database is empty");
        }
    }
}