﻿using Mountains.V1.Client;
using Mountains.V1.Client.Dtos;
using NUnit.Framework;
using System;
using System.Linq;

namespace Mountains.V1.Web.IntergrationTests
{
    [TestFixture]
    public sealed class UserTestFixture : TestFixtureBase
    {
        [Test]
        public void GetUsers()
        {
            MountainsClient client = CreateMountainsClient();

            UserCollectionDto users = client.GetUsersAsync().Result;

            Assert.IsNotNull(users);
            Assert.IsNotNull(users.Users);
        }

        [Test]
        public void CreateUser()
        {
            MountainsClient client = CreateMountainsClient();

            UserDto expectedUser = CreateUserDto();

            UserDto actualUser = client.CreateUserAsync(expectedUser).Result;
            expectedUser.Id = actualUser.Id;

            Assert.IsNotNull(actualUser);
            AssertIsEqual(expectedUser, actualUser);

            UserDto user = client.GetUserAsync(actualUser.Id).Result;

            Assert.IsNotNull(user);
            AssertIsEqual(expectedUser, user);
        }

        [Test]
        public void CreateUserWithNullName()
        {
            MountainsClient client = CreateMountainsClient();

            UserDto expectedUser = CreateUserDto();
            expectedUser.Name = null;

            Assert.IsNull(client.CreateUserAsync(expectedUser).Result);
        }

        [Test]
        public void CreateUserWithEmptyName()
        {
            MountainsClient client = CreateMountainsClient();

            UserDto expectedUser = CreateUserDto();
            expectedUser.Name = "";

            Assert.IsNull(client.CreateUserAsync(expectedUser).Result);
        }

        [Test]
        public void CreateUserWithLongName()
        {
            MountainsClient client = CreateMountainsClient();

            UserDto expectedUser = CreateUserDto();
            expectedUser.Name = Enumerable.Range(1, 500).Select(x => "a").Aggregate((x, y) => x + y);

            Assert.IsNull(client.CreateUserAsync(expectedUser).Result);
        }

        [Test]
        public void UpdateUser()
        {
            MountainsClient client = CreateMountainsClient();

            UserDto expectedUser = CreateUserDto();

            UserDto actualUser = client.CreateUserAsync(expectedUser).Result;
            expectedUser.Id = actualUser.Id;
            expectedUser.Name = "Updated " + Guid.NewGuid().ToString();

            actualUser = client.UpdateUserAsync(expectedUser.Id, expectedUser).Result;

            Assert.IsNotNull(actualUser);
            AssertIsEqual(expectedUser, actualUser);

            UserDto user = client.GetUserAsync(actualUser.Id).Result;

            Assert.IsNotNull(user);
            AssertIsEqual(expectedUser, user);
        }
    }
}
