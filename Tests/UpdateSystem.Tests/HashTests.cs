using System;
using Xunit;

namespace CodeElements.UpdateSystem.Tests
{
    public class HashTests
    {
        [Fact]
        public void TestInitializationWithArray()
        {
            var data = new byte[23];
            new Hash(data);
        }

        [Fact]
        public void TestInitializationWithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Hash(null));
        }

        [Fact]
        public void TestInitializationWithEmptyArray()
        {
            Assert.Throws<ArgumentException>(() => new Hash(new byte[0]));
        }

        [Theory]
        [InlineData("4ee9d89777d9606455f44dca120c3c956f87d09f")] //SHA1
        [InlineData("b94d4eb7fac7cd42cd7ee037d29e1bebef88ec6a333eb461fa0a44617dcc0c3a")] //SHA256
        [InlineData("d278a7ebbb2b518dd02004679e5a8ae04e9ae563db4e31ca3eec9398d9f05df5bc1f86a15d4d184724dbd36bd9b825995db6f7bd7395dbc85546ea6d0820f94e")] //SHA512
        [InlineData("9d40e9046ed9645812c4f85dabffe889")] //MD5
        [InlineData("4EE9D89777D9606455F44DCA120C3C956F87D09F")] //SHA1 upper case
        [InlineData("b94d4EB7fAC7cd42cd7EE037d29e1bebef88ec6a333EB461fa0a44617dcc0c3A")] //SHA256 mixed case
        public void TestParseValidHash(string hashValue)
        {
            Hash.Parse(hashValue);

            Assert.True(Hash.TryParse(hashValue, out var _));
        }

        [Theory]
        [InlineData("ADA")]
        [InlineData("akl")]
        [InlineData("ak")]
        [InlineData("??ä#")]
        [InlineData("FFF")]
        [InlineData("F!!!")]
        public void TestParseInvalidHash(string hashValue)
        {
            Assert.ThrowsAny<ArgumentException>(() => Hash.Parse(hashValue));
            Assert.False(Hash.TryParse(hashValue, out var _));
        }

        [Fact]
        public void TestEqualMethod()
        {
            const string hash = "FFFF";
            var hash1 = Hash.Parse(hash);
            var hash2 = new Hash(new[] { byte.MaxValue, byte.MaxValue });

            Assert.Equal(hash1, hash2);

            hash2 = new Hash(new byte[] {213, 19});
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void TestEqualOperator()
        {
            const string hash = "FFFF";
            var hash1 = Hash.Parse(hash);
            var hash2 = new Hash(new[] { byte.MaxValue, byte.MaxValue });

            Assert.True(hash1 == hash2);

            hash2 = new Hash(new byte[] { 213, 19 });
            Assert.False(hash1 == hash2);

            //same reference
            hash2 = hash1;
            Assert.True(hash1 == hash2);
        }

        [Fact]
        public void TestUnequalOperator()
        {
            const string hash = "FFFF";
            var hash1 = Hash.Parse(hash);
            var hash2 = new Hash(new[] { byte.MaxValue, byte.MaxValue });

            Assert.False(hash1 != hash2);

            hash2 = new Hash(new byte[] { 213, 19 });
            Assert.True(hash1 != hash2);

            //same reference
            hash2 = hash1;

            Assert.False(hash1 != hash2);
        }
    }
}