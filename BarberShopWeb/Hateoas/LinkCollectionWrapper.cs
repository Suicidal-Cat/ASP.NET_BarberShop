namespace BarberShopWeb.Hateoas
{
    public class LinkCollectionWrapper<T>
    {
        public T? Value { get; set; }
        public List<Link>? Links { get; set; }
        public LinkCollectionWrapper()
        {
        }
        public LinkCollectionWrapper(T value,List<Link> links)
        {
            Value = value;
            this.Links = links;
        }

    }
}
