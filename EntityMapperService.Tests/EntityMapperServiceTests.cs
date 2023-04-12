using Xunit;
using Model = EntityMapperService.Tests.Model;
using Dto = EntityMapperService.Tests.Dto;
using Newtonsoft.Json;
using ConversionUtilities;

namespace EntityMapperService.Tests
{
    public class EntityMapperServiceTests
    {
        [Fact]
        public void MainTest()
        {
            try
            {
                Model.A a = new Model.A()
                {
                    Id = Guid.NewGuid(),
                    Id2 = Guid.NewGuid(),
                    Prop1 = "Hello",
                    Bs = new List<Model.B>
                  {
                      new Model.B
                      {
                           Name = "B1",
                           C = new Model.C
                           {
                                ThingType = Model.ThingType.Hard,
                                 Label = "C1"
                           },
                           As = new List<Model.A>
                           {
                              new Model.A
                              {
                                   Prop1 = "asdad",
                                    Bs = new List<Model.B>
                                    {
                                        new Model.B
                                        {
                                             Name = "B1-2"
                                        }
                                    }
                              }
                           }
                      },
                      new Model.B
                      {
                           Name = "B2",
                           C = new Model.C
                           {
                                ThingType = Model.ThingType.Cold,
                                 Label = "C2"
                           },
                           As = new List<Model.A>
                           {
                              new Model.A
                              {
                                   Prop1 = "67567757",
                                    Bs = new List<Model.B>
                                    {
                                        new Model.B
                                        {
                                             Name = "B2-2"
                                        }
                                    }
                              }
                           }
                      }
                  }
                };

                EntityMappingService entityMappingService = new EntityMappingService();

                Dto.A converted = entityMappingService.Clone<Dto.A>(a);

                bool nameSame = a.Prop1 == converted.Prop1;
                bool IdSame = a.Id == converted.Id;
                bool Id2Same = a.Id2 == converted.Id2;
                bool Id3Same = a.Id3 == converted.Id3;
                bool listSame = JsonConvert.SerializeObject(a.Bs) == JsonConvert.SerializeObject(converted.Bs);

                Assert.True(nameSame && IdSame && Id2Same && Id3Same && listSame);
            }
            catch
            {
                Assert.True(false);
            }
        }
    }
}