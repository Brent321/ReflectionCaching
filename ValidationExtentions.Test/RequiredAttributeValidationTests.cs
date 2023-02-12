using ReflectionCaching.Models;

namespace ValidationExtentions.Test
{
    public class RequiredAttributeValidationTests
    {
        [Test]
        public void ValidateRequiredAttributeObject()
        {
            // Make a new object with missing required fields.
            var test = new Person();
            // Get validation messages.
            var messages = test.Validate();
            Assert.That(messages, Is.Not.Null);
            Assert.That(messages, Has.Count.EqualTo(2));
            test.FirstName = "Bill";
            messages = test.Validate();
            Assert.That(messages, Has.Count.EqualTo(1));
        }
    }
}