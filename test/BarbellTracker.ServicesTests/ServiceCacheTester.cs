using BarbellTracker.DomainCode;
using BarbellTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BarbellTracker.ServicesTests
{
    public class ServiceCacheTester
    {
        ServiceCache<object> _sut = new ServiceCache<object>(); // System Under tests

        [Fact]
        public void RequestItem_WithAKeyThatDontExist_willReturnFalseAndOutItemAsNull()
        {
            // Arrange
            TrackedInformation key = new TrackedInformation()
            {
                Id = "TestId"
            };

            // Act
            var HasItem = _sut.TryGetCachedItem(key, out var OutPutItem);

            // Assert
            Assert.False(HasItem);
            Assert.Null(OutPutItem);
        }
        [Fact]
        public void RequestItem_WithAKeyThatExist_willReturnTrueAndOutIteml()
        {
            // Arrange
            TrackedInformation key = new TrackedInformation()
            {
                Id = "TestId"
            };
            var Item = new object();
            _sut.AddItemToCache(key, Item);


            // Act
            var HasItem = _sut.TryGetCachedItem(key, out var OutPutItem);


            // Assert
            Assert.True(HasItem);
            Assert.StrictEqual(Item, OutPutItem);
        }
        [Fact]
        public void AddItem_WithAKeyThatAllreadyExist_willThrowKeyAlreadyExistExeption()
        {
            // Arrange
            TrackedInformation key = new TrackedInformation()
            {
                Id = "TestId"
            };
            var Item1 = new object();
            var Item2 = new object();



            // Act
            _sut.AddItemToCache(key, Item1);
            Action AddItem = () => _sut.AddItemToCache(key, Item2);


            // Assert
            Assert.Throws<KeyAlreadyExist>(AddItem);

        }
        [Fact]
        public void AddItem_AtMaxCacheSize_willRemoveFirstItem()
        {
            // Arrange
            TrackedInformation FirstKey = new TrackedInformation()
            {
                Id = "FirstID"
            };
            _sut.AddItemToCache(FirstKey, new object());

            var MaxSize = _sut.Max_Cache_Size;

            AddUnknowDummyItemsToSUT(MaxSize - 1);

            TrackedInformation LastKey = new TrackedInformation()
            {
                Id = "LastID"
            };



            // Act
            _sut.AddItemToCache(LastKey, new object());
            var containsKey = _sut.TryGetCachedItem(FirstKey, out var item);

            // Assert
            Assert.False(containsKey);
            Assert.Null(item);
        }


        [Fact]
        public void ChangeMaxCacheSize_AtInit_WillChangeMaxCacheSize()
        {
            // Arrange
            var NewCacheSize = _sut.Max_Cache_Size + 1;

            // Act
            var ServiceCacheWithNewCacheSize = new ServiceCache<object>()
            {
                Max_Cache_Size = NewCacheSize
            };


            // Assert
            Assert.Equal(NewCacheSize, ServiceCacheWithNewCacheSize.Max_Cache_Size);

        }



        private void AddUnknowDummyItemsToSUT(int TimesToAdd)
        {
            for (int i = 0; i < TimesToAdd; i++)
            {
                TrackedInformation Key = new TrackedInformation()
                {
                    Id = $"IDCount {i}"
                };

                _sut.AddItemToCache(Key, new object());
            }
        }
    }
}
