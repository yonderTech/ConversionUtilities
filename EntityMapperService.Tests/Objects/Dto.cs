namespace EntityMapperService.Tests.Dto
{
    public enum ThingType
    {
        Hard,
        Cold,
        Hot
    }

    public class A
    {
        public string Prop1 { get; set; }
        public Guid Id { get; set; }
        public Guid Id2 { get; set; }
        public Guid Id3 { get; set; }
        public List<B> Bs { get; set; } = new List<B>();
    }


    public class B
    {
        public string Name { get; set; }
        public C C { get; set; }
        public List<A> As { get; set; }
    }



    public class C
    {
        public string Label { get; set; }
        public ThingType ThingType { get; set; }

    }
}