using System;
using DemoLibrary;
using Shouldly;
using Xunit;

namespace XUnitTestProject_DemoLibrary
{
    public class UnitTest_Dog
    {
        [Fact]
        public void Test_Default_Constructor()
        {
            var dog = new Dog();
            dog.ShouldNotBeNull();
            dog.Name.ShouldNotBeNull();
            var name = Guid.NewGuid().ToString();
            dog.Name = name;
            dog.Name.ShouldBe(name);

        }
        [Fact]
        public void Test_Name_Constructor()
        {
            var name = Guid.NewGuid().ToString();
            var dog = new Dog(name);
            dog.ShouldNotBeNull();
            dog.Name.ShouldNotBeNull();
            dog.Name.ShouldBe(name);
        }
    }
}
