namespace Count4U.Model.Services
{
    public class Order
    {
        public int Price { get; set; }
        public string Product { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Order)) return false;
            return Equals((Order) obj);
        }

        public bool Equals(Order other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Product, Product);
        }

        public override int GetHashCode()
        {
            return (Product != null ? Product.GetHashCode() : 0);
        }
    }
}