using System.Collections.Generic;

namespace Coding.Challenge.Firstname.Lastname
{
    public class Car
    {
        public string Brand { get; set; }
        public bool IsEcoFriendly { get; set; }
        public string Fuel { get; set; }
        public List<string> Models { get; set; }
        public static Car Lookup(string id)
        {
            return new Car
            {
                Brand = "Mercedes-Benz",
                Fuel = "Hybrid",
                IsEcoFriendly = true,
                Models = new List<string> { "0", "1", "2" }
            };
        }
    }
}
