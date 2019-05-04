using System;

namespace DemoLibrary
{
    public class Dog : IDog
    {
        public Dog()
        {
            Name = "Rover";
        }
        public Dog(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
